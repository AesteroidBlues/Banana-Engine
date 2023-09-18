namespace BananaEngine.EntitySystem;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Ninject;

public class Entity : DependencyOwner
{
    public Scene Scene { get; private set; }

    public String Name { get;  set; }

    public Entity Parent;
    public TransformComponent Transform { get; private set; }

    public List<Entity> Children = new List<Entity>();
    public List<Component> Components = new List<Component>();

    #region OldEntity
    // [Dependency]
    // public ResourceManager ResourceManager;

    public Entity(Scene scene)
        : this(scene, "New Entity")
    {
    }

    public Entity(Scene scene, string name)
    {
        Name = name;
        Scene = scene;

        Transform = AddComponent<TransformComponent>();
    }

    override public void InitDependencies(DependencyManager dependencyManager)
    {
        base.InitDependencies(dependencyManager);
        foreach (var component in Components)
        {
            component.InitDependencies(dependencyManager);
        }

        foreach (var component in Components)
        {
            component.InitDependencies(dependencyManager);
            component.Start();
        }
    }

    public void Update(float gameTime)
    {
        foreach (var component in Components)
        {
            component.Update(gameTime);
        }

        foreach (var child in Children)
        {
            child.Update(gameTime);
        }
    }

    public void AddChild(Entity child)
    {
        bool add = true;
        var p = this;
        while (p != null)
        {
            if (p == child)
            {
                Debug.Assert(false, "Illegal loop in entity hierarchy");
                add = false;
                break;
            }
            p = p.Parent;
        }

        if (!add)
        {
            return;
        }

        Children.Add(child);
        child.Parent = this;
    }

    public T AddComponent<T>() where T : Component, new()
    {
        var component = new T();
        component.SetEntity(this);
        Components.Add(component);

        return component;
    }

    public void AddComponents(List<Component> components)
    {
        foreach (var component in components)
        {
            component.SetEntity(this);
            Components.Add(component);
        }
    }

    public T GetComponent<T>() where T : class
    {
        foreach (var component in Components)
        {
            if (component is T)
            {
                return component as T;
            }
        }

        return default(T);
    }
    #endregion
}