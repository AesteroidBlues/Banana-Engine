namespace BananaEngine.Util;

using System;
using System.Collections.Generic;

public abstract class BlackboardObject
{
    Dictionary<string, Object> m_Parameters = new Dictionary<string, Object>();

    public bool HasParameter<T>(string name)
    {
        if (m_Parameters.ContainsKey(name))
        {
            var obj = m_Parameters[name];
            if ((T)obj != null)
            {
                return true;
            }

            Console.Error.WriteLine($"Parameter {name} exists but is the wrong type. Requested: {typeof(T)}, actual: {obj.GetType()}");
        }

        return false;
    }

    public bool TryGetParameter<T>(string name, out T value)
    {
        if (m_Parameters.ContainsKey(name))
        {
            if ((T)m_Parameters[name] != null)
            {
                value = (T)m_Parameters[name];
                return true;
            }
        }

        value = default(T);
        return false;
    }

    public T GetParameter<T>(string name)
    {
        if (m_Parameters.ContainsKey(name))
        {
            if ((T)m_Parameters[name] != null)
            {
                return (T)m_Parameters[name];
            }
            else
            {
                throw new Exception($"Parameter {name} cannot be cast to {typeof(T)}");
            }
        }
        else
        {
            throw new Exception($"Parameter {name} does not exist on {this.ToString()}");
        }
    }

    public void SetParameter<T>(string name, T value)
    {
        if (!m_Parameters.ContainsKey(name))
        {
            m_Parameters.Add(name, value);
        }
        else
        {
            m_Parameters[name] = value;
        }
    }

    public void AddToParameterList<T>(string name, T value)
    {
        var listName = $"LIST_{name}";
        if (!m_Parameters.ContainsKey(name))
        {
            m_Parameters.Add(listName, new List<T>());
        }

        if (m_Parameters[listName] is List<T>)
        {
            (m_Parameters[listName] as List<T>).Add(value);
        }
        else
        {
            throw new Exception($"Parameter {name} is not a list of type {typeof(T)}");
        }
    }

    public List<T> GetParameterList<T>(string name)
    {
        var listName = $"LIST_{name}";
        if (m_Parameters.ContainsKey(listName))
        {
            if (m_Parameters[listName] is List<T>)
            {
                return m_Parameters[listName] as List<T>;
            }
            else
            {
                throw new Exception($"Parameter {name} is not a list of type {typeof(T)}");
            }
        }
        else
        {
            return new List<T>();
        }
    }

    public void SetParameterList<T>(string name, List<T> value)
    {
        var listName = $"LIST_{name}";
        if (!m_Parameters.ContainsKey(listName))
        {
            m_Parameters.Add(listName, value);
        }
        else
        {
            m_Parameters[listName] = value;
        }
    }
}