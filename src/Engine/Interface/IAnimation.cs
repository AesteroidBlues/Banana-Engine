namespace BananaEngine;

using Microsoft.Xna.Framework;

public interface IAnimation
{
    public void Update(float gameTime);
    public Rectangle GetCurrentFrame();
    public void Reset();
}