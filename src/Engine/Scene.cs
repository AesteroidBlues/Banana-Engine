namespace BananaEngine;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BananaEngine.EntitySystem;

using Ninject;

public class Scene : BaseDrawable, IUpdatable
{
    DependencyManager m_DependencyManager;
    
    public List<Entity> m_Children = new List<Entity>();
    private List<IRenderable> m_DrawList = new List<IRenderable>();

    public Scene(DependencyManager dependencyManager)
    {
        m_DependencyManager = dependencyManager;
        InitDependencies(m_DependencyManager);
    }

    public void Start()
    {
        Queue<Entity> queue = new Queue<Entity>();
        foreach (var entity in m_Children)
        {
            queue.Enqueue(entity);
        }

        while (queue.Count > 0)
        {
            var entity = queue.Dequeue();
            entity.InitDependencies(m_DependencyManager);
            foreach (var child in entity.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    public void Update(float gameTime)
    {
        foreach (var entity in m_Children)
        {
            entity.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var component in m_DrawList)
        {
            component.Draw(gameTime, spriteBatch);
        }
    }

    public Entity CreateEntity()
    {
        return CreateEntity("New Entity", Vector3.Zero);
    }

    public Entity CreateEntity(string name, Vector3 position)
    {
        Entity ent = new Entity(this, name);
        m_Children.Add(ent);
        ent.Transform.m_Position = position;
        return ent;
    }

    public void AddToDrawList(IRenderable component)
    {
        m_DrawList.Add(component);
    }

    public void RemoveFromDrawList(IRenderable component)
    {
        m_DrawList.Remove(component);
    }
}