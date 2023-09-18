namespace BananaEngine;

using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Xna.Framework.Graphics;

using BananaEngine.Animation;
using BananaEngine.Resources;
using BananaEngine.Util;

using Ninject;

public class ResourceManager : DependencyOwner
{
    private Dictionary<Type, IResourceProvider> m_ResourceProviders = new Dictionary<Type, IResourceProvider>();

    [Dependency]
    private Filesystem m_Filesystem = null;

    public ResourceManager()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var attr = type.GetCustomAttribute<ResourceProviderForAttribute>();
            if (attr != null)
            {
                var provider = Activator.CreateInstance(type) as IResourceProvider;
                m_ResourceProviders.Add(attr.ResourceType, provider);
            }
        }
    }

    public override void InitDependencies(DependencyManager dependencyManager)
    {
        base.InitDependencies(dependencyManager);
        foreach (var provider in m_ResourceProviders)
        {
            (provider.Value as DependencyOwner).InitDependencies(dependencyManager);
        }
    }

    public void AddResourceProvider<T>(ResourceProvider<T> provider) where T : class
    {
        m_ResourceProviders.Add(typeof(T), provider);
    }

    public void Load()
    {
        foreach (var provider in m_ResourceProviders.Values)
        {
            provider.Load();
        }

        foreach (var provider in m_ResourceProviders.Values)
        {
            provider.PostLoad();
        }
    }

    public T Get<T>(string name) where T : class
    {
        if (!m_ResourceProviders.ContainsKey(typeof(T)))
        {
            Console.Error.WriteLine($"No resource provider for type {typeof(T).Name}");
            return default(T);
        }

        return GetProvider<T>().Get(name);
    }

    public IEnumerable<T> GetAll<T>() where T : class
    {
        if (!m_ResourceProviders.ContainsKey(typeof(T)))
        {
            Console.Error.WriteLine($"No resource provider for type {typeof(T).Name}");
            return null;
        }

        return GetProvider<T>().GetAll();
    }

    public Resource<T> GetRef<T>(string name) where T : class
    {
        if (!m_ResourceProviders.ContainsKey(typeof(T)))
        {
            Console.Error.WriteLine($"No resource provider for type {typeof(T).Name}");
            return new Resource<T>(name, default(T));
        }

        return GetProvider<T>().GetRef(name);
    }

    public IEnumerable<Resource<T>> GetAllRefs<T>() where T : class
    {
        if (!m_ResourceProviders.ContainsKey(typeof(T)))
        {
            Console.Error.WriteLine($"No resource provider for type {typeof(T).Name}");
            return null;
        }

        return GetProvider<T>().GetAllRefs();
    }

    public Resource<T> GetInstanceRef<T>(string name) where T : class
    {
        if (!m_ResourceProviders.ContainsKey(typeof(T)))
        {
            Console.Error.WriteLine($"No resource provider for type {typeof(T).Name}");
            return null;
        }

        return GetProvider<T>().GetInstanceRef(name);
    }

    public bool Save<T>(string name, T resource) where T : class
    {
        if (!m_ResourceProviders.ContainsKey(typeof(T)))
        {
            Debug.Assert(false, $"No resource provider for type {typeof(T).Name}");
            return false;
        }

        return GetProvider<T>().Save(name, resource);
    }

    public bool Delete<T>(string name) where T : class
    {
        if (!m_ResourceProviders.ContainsKey(typeof(T)))
        {
            Debug.Assert(false, $"No resource provider for type {typeof(T).Name}");
            return false;
        }

        return GetProvider<T>().Delete(name);
    }

    public string GetResourceRootPath<T>()
    {
        switch (typeof(T).Name)
        {
            case (nameof(Texture2D)):
                return m_Filesystem.TextureDirectory;
            default:
                Console.Error.WriteLine($"No resource root path for type {typeof(T).Name}");
                return m_Filesystem.ContentDirectory;
        }
    }

    private IResourceProvider GetProvider(Type type)
    {
        if (!m_ResourceProviders.ContainsKey(type))
        {
            Console.Error.WriteLine($"No resource provider for type {type.Name}");
            return null;
        }

        return m_ResourceProviders[type];
    }

    private ResourceProvider<T> GetProvider<T>() where T : class
    {
        if (!m_ResourceProviders.ContainsKey(typeof(T)))
        {
            Console.Error.WriteLine($"No resource provider for type {typeof(T).Name}");
            return null;
        }

        return m_ResourceProviders[typeof(T)] as ResourceProvider<T>;
    }

    private static IEnumerable<Tuple<Type, Type>> GetTypesWithDebugMenuAttribute(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (type.GetCustomAttributes(typeof(ResourceProviderForAttribute), true).Length > 0)
            {
                var attr = type.GetCustomAttribute<ResourceProviderForAttribute>(true);
                yield return new Tuple<Type, Type>(type, attr.ResourceType);
            }
        }
    }
}