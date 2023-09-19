namespace BananaEngine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BananaEngine;

using ImGuiNET;
using BananaEngine.EntitySystem;

public class BananaSample : Game
{
    private GraphicsDeviceManager m_GraphicsManager;
    private SpriteBatch m_SpriteBatch;
    private ImGuiRenderer m_ImGuiRenderer;

    private DependencyManager m_DependencyManager;

    Scene m_Scene;
    Entity m_BananaGuy;

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
        m_DependencyManager.Get<ResourceManager>().InitDependencies(m_DependencyManager);

        m_ImGuiRenderer = new ImGuiRenderer(this.GraphicsDevice, this.Window);
        m_ImGuiRenderer.RebuildFontAtlas();

        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        m_SpriteBatch = new SpriteBatch(GraphicsDevice);

        Debug.LoadContent(Content);
        m_DependencyManager.Get<ResourceManager>().Load();

        // TODO: use this.Content to load your game content here

        // Set up the scene and add a banana guy
        // TODO: This API needs cleanup
        m_Scene = new Scene(m_DependencyManager);
        m_BananaGuy = m_Scene.CreateEntity("BananaGuy", new Vector3(100, 100, 0));
        DrawableComponent comp = m_BananaGuy.AddComponent<DrawableComponent>();
        comp.InitDependencies(m_DependencyManager);

        SimpleAnimationComponent simpleAnimationComponent = m_BananaGuy.AddComponent<SimpleAnimationComponent>();
        SpriteSheetAnimation animation = new SpriteSheetAnimation("banana_anim", 333, 333, 10, -1);
        for (int i = 0; i < 8; i++)
        {
            animation.AddFrame();
            animation.SetFrameAt(i, i % 3, i / 3);
        }
        SimpleAnimationComponentArguments args2 = new SimpleAnimationComponentArguments();
        args2.Animation = animation;
        simpleAnimationComponent.Initialize(args2);

        DrawableComponentArguments args = new DrawableComponentArguments();
        args.TextureName = "pbjtime.png";
        comp.Initialize(args);


        m_Scene.Start();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        Debug.Update();
        m_Scene.Update(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
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
            m_Scene.Draw(gameTime, m_SpriteBatch);
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
