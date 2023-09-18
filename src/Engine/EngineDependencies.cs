namespace BananaEngine;

using System;
using System.Collections.Generic;

using BananaEngine.Animation;
using BananaEngine.Db;
using BananaEngine.Util;

using Ninject.Modules;

public class EngineDependencies : NinjectModule
{
    public EngineDependencies()
    {
    }

    public override void Load()
    {
        Bind<Filesystem>().ToSelf().InSingletonScope();
        Bind<ResourceManager>().ToSelf().InSingletonScope();
        Bind<DatabaseModule>().ToSelf().InSingletonScope();
        Bind<RenderManager>().ToSelf().InSingletonScope();
    }
}