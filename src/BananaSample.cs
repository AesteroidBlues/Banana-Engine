namespace BananaEngine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BananaEngine;

using ImGuiNET;

public class BananaSample : Game
{
    private GraphicsDeviceManager m_GraphicsManager;
    private SpriteBatch m_SpriteBatch;
    private ImGuiRenderer m_ImGuiRenderer;

    private DependencyManager m_DependencyManager;

    public BananaSample()
    {
        m_GraphicsManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "res";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        m_GraphicsManager.PreferredBackBufferWidth = 1280;
        m_GraphicsManager.PreferredBackBufferHeight = 720;

        m_DependencyManager = DependencyManager.Create();
        m_DependencyManager.AddDepedencyInstance<GraphicsDevice>(GraphicsDevice);
        m_DependencyManager.AddDepedencyInstance<GameWindow>(Window);
        m_DependencyManager.AddDepedencyInstance<ContentManager>(Content);

        m_ImGuiRenderer = new ImGuiRenderer(this.GraphicsDevice, this.Window);
        m_ImGuiRenderer.RebuildFontAtlas();

        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        m_SpriteBatch = new SpriteBatch(GraphicsDevice);

        Debug.LoadContent(Content);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Debug.Update();
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        m_SpriteBatch.Begin();
        if (Debug.HasAsserts())
        {
            Debug.DrawAssert(GraphicsDevice, m_SpriteBatch);
        }
        else
        {
            // TODO: Add your drawing code here
        }
        m_SpriteBatch.End();

        if (!Debug.HasAsserts())
        {
            m_ImGuiRenderer.BeforeLayout(gameTime);
            ImGui.ShowDemoWindow();
            m_ImGuiRenderer.AfterLayout();
        }

        base.Draw(gameTime);
    }
}
