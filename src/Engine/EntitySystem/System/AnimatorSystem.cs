namespace BananaEngine.EntitySystem.Systems;

using System.Collections.Generic;

using BananaEngine;
using BananaEngine.Animation;

using Ninject;

public class AnimatorSystem : DependencyOwner
{
    private List<AnimatorComponent> m_AnimatorComponents;
    private Dictionary<string, Resource<AnimationStateMachine>> m_Animators;

    public AnimatorSystem()
    {
        m_AnimatorComponents = new List<AnimatorComponent>();
        m_Animators = new Dictionary<string, Resource<AnimationStateMachine>>();
    }

    public override void InitDependencies(DependencyManager dependencyManager)
    {
        base.InitDependencies(dependencyManager);
        
    }

    public void Update(float gameTime)
    {
    }
}