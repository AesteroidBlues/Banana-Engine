namespace BananaEngine;

using System;

using Ninject;
using Ninject.Modules;

public class DependencyManager
{
    private StandardKernel m_Container;

    public static DependencyManager Create()
    {
        return new DependencyManager();
    }

    public DependencyManager()
    {
        EngineDependencies engineDependencies = new EngineDependencies();
        m_Container = new StandardKernel(engineDependencies);
    }

    public Object Get(Type t)
    {
        return m_Container.Get(t);
    }

    public T Get<T>()
    {
        return m_Container.Get<T>();
    }

    public void AddDepedencyInstance<T>(object instance)
    {
        if (instance == null)
        {
            Debug.AssertFail($"DependencyManager: Cannot add null instance as dependency of type {typeof(T)}");
            return;
        }
            
        Debug.Assert(instance.GetType().IsAssignableTo(typeof(T)),
            $"DependencyManager: Cannot add instance of type {instance.GetType()} as dependency of type {typeof(T)}");

        m_Container.Bind(typeof(T)).ToConstant(instance);
    }

    public void AddDependencySet(NinjectModule module)
    {
        m_Container.Load(module);
    }

}