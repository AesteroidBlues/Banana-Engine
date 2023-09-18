namespace BananaEngine.Resources;

using System;
using System.Collections.Generic;

public interface IResourceProvider
{
    void Load();
    void PostLoad();
}

public class ResourceProviderForAttribute : Attribute
{
    public Type ResourceType { get; private set; }

    public ResourceProviderForAttribute(Type resourceType)
    {
        ResourceType = resourceType;
    }
}

public abstract class BasicResourceProvider : DependencyOwner, IResourceProvider
{
    [Dependency]
    protected ResourceManager m_ResourceManager = null;

    public abstract void Load();
    public abstract void PostLoad();

    public abstract T Get<T>(string name) where T : class;
    public abstract Resource<T> GetRef<T>(string name) where T : class;
    public abstract IEnumerable<T> GetAll<T>() where T : class;
    public abstract IEnumerable<Resource<T>> GetAllRefs<T>() where T : class;

    public abstract bool Save<T>(string name, T resource);
}

public abstract class ResourceProvider<T> : DependencyOwner, IResourceProvider where T : class
{
    [Dependency]
    protected ResourceManager m_ResourceManager = null;

    public abstract void Load();
    public abstract void PostLoad();

    public abstract T Get(string name);
    public abstract Resource<T> GetRef(string name);
    public abstract IEnumerable<T> GetAll();
    public abstract IEnumerable<Resource<T>> GetAllRefs();
    public abstract Resource<T> GetInstanceRef(string name);

    public abstract bool Save(string name, T resource);
    public virtual bool Delete(string name)
    {
        Debug.Assert(false, $"Delete operation not supported on type {typeof(T).Name}");
        return false;
    }
}