namespace BananaEngine.Serialization;

using System;
using System.Collections.Generic;

using BananaEngine;
using BananaEngine.Animation;
using BananaEngine.Util;

using LiteDB;

// [BsonMapperFor(typeof(SpriteSheetAnimation))]
// public static class SpriteSheetAnimationMapper
// {
//     private static BsonMapper m_Mapper = null;
//     public static void Register(BsonMapper dbMapper)
//     {
//         m_Mapper = dbMapper;
//         dbMapper.RegisterType(
//             serialize: SpriteSheetAnimationMapper.Serialize,
//             deserialize: SpriteSheetAnimationMapper.Deserialize
//         );
//     }

//     public static BsonValue Serialize(SpriteSheetAnimation animation)
//     {
//         return new BsonDocument()
//         {
//             ["SpriteWidth"] = animation.SpriteWidth,
//             ["SpriteHeight"] = animation.SpriteHeight,
//             ["AnimationSpeed"] = animation.AnimationSpeed,
//             ["LoopRestartFrame"] = animation.LoopRestartFrame,
//             ["Frames"] = m_Mapper.Serialize<List<Vector2i>>(animation.Frames)
//         };
//     }

//     public static SpriteSheetAnimation Deserialize(BsonValue bson)
//     {
//         var animation = new SpriteSheetAnimation();
//         animation.SpriteWidth = bson["SpriteWidth"];
//         animation.SpriteHeight = bson["SpriteHeight"];
//         animation.AnimationSpeed = bson["AnimationSpeed"];
//         animation.LoopRestartFrame = bson["LoopRestartFrame"];
//         animation.Frames = m_Mapper.Deserialize<List<Vector2i>>(bson["Frames"]);

//         return animation;
//     }
// }

[BsonMapperFor(typeof(AnimationStateMachine))]
public static class AnimationStateMachineMapper
{
    private static BsonMapper m_Mapper = null;

    public static void Register(BsonMapper dbMapper)
    {
        m_Mapper = dbMapper;

        dbMapper.RegisterType(
            serialize: AnimationStateMachineMapper.Serialize,
            deserialize: AnimationStateMachineMapper.Deserialize
        );

        dbMapper.RegisterType(
            serialize: AnimationStateMachineMapper.SerializeState,
            deserialize: AnimationStateMachineMapper.DeserializeState
        );

        dbMapper.RegisterType(
            serialize: AnimationStateMachineMapper.SerializeTransition,
            deserialize: AnimationStateMachineMapper.DeserializeTransition
        );

        dbMapper.RegisterType(
            serialize: AnimationStateMachineMapper.SerializeFloatCondition,
            deserialize: AnimationStateMachineMapper.DeserializeFloatCondition
        );

        dbMapper.RegisterType(
            serialize: AnimationStateMachineMapper.SerializeStringCondition,
            deserialize: AnimationStateMachineMapper.DeserializeStringCondition
        );
    }

    public static BsonValue Serialize(AnimationStateMachine stateMachine)
    {
        BsonDocument bson = new BsonDocument();
        bson["StartState"] = stateMachine.StartState.Id;
        bson["AnyState"] = stateMachine.AnyState.Id;

        Queue<AnimationStateMachine.AnimationTransition> transitions = new Queue<AnimationStateMachine.AnimationTransition>();
        
        var statesArray = new BsonArray();
        var transitionsArray = new BsonArray();

        foreach (var state in GraphIterator.Bfs(new List<IGraphNode> { stateMachine.StartState, stateMachine.AnyState }))
        {
            var animState = state as AnimationStateMachine.AnimationState;
            if (animState != null)
            {
                statesArray.Add(m_Mapper.Serialize(animState));
            }

            foreach (var transition in animState.Transitions)
            {
                transitionsArray.Add(m_Mapper.Serialize(transition));
            }
        }

        bson["States"] = statesArray;
        bson["Transitions"] = transitionsArray;

        return bson;
    }

