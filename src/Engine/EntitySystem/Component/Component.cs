namespace BananaEngine.EntitySystem;

using Microsoft.Xna.Framework;

using Ninject;

public class Component : DependencyOwner
{
    protected Entity m_Entity;
    private bool m_Enabled = true;

    public Component()
    {
    }

    public void SetEnabled(bool enabled)
    {
        m_Enabled = enabled;
    }

    public Entity GetEntity()
    {
        return m_Entity;
    }

    public void SetEntity(Entity entity)
    {
        m_Entity = entity;
    }

    protected bool IsEnabled()
    {
        return m_Enabled;
    }

    public virtual void Start() {}

    public virtual void Update(float gameTime) {}
}