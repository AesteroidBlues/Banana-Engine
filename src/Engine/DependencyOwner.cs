namespace BananaEngine;

using System;

using Ninject;

public interface IDepedencyOwner
{
    public bool HasInitializedDependencies { get; }
    public void InitDependencies(StandardKernel dependencyManager);
}

public class DependencyAttribute : Attribute
{
    public DependencyAttribute()
    {
    }
}

public class DependencyOwner
{
    public bool HasInitializedDependencies { get; private set; } = false;

    public DependencyOwner()
    {
    }

    public virtual void InitDependencies(DependencyManager dependencyManager)
    {
        var fields = GetType().GetFields(
            System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.FlattenHierarchy
            );

        foreach (var field in fields)
        {
            // Fill out dependencies
            var attr = field.GetCustomAttributes(typeof(DependencyAttribute), true);
            if (attr.Length > 0)
            {
                var dep = dependencyManager.Get(field.FieldType);
                field.SetValue(this, dep);
            }

            // Check for nested dependency owners and init their dependencies
            if (field.GetType().IsAssignableFrom(typeof(DependencyOwner)))
            {
                var val = field.GetValue(null);
                if (val is DependencyOwner depOwner && !depOwner.HasInitializedDependencies)
                {
                    (val as DependencyOwner).InitDependencies(dependencyManager);
                }
            }
        }

        HasInitializedDependencies = true;
    }
}