    public static AnimationStateMachine Deserialize(BsonValue bson)
    {
        if (bson.Type == BsonType.Null)
        {
            return null;
        }

        int startStateId = bson["StartState"];
        int anyStateId = bson["AnyState"];

        var states = m_Mapper.Deserialize<List<AnimationStateMachine.AnimationState>>(bson["States"]);
        Dictionary<int, AnimationStateMachine.AnimationState> statesById = new Dictionary<int, AnimationStateMachine.AnimationState>();
        foreach (var state in states)
        {
            statesById.Add(state.Id, state);
        }

        var transitions = m_Mapper.Deserialize<List<AnimationStateMachine.AnimationTransition>>(bson["Transitions"]);
        foreach (var transition in transitions)
        {
            var sourceState = statesById[transition.SourceStateId];
            transition.SourceState = sourceState;
            sourceState.AddTransition(transition);

            transition.DestinationState = statesById[transition.DestinationStateId];
        }

        var animator = new AnimationStateMachine();
        animator.StartState = states[startStateId];
        animator.AnyState = states[anyStateId];
        return animator;
    }

    private static BsonDocument SerializeState(AnimationStateMachine.AnimationState state)
    {
        return new BsonDocument()
        {
            ["Id"] = state.Id,
            ["Animation"] = m_Mapper.Serialize(state.Animation.Name),
        };
    }

    private static AnimationStateMachine.AnimationState DeserializeState(BsonValue bson)
    {
        int id = bson["Id"];
        string animName = bson["Animation"];
        return new AnimationStateMachine.AnimationState(id, new Resource<IAnimation>(animName));
    }

    private static BsonDocument SerializeTransition(AnimationStateMachine.AnimationTransition transition)
    {
        return new BsonDocument()
        {
            ["Id"] = transition.Id,
            ["SourceState"] = transition.SourceState.Id,
            ["DestinationState"] = transition.DestinationState.Id,
            ["FloatConditions"] = m_Mapper.Serialize<List<FloatCondition>>(transition.m_FloatConditions),
            ["StringConditions"] = m_Mapper.Serialize<List<StringCondition>>(transition.m_StringConditions)
        };
    }

    private static AnimationStateMachine.AnimationTransition DeserializeTransition(BsonValue bson)
    {
        int id = bson["Id"];
        var transition = new AnimationStateMachine.AnimationTransition(id);

        transition.SourceStateId = bson["SourceState"];
        transition.DestinationStateId = bson["DestinationState"];
        transition.m_FloatConditions = m_Mapper.Deserialize<List<FloatCondition>>(bson["FloatConditions"]);
        transition.m_StringConditions = m_Mapper.Deserialize<List<StringCondition>>(bson["StringConditions"]);

        return transition;
    }

    private static BsonDocument SerializeFloatCondition(FloatCondition condition)
    {
        return new BsonDocument()
        {
            ["ParameterName"] = condition.ParameterName,
            ["Operator"] = condition.Operator.ToString(),
            ["ReferenceValue"] = condition.ReferenceValue
        };
    }

    private static FloatCondition DeserializeFloatCondition(BsonValue bson)
    {
        string paramName = bson["ParameterName"];
        Operator op = (Operator)bson["Operator"].AsInt32;
        float value = (float)bson["ReferenceValue"].AsDouble;

        return new FloatCondition(paramName, op, value);
    }

    private static BsonDocument SerializeStringCondition(StringCondition condition)
    {
        return new BsonDocument()
        {
            ["ParameterName"] = condition.ParameterName,
            ["Operator"] = condition.Operator.ToString(),
            ["ReferenceValue"] = condition.ReferenceValue
        };
    }

    private static StringCondition DeserializeStringCondition(BsonValue bson)
    {
        string paramName = bson["ParameterName"];
        var opObj = bson["Operator"];
        Operator op;
        if (opObj.IsString)
        {
            op = Enum.Parse<Operator>(opObj.AsString);
        }
        else
        {
            op = (Operator)opObj.AsInt32;
        }
        string value = bson["ReferenceValue"];

        return new StringCondition(paramName, op, value);
    }
}