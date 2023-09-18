namespace BananaEngine.Animation;

using Microsoft.Xna.Framework;

public class EmptyAnimation : IAnimation
{
    public string Name { get { return "Empty Animation"; } }

    public void Update(float gameTime)
    {
    }

    public Rectangle GetCurrentFrame()
    {
        return Rectangle.Empty;
    }

    public void Reset()
    {
    }
}