namespace BananaEngine.EntitySystem;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using BananaEngine;
using BananaEngine.Animation;

using Ninject;

public struct DrawableComponentArguments
{
    public string TextureName { get; set; }
}

public class DrawableComponent : Component, IRenderable
{
    Resource<Texture2D> m_Texture;

    [Dependency]
    private ResourceManager m_ResourceManager = null;

    public DrawableComponent()
    {
    }

    ~DrawableComponent()
    {
        Debug.FatalAssert(m_Entity != null, "DrawableComponent: Entity removed before component destructor");
        Debug.FatalAssert(m_Entity.Scene != null, "DrawableComponent: Entity scene destroyed before component destructor");

        m_Entity.Scene.RemoveFromDrawList(this);
    }

    public override void Start()
    {
        m_Entity.Scene.AddToDrawList(this);
    }

    public void Initialize(DrawableComponentArguments args)
    {
        m_Texture = m_ResourceManager.GetRef<Texture2D>(args.TextureName);
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (m_Texture.Value == null)
        {
            return;
        }

        Rectangle rectangle = new Rectangle(0, 0, m_Texture.Value.Width, m_Texture.Value.Height);
        if (m_Entity.GetComponent<AnimatorComponent>() is AnimatorComponent animator)
        {
            rectangle = animator.GetDrawRect();
        }

        Vector2 position = new Vector2(m_Entity.Transform.m_Position.X, m_Entity.Transform.m_Position.Y);
        spriteBatch.Draw(m_Texture.Value, position, rectangle, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
    }
}