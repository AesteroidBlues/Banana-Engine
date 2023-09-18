namespace BananaEngine.Serialization;

using System;
using System.Collections.Generic;

using BananaEngine;
using BananaEngine.Animation;
using BananaEngine.Util;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#region AnimationStateMachine Serialization
public class AnimationStateMachineConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(AnimationStateMachine);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        int startStateId = obj["StartState"].Value<int>();
        int anyStateId = obj["AnyState"].Value<int>();

        var states = serializer.Deserialize<Dictionary<int, AnimationStateMachine.AnimationState>>(obj["States"].CreateReader());
        var transitions = serializer.Deserialize<List<AnimationStateMachine.AnimationTransition>>(obj["Transitions"].CreateReader());

        Dictionary<int, AnimationStateMachine.AnimationTransition> transitionsBySourceId = new Dictionary<int, AnimationStateMachine.AnimationTransition>();
        foreach (var transition in transitions)
        {
            var sourceState = states[transition.SourceStateId];
            transition.SourceState = sourceState;
            sourceState.AddTransition(transition);

            transition.DestinationState = states[transition.DestinationStateId];
        }

        var animator = new AnimationStateMachine();
        animator.StartState = states[startStateId];
        animator.AnyState = states[anyStateId];
        return animator;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var animator = value as AnimationStateMachine;
        {
            writer.WriteStartObject(); // Animator

            writer.WritePropertyName("StartState");
            writer.WriteValue(animator.StartState.Id);

            writer.WritePropertyName("AnyState");
            writer.WriteValue(animator.AnyState.Id);

            Queue<AnimationStateMachine.AnimationTransition> transitions = new Queue<AnimationStateMachine.AnimationTransition>();
            writer.WritePropertyName("States");
            {
                writer.WriteStartObject(); // States

                foreach (var state in GraphIterator.Bfs(new List<IGraphNode> { animator.StartState, animator.AnyState }))
                {
                    var animationState = state as AnimationStateMachine.AnimationState;

                    writer.WritePropertyName(animationState.Id.ToString());
                    serializer.Serialize(writer, animationState);

                    foreach (var edge in animationState.GetEdges())
                    {
                        transitions.Enqueue(edge as AnimationStateMachine.AnimationTransition);
                    }
                }

                writer.WriteEndObject(); // States
            }

            writer.WritePropertyName("Transitions");
            {
                writer.WriteStartArray(); // Transitions
                foreach (var transition in transitions)
                {
                    serializer.Serialize(writer, transition);
                }
                writer.WriteEndArray(); // End Transitions
            }

            writer.WriteEndObject(); // End Animator
        }
    }
}

public class AnimationStateConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(AnimationStateMachine.AnimationState);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);

        var id = obj["Id"].Value<int>();

        var animationName = obj["Animation"].Value<string>();
        var animationState = new AnimationStateMachine.AnimationState(id, new Resource<IAnimation>(animationName));

        return animationState;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var state = value as AnimationStateMachine.AnimationState;

        writer.WriteStartObject();

        writer.WritePropertyName(nameof(state.Id));
        writer.WriteValue(state.Id.ToString());
        writer.WritePropertyName(nameof(state.Animation));
        writer.WriteValue(state.Animation.Name);

        writer.WriteEndObject();
    }
}

public class AnimationTransitionConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(AnimationStateMachine.AnimationTransition);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);

        var id = obj["Id"].Value<int>();
        var transition = new AnimationStateMachine.AnimationTransition(id);

        transition.SourceStateId = obj["SourceState"].Value<int>();
        transition.DestinationStateId = obj["DestinationState"].Value<int>();
        transition.m_FloatConditions = serializer.Deserialize<List<FloatCondition>>(obj["FloatConditions"].CreateReader());
        transition.m_StringConditions = serializer.Deserialize<List<StringCondition>>(obj["StringConditions"].CreateReader());

        return transition;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var transition = value as AnimationStateMachine.AnimationTransition;

        writer.WriteStartObject();

        writer.WritePropertyName(nameof(transition.Id));
        writer.WriteValue(transition.Id.ToString());

        writer.WritePropertyName(nameof(transition.SourceState));
        writer.WriteValue(transition.SourceState.Id);

        writer.WritePropertyName(nameof(transition.DestinationState));
        writer.WriteValue(transition.DestinationState.Id);

        writer.WritePropertyName("FloatConditions");
        WriteConditionList<FloatCondition, float>(writer, transition.m_FloatConditions);

        writer.WritePropertyName("StringConditions");
        WriteConditionList<StringCondition, string>(writer, transition.m_StringConditions);

        writer.WriteEndObject();
    }

    private void WriteConditionList<T1, T2>(JsonWriter writer, List<T1> conditionList) where T1 : SerializableCondition<T2>
    {
        writer.WriteStartArray();
        foreach (var condition in conditionList)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(condition.ParameterName));
            writer.WriteValue(condition.ParameterName);

            writer.WritePropertyName(nameof(condition.Operator));
            writer.WriteValue((int)condition.Operator);

            writer.WritePropertyName(nameof(condition.ReferenceValue));
            writer.WriteValue(condition.ReferenceValue);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
#endregion
