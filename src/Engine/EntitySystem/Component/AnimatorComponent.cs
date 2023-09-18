namespace BananaEngine.EntitySystem;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BananaEngine;
using BananaEngine.Animation;

public struct AnimatorComponentArguments
{
    public string AnimatorName { get; set; }
    public string SimpleAnimationName { get; set; }
}

public class AnimatorComponent : Component
{
    Resource<AnimationStateMachine> m_Animator;

    AnimationStateMachine.AnimationState m_CurrentState;
    Dictionary<string, Object> m_Parameters;

    Dictionary<string, Resource<IAnimation>> m_AnimationCache;
    Resource<IAnimation> m_CurrentAnimation;

    [Dependency]
    private ResourceManager m_ResourceManager = null;

    public AnimatorComponent()
    {
        m_Parameters = new Dictionary<string, Object>();
        m_AnimationCache = new Dictionary<string, Resource<IAnimation>>();
    }

    public void Initialize(AnimatorComponentArguments args)
    {
        if (args.AnimatorName != null)
        {
            Debug.Assert(args.SimpleAnimationName == null, "AnimatorComponent: Cannot specify both AnimatorName and SimpleAnimationName");
            m_Animator = m_ResourceManager.GetRef<AnimationStateMachine>(args.AnimatorName);
        }
        else
        {
            m_CurrentAnimation = m_ResourceManager.GetRef<IAnimation>(args.SimpleAnimationName);
        }
    }

    public override void Update(float gameTime)
    {
        if (m_Animator != null)
        {
            UpdateAnimationStateMachine(gameTime);
        }

        m_CurrentAnimation.Value.Update(gameTime);
    }

    private void UpdateAnimationStateMachine(float gameTime)
    {
        if (m_CurrentState == null)
        {
            m_CurrentState = m_Animator.Value.StartState;
        }

        // Check if we should transition to a new state from our current state
        foreach (var transition in m_CurrentState.Transitions)
        {
            if (transition.ShouldTransition(m_Parameters))
            {
                m_CurrentState = transition.DestinationState ?? m_Animator.Value.StartState;
                break;
            }
        }

        // Check if we should transition to a new state from the any state
        foreach (var transition in m_Animator.Value.AnyState.Transitions)
        {
            if (transition.ShouldTransition(m_Parameters))
            {
                m_CurrentState = transition.DestinationState ?? m_Animator.Value.StartState;
                break;
            }
        }

        string animName = m_CurrentState.Animation.Name;
        if (m_AnimationCache.ContainsKey(animName))
        {
            m_CurrentAnimation = m_AnimationCache[animName];
        }
        else
        {
            m_CurrentAnimation = m_ResourceManager.GetInstanceRef<IAnimation>(animName);
            m_AnimationCache.Add(animName, m_CurrentAnimation);
        }
    }

    public Rectangle GetDrawRect()
    {
        if (!m_CurrentAnimation.IsValid)
        {
            return new Rectangle(0, 0, 100, 100);
        }

        return m_CurrentAnimation.Value.GetCurrentFrame();
    }

    public void SetAnimatorVariable<T>(string name, T value)
    {
        if (!m_Parameters.ContainsKey(name))
        {
            m_Parameters.Add(name, value);
        }
        else
        {
            m_Parameters[name] = value;
        }
    }
}