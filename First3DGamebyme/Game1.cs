using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using First3DGamebyme.Engine.Camera;
using First3DGamebyme.Engine.Input;
using First3DGamebyme.Entities;
using System.Collections.Generic;

namespace First3DGamebyme;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private Camera2D _camera;
    private InputManager _inputManager;
    private Player _player;
    private List<Enemy> _enemies;
    
    private RenderTarget2D _gameRenderTarget;
    private const int GAME_WIDTH = 1920;
    private const int GAME_HEIGHT = 1080;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        _camera = new Camera2D(GraphicsDevice.Viewport);
        _inputManager = new InputManager();
        _player = new Player(new Vector2(GAME_WIDTH / 2, GAME_HEIGHT / 2));
        _enemies = new List<Enemy>();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _gameRenderTarget = new RenderTarget2D(
            GraphicsDevice,
            GAME_WIDTH,
            GAME_HEIGHT,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.None
        );
        
        _player.LoadContent(GraphicsDevice);
        
        for (int i = 0; i < 5; i++)
        {
            var enemy = new BasicEnemy(new Vector2(
                GAME_WIDTH / 2 + (i - 2) * 200,
                GAME_HEIGHT / 2 + 300
            ));
            enemy.LoadContent(GraphicsDevice);
            enemy.SetTarget(_player);
            _enemies.Add(enemy);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        _inputManager.Update();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
            _inputManager.IsKeyPressed(Keys.Escape))
            Exit();
        
        var worldMouse = _camera.ScreenToWorld(_inputManager.MousePosition);
        _player.HandleInput(_inputManager, gameTime, worldMouse);
        _player.Update(gameTime);
        
        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime);
            
            if (_player.CurrentAttack.IsActive && 
                !_player.CurrentAttack.HitTargets.Contains(enemy) &&
                _player.CurrentAttack.IsInHitArea(enemy.Position))
            {
                enemy.TakeDamage(_player.CurrentAttack.Damage);
                _player.CurrentAttack.HitTargets.Add(enemy);
            }
        }
        
        _enemies.RemoveAll(e => !e.IsActive);
        
        _camera.Follow(_player.Position, 0.1f);
        
        if (_inputManager.IsKeyHeld(Keys.Q))
            _camera.ZoomIn(0.02f);
        if (_inputManager.IsKeyHeld(Keys.E))
            _camera.ZoomOut(0.02f);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_gameRenderTarget);
        GraphicsDevice.Clear(new Color(20, 20, 30));
        
        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            null,
            null,
            null,
            _camera.GetTransformMatrix()
        );
        
        DrawGrid();
        
        foreach (var enemy in _enemies)
        {
            enemy.Draw(_spriteBatch);
        }
        
        _player.Draw(_spriteBatch);
        
        _spriteBatch.End();
        
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        
        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
        
        float scale = System.Math.Min(
            (float)GraphicsDevice.Viewport.Width / GAME_WIDTH,
            (float)GraphicsDevice.Viewport.Height / GAME_HEIGHT
        );
        
        Vector2 position = new Vector2(
            (GraphicsDevice.Viewport.Width - GAME_WIDTH * scale) / 2,
            (GraphicsDevice.Viewport.Height - GAME_HEIGHT * scale) / 2
        );
        
        _spriteBatch.Draw(
            _gameRenderTarget,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
    
    private void DrawGrid()
    {
        Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        
        Color gridColor = new Color(40, 40, 50, 100);
        int gridSize = 64;
        
        for (int x = 0; x < GAME_WIDTH * 2; x += gridSize)
        {
            _spriteBatch.Draw(pixel, new Rectangle(x - GAME_WIDTH/2, -GAME_HEIGHT/2, 1, GAME_HEIGHT * 2), gridColor);
        }
        
        for (int y = 0; y < GAME_HEIGHT * 2; y += gridSize)
        {
            _spriteBatch.Draw(pixel, new Rectangle(-GAME_WIDTH/2, y - GAME_HEIGHT/2, GAME_WIDTH * 2, 1), gridColor);
        }
        
        pixel.Dispose();
    }
}
