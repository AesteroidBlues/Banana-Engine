namespace BananaEngine;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Ninject;

public interface IRenderable
{
    void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}

public class RenderManager
{
    private List<IRenderable> m_Drawables = new List<IRenderable>();

    public RenderManager()
    {
    }

    public void RegisterDrawable(IRenderable drawable)
    {
        m_Drawables.Add(drawable);
    }

    public IEnumerable<IRenderable> GetDrawables()
    {
        return m_Drawables;
    }
}

public class BaseDrawable : DependencyOwner, IRenderable
{
    [Dependency]
    protected RenderManager m_GameLoopManager;

    public BaseDrawable() { }

    public override void InitDependencies(DependencyManager dependencyManager)
    {
        base.InitDependencies(dependencyManager);
        m_GameLoopManager.RegisterDrawable(this);
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
    }
}