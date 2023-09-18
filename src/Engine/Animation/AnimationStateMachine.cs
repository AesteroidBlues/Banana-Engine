namespace BananaEngine.Animation;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using BananaEngine.Serialization;
using BananaEngine.Util;

using Newtonsoft.Json;

public class AnimatorFactory
{
    public static AnimationStateMachine Create(BlackboardVertex startNode, BlackboardVertex anyNode)
    {
        AnimationStateMachine animator = new AnimationStateMachine();
        Dictionary<int, AnimationStateMachine.AnimationState> seenNodes = new Dictionary<int, AnimationStateMachine.AnimationState>();
        Dictionary<int, List<AnimationStateMachine.AnimationTransition>> unsatisfiedTransitions = new Dictionary<int, List<AnimationStateMachine.AnimationTransition>>();
        foreach (var gNode in GraphIterator.Bfs(new List<IGraphNode> { startNode, anyNode }))
        {
            AnimationStateMachine.AnimationState animationState;
            var node = gNode as BlackboardVertex;
            if (node.HasParameter<bool>("IsStartState") || node.HasParameter<bool>("IsAnyState"))
            {
                animationState = new AnimationStateMachine.AnimationState(node.Id);
                if (node.HasParameter<bool>("IsStartState"))
                {
                    animator.StartState = animationState;
                }
                else
                {
                    animator.AnyState = animationState;
                }
            }
            else
            {
                animationState = new AnimationStateMachine.AnimationState(node.Id, node.GetParameter<Resource<IAnimation>>("Animation"));
            }

            if (unsatisfiedTransitions.ContainsKey(node.Id))
            {
                foreach (var transition in unsatisfiedTransitions[node.Id])
                {
                    transition.DestinationState = animationState;
                }

                unsatisfiedTransitions.Remove(node.Id);
            }

            foreach (var gEdge in node.GetEdges())
            {
                var edge = gEdge as BlackboardDirectedEdge;
                var transition = new AnimationStateMachine.AnimationTransition(edge.Id);
                foreach (var condition in edge.GetParameterList<Tuple<string, int, string>>("Conditions"))
                {
                    transition.AddCondition(condition);
                }

                transition.SourceState = animationState;
                animationState.AddTransition(transition);

                if (seenNodes.ContainsKey(edge.m_Destination.Id))
                {
                    transition.DestinationState = seenNodes[edge.m_Destination.Id];
                }
                else
                {
                    if (!unsatisfiedTransitions.ContainsKey(edge.m_Destination.Id))
                    {
                        unsatisfiedTransitions.Add(edge.m_Destination.Id, new List<AnimationStateMachine.AnimationTransition>());
                    }

                    unsatisfiedTransitions[edge.m_Destination.Id].Add(transition);
                }
            }

            seenNodes.Add(node.Id, animationState);
            animator.States.Add(animationState);
        }

        return animator;
    }
}

[JsonConverter(typeof(AnimationStateMachineConverter))]
public class AnimationStateMachine
{
    public List<AnimationState> States = new List<AnimationState>();

    public AnimationState AnyState;
    public AnimationState StartState;

    public AnimationStateMachine()
    {
    }

    public void AddState(ref AnimationState state)
    {
        States.Add(state);
    }

    #region Graph Types
    [JsonConverter(typeof(AnimationStateConverter))]
    public class AnimationState : IGraphNode
    {
        public int Id { get; private set; }
        public Resource<IAnimation> Animation { get; private set; }
        public List<AnimationTransition> Transitions { get; private set; }

        private static int m_NextId = 1;

        public AnimationState()
        : this(new Resource<IAnimation>("", new EmptyAnimation()))
        {
        }

        public AnimationState(int id)
        : this(id, new Resource<IAnimation>("", new EmptyAnimation()))
        {
        }

        public AnimationState(Resource<IAnimation> animation)
            : this(m_NextId++, animation)
        {
        }

        public AnimationState(int id, Resource<IAnimation> animation)
        {
            Id = id;
            Animation = animation;
            Transitions = new List<AnimationTransition>();
        }

        public void AddTransition(AnimationTransition transition)
        {
            Transitions.Add(transition);
        }

        public IEnumerable<IGraphEdge> GetEdges()
        {
            return Transitions.ConvertAll(x => x as IGraphEdge);
        }

        public void Update(float gameTime)
        {
            Animation.Value.Update(gameTime);
        }
    }

    [JsonConverter(typeof(AnimationTransitionConverter))]
    public class AnimationTransition : IGraphEdge
    {
        public int Id;

        public int SourceStateId { get; set; }
        public AnimationState SourceState { get; set; }
        
        public int DestinationStateId { get; set; }
        public AnimationState DestinationState { get; set; }

        public List<FloatCondition> m_FloatConditions = new List<FloatCondition>();
        public List<StringCondition> m_StringConditions = new List<StringCondition>();

        public AnimationTransition(int id)
        {
            Id = id;
        }

        public IGraphNode GetDestination()
        {
            return DestinationState;
        }

        public void AddCondition(Tuple<string, int, string> condition)
        {
            string variableName = condition.Item1;
            Operator op = (Operator)condition.Item2;
            if (float.TryParse(condition.Item3, out float value))
            {
                m_FloatConditions.Add(new FloatCondition(variableName, op, value));
            }
            else
            {
                Console.WriteLine($"Defaulting to string comparison for {condition.Item1} == {condition.Item3}");
                m_StringConditions.Add(new StringCondition(variableName, op, condition.Item3));
            }
        }

        public bool ShouldTransition(in Dictionary<string, Object> parameters)
        {
            foreach (var floatCondition in m_FloatConditions)
            {
                // Fail on unset parameters
                if (!parameters.ContainsKey(floatCondition.ParameterName))
                {
                    return false;
                }

                if (parameters[floatCondition.ParameterName] is float value)
                {
                    if (!floatCondition.Evaluate(value))
                    {
                        return false;
                    }
                }
            }

            foreach (var stringCondition in m_StringConditions)
            {
                // Fail on unset parameters
                if (!parameters.ContainsKey(stringCondition.ParameterName))
                {
                    return false;
                }

                if (parameters[stringCondition.ParameterName] is string value)
                {
                    if (!stringCondition.Evaluate(value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
    #endregion
}