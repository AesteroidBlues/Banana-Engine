namespace BananaEngine.Resources;

using System;
using System.Collections.Generic;
using System.Linq;

using BananaEngine.Db;

public abstract class BasicJsonResourceProvider<T> : ResourceProvider<T> where T : class
{
    private Dictionary<string, T> m_Resources = new Dictionary<string, T>();
    private Dictionary<string, Resource<T>> m_ResourceRefs = new Dictionary<string, Resource<T>>();

    [Dependency]
    protected DatabaseModule m_Database = null;

    string m_DatabaseTableName;

    protected BasicJsonResourceProvider(string dbTableName)
    {
        m_DatabaseTableName = dbTableName;
    }

    public override void Load()
    {
        m_ResourceRefs = m_Database.LoadAll<T>(m_DatabaseTableName).ToDictionary(r => r.Name, r => r);
        m_Resources = m_ResourceRefs.ToDictionary(r => r.Key, r => r.Value.Value);
    }

    public override void PostLoad()
    {
        foreach (var c in m_ResourceRefs)
        {
            c.Value.Hydrate(m_ResourceManager);
        }
    }

    public override bool Save(string name, T resource)
    {
        if (!m_Resources.ContainsKey(name))
        {
            m_Resources.Add(name, resource);
            m_ResourceRefs.Add(name, new Resource<T>(name, resource));
        }

        return m_Database.Save<T>(m_DatabaseTableName, new Resource<T>(name, resource));
    }

    public override bool Delete(string name)
    {
        if (!m_Resources.ContainsKey(name))
        {
            Console.WriteLine("INFORM: Nothing to delete: {0}", name);
            return true;
        }

        var resource = m_Resources[name];
        m_Resources.Remove(name);

        return m_Database.Delete<T>(m_DatabaseTableName, name);
    }

    public override T Get(string name)
    {
        if (!m_Resources.ContainsKey(name))
        {
            Console.Error.WriteLine($"BasicJsonResourceProvider.Get<T>(): Resource not found: {name}");
            return null;
        }

        return m_Resources[name];
    }

    public override IEnumerable<T> GetAll()
    {
        return m_Resources.Values;
    }

    public override Resource<T> GetRef(string name)
    {
        if (!m_ResourceRefs.ContainsKey(name))
        {
            Console.Error.WriteLine("BasicJsonResourceProvider.GetRef<T>(): Resource not found: {0}", name);
            return null;
        }

        return m_ResourceRefs[name];
    }

    public override IEnumerable<Resource<T>> GetAllRefs()
    {
        return m_ResourceRefs.Values;
    }

    public override Resource<T> GetInstanceRef(string name)
    {
        return m_Database.LoadByName<T>(m_DatabaseTableName, name);
    }
}