namespace BananaEngine.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using BananaEngine.Animation;
using BananaEngine.Db;

using BananaEngine.Util;

using Newtonsoft.Json;

[ResourceProviderFor(typeof(AnimationStateMachine))]
class AnimationStateMachineProvider : ResourceProvider<AnimationStateMachine>
{
    private Dictionary<string, AnimationStateMachine> m_Animators = new Dictionary<string, AnimationStateMachine>();

    [Dependency]
    private DatabaseModule m_Database = null;

    public AnimationStateMachineProvider()
    {
    }

    public override void Load()
    {
        foreach (var animator in m_Database.LoadAll<AnimationStateMachine>(DbTable.AnimationStateMachine))
        {
            m_Animators.Add(animator.Name, animator.Value);
        }
    }

    public override void PostLoad()
    {
        foreach (var animator in m_Animators.Values)
        {
            RehydrateRefs(animator);
        }
    }

    private void RehydrateRefs(AnimationStateMachine animator)
    {
        foreach (var node in GraphIterator.Bfs(new List<IGraphNode> { animator.StartState, animator.AnyState }))
        {
            var state = node as AnimationStateMachine.AnimationState;
            state.Animation.Hydrate(m_ResourceManager);
        }
    }

    public override AnimationStateMachine Get(string name)
    {
        if (!m_Animators.ContainsKey(name))
        {
            Console.Error.WriteLine("AnimationProvider.Get<T>(): Animator not found: {0}", name);
            return new AnimationStateMachine();
        }

        return m_Animators[name];
    }

    public override Resource<AnimationStateMachine> GetRef(string name)
    {
        if (!m_Animators.ContainsKey(name))
        {
            Console.Error.WriteLine("AnimationProvider.Get<T>(): Animator not found: {0}", name);
            return new Resource<AnimationStateMachine>(name, new AnimationStateMachine());
        }

        return new Resource<AnimationStateMachine>(name, m_Animators[name]);
    }

    public override IEnumerable<AnimationStateMachine> GetAll()
    {
        return m_Animators.Values;
    }

    public override IEnumerable<Resource<AnimationStateMachine>> GetAllRefs()
    {
        throw new NotImplementedException();
    }

    public override Resource<AnimationStateMachine> GetInstanceRef(string name)
    {
        throw new NotImplementedException();
    }

    public override bool Save(string name, AnimationStateMachine resource)
    {
        return m_Database.Save<AnimationStateMachine>(DbTable.AnimationStateMachine, new Resource<AnimationStateMachine>(name, resource));
    }
}