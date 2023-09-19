namespace BananaEngine.EntitySystem;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using BananaEngine;
using BananaEngine.Animation;

using Ninject;

public struct SimpleAnimationComponentArguments
{
    public SpriteSheetAnimation Animation { get; set; }
}

public class SimpleAnimationComponent : Component
{
    Resource<SpriteSheetAnimation> m_Animation;

    public SimpleAnimationComponent()
    {
    }

    ~SimpleAnimationComponent()
    {
    }

    public override void Start()
    {
    }

    public void Initialize(SimpleAnimationComponentArguments args)
    {
        m_Animation = new Resource<SpriteSheetAnimation>("", args.Animation);
    }

    public override void Update(float gameTime)
    {
        m_Animation.Value.Update(gameTime);
    }

    public Rectangle GetDrawRect()
    {
        return m_Animation.Value.GetCurrentFrame();
    }
}