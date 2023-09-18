namespace BananaEngine;

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public static class Debug
{
    private struct AssertInfo
    {
        public string Message;
        public string StackTrace;
        public string File;
        public int Line;
    }

    private static Queue<AssertInfo> m_AssertQueue = new Queue<AssertInfo>();
    private static SpriteFont m_Font;

    public static void Log(string message)
    {
        Console.WriteLine(message);
    }

    public static void LoadContent(ContentManager content)
    {
        m_Font = content.Load<SpriteFont>("font\\UbuntuMono-R");
    }

    public static void Assert(bool condition, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        if (!condition)
        {
            AssertInfo info = new AssertInfo();
            info.Message = message;
            info.File = Path.GetFileName(sourceFilePath);
            info.Line = sourceLineNumber;
            info.StackTrace = Environment.StackTrace;
            m_AssertQueue.Enqueue(info);
        }
    }
    
    public static void AssertFail(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        Assert(false, message, memberName, sourceFilePath, sourceLineNumber);
    }
    public static void FatalAssert(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception(message);
        }
    }

    public static bool HasAsserts()
    {
        return m_AssertQueue.Count > 0;
    }

    public static void Update()
    {
        if (!HasAsserts())
        {
            return;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.X))
        {
            m_AssertQueue.Dequeue();
        }
    }

    public static void DrawAssert(GraphicsDevice graphics, SpriteBatch spriteBatch)
    {
        if (!HasAsserts())
        {
            return;
        }

        graphics.Clear(Color.Black);
        int xOffset = 10; int yOffset = 20;
        if (m_AssertQueue.Count > 0)
        {
            AssertInfo info = m_AssertQueue.Peek();
            spriteBatch.DrawString(m_Font, "Assertion Failed:", new Microsoft.Xna.Framework.Vector2(xOffset, 1 * yOffset), Microsoft.Xna.Framework.Color.Red);
            spriteBatch.DrawString(m_Font, info.Message, new Microsoft.Xna.Framework.Vector2(xOffset, 2 * yOffset), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(m_Font, info.File + ":" + info.Line, new Microsoft.Xna.Framework.Vector2(xOffset, 3 * yOffset), Microsoft.Xna.Framework.Color.Green);
            spriteBatch.DrawString(m_Font, "Press X to continue", new Microsoft.Xna.Framework.Vector2(xOffset, 5 * yOffset), Microsoft.Xna.Framework.Color.CornflowerBlue);
            spriteBatch.DrawString(m_Font, info.StackTrace, new Microsoft.Xna.Framework.Vector2(xOffset, 6 * yOffset), Microsoft.Xna.Framework.Color.White);
        }
    }
}