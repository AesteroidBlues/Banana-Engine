namespace BananaEngine;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Microsoft.Xna.Framework;

using BananaEngine.Util;

public class SpriteSheetAnimation : IAnimation
{
    public static int MAX_FRAME_COUNT = 100;

    public int SpriteWidth { get; set; }
    public int SpriteHeight { get; set; }
    public int AnimationSpeed { get; set; }

    public int LoopRestartFrame { get; set; }

    public List<Vector2i> Frames { get; set; }

    private int m_CurrentFrame;
    private double m_CurrentFrameTime;

    public SpriteSheetAnimation()
        : this("New Animation", 32, 64, 60, 0)
    { }

    public SpriteSheetAnimation(string name, int spriteWidth, int spriteHeight, int animationSpeed, int loopRestartFrame)
    {
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
        AnimationSpeed = animationSpeed;

        LoopRestartFrame = loopRestartFrame;

        Frames = new List<Vector2i>(1);
    }

    ~SpriteSheetAnimation()
    {
        Frames = null;
    }

    public void Update(float gameTime)
    {
        m_CurrentFrameTime += gameTime * 1000;
        if (m_CurrentFrameTime > (1 / (float)this.AnimationSpeed) * 1000)
        {
            m_CurrentFrame = (m_CurrentFrame + 1) % Frames.Count;
            m_CurrentFrameTime = 0;
        }
    }

    public Rectangle GetCurrentFrame()
    {
        return new Rectangle(
            Frames[m_CurrentFrame].X * SpriteWidth,
            Frames[m_CurrentFrame].Y * SpriteHeight,
            SpriteWidth,
            SpriteHeight
        );
    }

    public void Reset()
    {
        m_CurrentFrame = 0;
        m_CurrentFrameTime = 0;
    }

    public void AddFrame()
    {
        Frames.Add(Vector2i.Zero);
    }

    public void SetFrameAt(int idx, int x, int y)
    {
        if (idx < 0)
        {
            throw new ArgumentOutOfRangeException("idx", "Index must be >= 0");
        }

        Frames[idx].X = x;
        Frames[idx].Y = y;
    }

    public Vector2i GetFrameAt(int i)
    {
        if (i < 0 || i >= Frames.Count)
        {
            return Vector2i.Zero;
        }

        return new Vector2i(Frames[i].X, Frames[i].Y);
    }

    public void RemoveLastFrame()
    {
        if (Frames.Count == 0)
        {
            return;
        }

        Frames.RemoveAt(Frames.Count - 1);
    }
}