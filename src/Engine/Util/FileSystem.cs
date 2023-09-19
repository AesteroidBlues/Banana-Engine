namespace BananaEngine.Util;

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework.Content;

using Ninject;

public class Filesystem
{
    private String m_ContentRootDirectory;
    public String ContentDirectory
    {
        get { return m_ContentRootDirectory; }
    }

    public String TextureDirectory
    {
        get { return Path.Combine(m_ContentRootDirectory, "texture"); }
    }

    [Inject]
    public Filesystem(ContentManager contentManager)
    {
        DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        m_ContentRootDirectory = GetFullPathOfDirectory(contentManager.RootDirectory, currentDirectory.FullName);
    }

    public List<String> GetFilesInDirectory(String directory)
    {
        var files = new List<String>();

        var directories = new DirectoryInfo(directory).GetDirectories();

        var filesInDirectory = new DirectoryInfo(directory).GetFiles();
        foreach (var file in filesInDirectory)
        {
            files.Add(file.Name);
        }

        return files;
    }

    public List<String> GetFileNamesInDirectory(String directory)
    {
        var files = new List<String>();

        var directories = new DirectoryInfo(directory).GetDirectories();

        var filesInDirectory = new DirectoryInfo(directory).GetFiles();
        foreach (var file in filesInDirectory)
        {
            files.Add(file.Name.Substring(0, file.Name.LastIndexOf('.')));
        }

        return files;
    }

    private static string GetFullPathOfDirectory(string directoryName, string startingDirectory)
    {
        string foundDirectoryPath = "";
        bool foundRootContent = false;

        DirectoryInfo currentDirectory = new DirectoryInfo(startingDirectory);
        while (!foundRootContent)
        {
            if (currentDirectory == null)
            {
                Console.Error.WriteLine($"Could not find directory {directoryName} in {startingDirectory} or any of its parents");
                foundRootContent = true;
            }

            var directoriesAtThisLevel = Directory.GetDirectories(currentDirectory.FullName);
            foreach (var fullDirectoryPath in directoriesAtThisLevel)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(fullDirectoryPath);
                if (dirInfo.Name == directoryName)
                {
                    foundDirectoryPath = fullDirectoryPath;
                    foundRootContent = true;
                    break;
                }
            }

            currentDirectory = currentDirectory.Parent;
        }

        return foundDirectoryPath;
    }
}