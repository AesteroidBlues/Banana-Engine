namespace BananaEngine.Resources;

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BananaEngine.Util;
using BananaEngine;

using Newtonsoft.Json;

[ResourceProviderFor(typeof(Texture2D))]
public class Texture2DProvider : ResourceProvider<Texture2D>
{
    private Dictionary<string, Texture2D> m_Textures = new Dictionary<string, Texture2D>();
    private Dictionary<string, Resource<Texture2D>> m_TextureRefs = new Dictionary<string, Resource<Texture2D>>();

    private Texture2D m_DefaultTexture;

    [Dependency]
    private Filesystem m_Filesystem = null;

    [Dependency]
    private GraphicsDevice m_GraphicsDevice = null;

    public Texture2DProvider()
    {
    }

    public override void Load()
    {
        m_DefaultTexture = new Texture2D(m_GraphicsDevice, 1, 1);
        m_DefaultTexture.SetData<Color>(new Color[] { Color.Magenta });

        var textureFiles = m_Filesystem.GetFilesInDirectory(m_Filesystem.TextureDirectory);
        foreach (var textureName in textureFiles)
        {
            String texturePath = Path.Combine(m_Filesystem.TextureDirectory, textureName);
            Texture2D texture = Texture2D.FromFile(m_GraphicsDevice, texturePath);
            
            m_Textures.Add(textureName, texture);
            m_TextureRefs.Add(textureName, new Resource<Texture2D>(textureName, texture));
        }
    }

    public override void PostLoad()
    {
    }

    public override Texture2D Get(string name)
    {
        if (!m_Textures.ContainsKey(name))
        {
            Console.Error.WriteLine("TextureProvider.Get<T>(): Texture not found: {0}", name);
            return m_DefaultTexture;
        }

        return m_Textures[name];
    }

    public override Resource<Texture2D> GetRef(string name)
    {
        return new Resource<Texture2D>(name, Get(name));
    }

    public override IEnumerable<Texture2D> GetAll()
    {
        return m_Textures.Values;
    }

    public override IEnumerable<Resource<Texture2D>> GetAllRefs()
    {
        return m_TextureRefs.Values;
    }

    public override Resource<Texture2D> GetInstanceRef(string name)
    {
        throw new NotImplementedException();
    }

    public override bool Save(string name, Texture2D resource)
    {
        return false;
    }
}
