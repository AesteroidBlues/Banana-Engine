namespace BananaEngine.EntitySystem;

using Microsoft.Xna.Framework;

public class TransformComponent : Component
{
    public Vector3 m_Position = new Vector3(0, 0, 0);
    public Vector3 m_Rotation = new Vector3(0, 0, 0);
    public Vector3 m_Scale = new Vector3(1, 1, 1);

    public TransformComponent()
    {
    }
}