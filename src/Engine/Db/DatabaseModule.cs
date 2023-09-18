namespace BananaEngine.Db;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using BananaEngine.Serialization;
using BananaEngine.Util;

using LiteDB;

using Newtonsoft.Json;
using Ninject;

public enum DbTable
{
    Animation,
    AnimationStateMachine
}

public class DatabaseModule
{
    public class DatabaseMetaInfo
    {
        public int Version { get; set; }
    }

#if DEBUG
    string m_DatabaseName = "data_dev.db";
#else
    string m_DatabaseName = "data.db";
#endif

    string m_DbPath;

    private bool m_BackupToJson = true;

    Filesystem m_Filesystem;

    public DatabaseModule(Filesystem fs)
    {
        m_Filesystem = fs;
        m_DbPath = Path.Combine(fs.ContentDirectory, m_DatabaseName);
    }

    public void Initialize()
    {
        if (!File.Exists(m_DbPath))
        {
            Console.Error.WriteLine($"Database {m_DatabaseName} not found, creating...");
            using (var db = new LiteDatabase(m_DbPath))
            {
                var meta = db.GetCollection<DatabaseMetaInfo>("Meta");

                meta.Insert(new DatabaseMetaInfo()
                {
                    Version = 1
                });

                meta.EnsureIndex(x => x.Version);
            }
        }
        else
        {
            using (var db = new LiteDatabase(m_DbPath))
            {
                var meta = db.GetCollection<DatabaseMetaInfo>("Meta");
                var info = meta.FindOne(x => x.Version == 1);
                Debug.Assert(info != null, "Database version not found");
            }
        }

        foreach (var type in GetTypesWithBsonMapperForAttribute(Assembly.GetExecutingAssembly()))
        {
            // Better way to do this??
            type.GetMethod("Register").Invoke(null, new object[] { BsonMapper.Global });
        }

        BsonMapper.Global.EnumAsInteger = true;
    }

    public bool Save<T>(DbTable table, Resource<T> data) where T : class
    {
        return Save<T>(Enum.GetName(table), data);
    }

    public bool Save<T>(string tableName, Resource<T> data) where T : class
    {
        bool ret = false;

        try
        {
            using (var db = new LiteDatabase(m_DbPath))
            {
                var col = db.GetCollection<Resource<T>>(tableName);
                col.EnsureIndex(x => x.Name);

                if (col.FindById(data.Name) != null)
                {
                    col.Update(data);
                }
                else
                {
                    col.Insert(data);
                }

                db.Commit();
            }
            ret = true;
        }
        catch (Exception e)
        {
            Debug.Assert(false, $"Failed to save {data.Name} to table {tableName}: {e.Message}");
            ret = false;
        }

        if (m_BackupToJson)
        {
            ret &= SaveToJson(tableName, data);
        }

        return ret;
    }

    public IEnumerable<Resource<T>> LoadAll<T>(DbTable table) where T : class
    {
        return LoadAll<T>(Enum.GetName(table));
    }

    public IEnumerable<Resource<T>> LoadAll<T>(string tableName) where T : class
    {
        var ret = new List<Resource<T>>();
        using (var db = new LiteDatabase(m_DbPath))
        {
            var col = db.GetCollection<Resource<T>>(tableName);
            ret = col.FindAll().ToList();
        }

        return ret;
    }

    public Resource<T> LoadByName<T>(DbTable table, string resourceName) where T : class
    {
        return LoadByName<T>(Enum.GetName(table), resourceName);
    }

    public Resource<T> LoadByName<T>(string tableName, string resourceName) where T : class
    {
        var ret = new Resource<T>(resourceName);
        using (var db = new LiteDatabase(m_DbPath))
        {
            var col = db.GetCollection<Resource<T>>(tableName);
            ret = col.FindById(resourceName);
        }

        return ret;
    }

    public bool Delete<T>(DbTable table, string name) where T : class
    {
        return Delete<T>(Enum.GetName(table), name);
    }

    public bool Delete<T>(string tableName, string name) where T : class
    {
        bool ret = false;

        try
        {
            using (var db = new LiteDatabase(m_DbPath))
            {
                var col = db.GetCollection<Resource<T>>(tableName);
                col.Delete(name);
                db.Commit();
            }
            ret = true;
        }
        catch (Exception e)
        {
            Debug.Assert(false, $"Failed to delete {name} from table {tableName}: {e.Message}");
            ret = false;
        }

        if (m_BackupToJson)
        {
            ret &= DeleteFromJson(tableName, name);
        }

        return ret;
    }

    private bool SaveToJson<T>(string tableName, Resource<T> data) where T : class
    {
        string path = Path.Combine(m_Filesystem.ContentDirectory, "db", tableName) + Path.DirectorySeparatorChar;
        string file = Path.Combine(path, data.Name + ".json");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        try
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter { NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy() });

                string json = JsonConvert.SerializeObject(
                    data.Value,
                    Formatting.Indented,
                    settings
                );
                writer.Write(json);
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Assert(false, $"Failed to save {data.Name} to {file}: {e.Message}");
            return false;
        }
    }

    private bool DeleteFromJson(string tableName, string name)
    {
        string path = Path.Combine(m_Filesystem.ContentDirectory, "db", tableName) + Path.DirectorySeparatorChar;
        string file = Path.Combine(path, name + ".json");
        if (!Directory.Exists(path))
        {
            return true;
        }

        try
        {
            File.Delete(file);
            return true;
        }
        catch (Exception e)
        {
            Debug.Assert(false, $"Failed to delete {name} from {file}: {e.Message}");
            return false;
        }
    }

#if DEBUG
    public List<String> DebugGetAllTables()
    {
        var ret = new List<String>();
        using (var db = new LiteDatabase(m_DbPath))
        {
            foreach (var col in db.GetCollectionNames())
            {
                ret.Add(col);
            }
        }

        return ret;
    }

    public List<String> DebugGetAllResourcesInTable(string tableName)
    {
        var ret = new List<String>();
        using (var db = new LiteDatabase(m_DbPath))
        {
            var col = db.GetCollection<Resource<object>>(tableName);
            foreach (var res in col.FindAll())
            {
                ret.Add(res.Name);
            }
        }

        return ret;
    }
#endif

    static IEnumerable<Type> GetTypesWithBsonMapperForAttribute(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (type.GetCustomAttributes(typeof(BsonMapperForAttribute), true).Length > 0)
            {
                yield return type;
            }
        }
    }
}