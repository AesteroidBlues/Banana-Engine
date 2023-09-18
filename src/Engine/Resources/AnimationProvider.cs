namespace BananaEngine.Resources;

using System;
using System.Collections.Generic;
using System.IO;

using BananaEngine.Animation;
using BananaEngine.Db;
using BananaEngine.Resources;

using BananaEngine.Util;

[ResourceProviderFor(typeof(IAnimation))]
class AnimationProvider : BasicJsonResourceProvider<IAnimation>
{
    public AnimationProvider()
        : base(Enum.GetName(DbTable.Animation))
    {
    }

    // private Dictionary<string, IAnimation> m_Animations = new Dictionary<string, IAnimation>();
    // private Dictionary<string, Resource<IAnimation>> m_AnimationRefs = new Dictionary<string, Resource<IAnimation>>();

    // [Dependency]
    // private DatabaseModule m_Database = null;

    // [Dependency]
    // private Filesystem m_Filesystem = null;

    // public AnimationProvider()
    // {
    // }

    // public override void Load()
    // {
    //     foreach (var animationName in m_Filesystem.GetFilesInDirectory(m_Filesystem.AnimationDirectory))
    //     {
    //         using (StreamReader reader = new StreamReader(Path.Combine(m_Filesystem.AnimationDirectory, animationName)))
    //         {
    //             string json = reader.ReadToEnd();
    //             var animation = JsonConvert.DeserializeObject<IAnimation>(json, new JsonSerializerSettings()
    //             {
    //                 TypeNameHandling = TypeNameHandling.All
    //             });

    //             m_Animations.Add(animationName, animation);
    //             m_AnimationRefs.Add(animationName, new Resource<IAnimation>(animationName, animation));
    //         }
    //     }
    // }

    // public override void PostLoad()
    // {
    // }

    // public override IAnimation Get(string name)
    // {
    //     if (!m_Animations.ContainsKey(name))
    //     {
    //         Console.Error.WriteLine("AnimationProvider.Get<T>(): Animation not found: {0}", name);
    //         return new EmptyAnimation();
    //     }

    //     return m_Animations[name];
    // }

    // public override Resource<IAnimation> GetRef(string name)
    // {
    //     if (!m_Animations.ContainsKey(name))
    //     {
    //         Console.Error.WriteLine("AnimationProvider.Get<T>(): Animation not found: {0}", name);
    //         return new Resource<IAnimation>(name, new EmptyAnimation());
    //     }

    //     return m_AnimationRefs[name];
    // }

    // public override IEnumerable<IAnimation> GetAll()
    // {
    //     return m_Animations.Values;
    // }

    // public override IEnumerable<Resource<IAnimation>> GetAllRefs()
    // {
    //     return m_AnimationRefs.Values;
    // }

    // public override bool Save(string name, IAnimation resource)
    // {
    //     return m_Database.Save(DbTable.Animation, new Resource<IAnimation>(name, resource));
    // }
}