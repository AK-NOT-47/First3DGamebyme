using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using First3DGamebyme.Engine.Camera;
using First3DGamebyme.Engine.Input;
using First3DGamebyme.Entities;
using First3DGamebyme.Systems.Combat;
using First3DGamebyme.UI;
using System.Collections.Generic;
using System;

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
    
    private Random _random = new Random();
    private GameOverScreen _gameOverScreen;

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
        _gameOverScreen = new GameOverScreen();

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
        
        if (_gameOverScreen.IsActive)
        {
            _gameOverScreen.Update(gameTime);
            
            if (_gameOverScreen.ShouldRestart)
            {
                RestartGame();
            }
            return;
        }
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
            _inputManager.IsKeyPressed(Keys.Escape))
            Exit();
        
        var worldMouse = _camera.ScreenToWorld(_inputManager.MousePosition);
        
        if (_player.IsActive)
        {
            _player.HandleInput(_inputManager, gameTime, worldMouse);
            _player.Update(gameTime);
        }
        else if (!_gameOverScreen.IsActive)
        {
            _gameOverScreen.Show();
        }
        
        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime);
            
            if (_player.CurrentAttack.IsActive && 
                !_player.CurrentAttack.HitTargets.Contains(enemy) &&
                _player.CurrentAttack.IsInHitArea(enemy.Position))
            {
                enemy.TakeDamage(
                    _player.CurrentAttack.Damage, 
                    _player.Position, 
                    _player.CurrentAttack.KnockbackForce,
                    _player.CurrentAttack.HitStunDuration,
                    _player.CurrentAttack.IsCritical
                );
                _player.CurrentAttack.HitTargets.Add(enemy);
                
                _player.CombatSystem.TriggerHitPause(0.05f);
                _player.CombatSystem.TriggerScreenShake(3f, 0.15f);
                _player.CombatSystem.SpawnDamageNumber(enemy.Position, _player.CurrentAttack.Damage, _player.CurrentAttack.IsCritical);
                
                Vector2 hitDirection = enemy.Position - _player.Position;
                hitDirection.Normalize();
                _player.CombatSystem.SpawnHitParticles(enemy.Position, hitDirection, 5);
            }
        }
        
        _enemies.RemoveAll(e => !e.IsActive);
        
        Vector2 shakeOffset = Vector2.Zero;
        if (_player.CombatSystem.ScreenShakeAmount > 0)
        {
            shakeOffset = new Vector2(
                (float)(_random.NextDouble() - 0.5) * _player.CombatSystem.ScreenShakeAmount,
                (float)(_random.NextDouble() - 0.5) * _player.CombatSystem.ScreenShakeAmount
            );
        }
        
        _camera.Follow(_player.Position + shakeOffset, 0.1f);
        
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
        
        DrawCombatEffects();
        
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
        
        DrawUI();
        
        _gameOverScreen.Draw(_spriteBatch, GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        
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
    
    private void DrawCombatEffects()
    {
        if (_player.CombatSystem == null) return;
        
        foreach (var particle in _player.CombatSystem.ActiveParticles)
        {
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            _spriteBatch.Draw(
                pixel,
                particle.Position,
                null,
                particle.GetColor(),
                0f,
                new Vector2(0.5f, 0.5f),
                3f,
                SpriteEffects.None,
                0f
            );
            
            pixel.Dispose();
        }
        
        foreach (var dmgNumber in _player.CombatSystem.ActiveDamageNumbers)
        {
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            int digitCount = (int)Math.Log10(dmgNumber.Damage) + 1;
            float digitWidth = 10f;
            float totalWidth = digitCount * digitWidth;
            
            for (int i = 0; i < digitCount; i++)
            {
                int digit = (int)(dmgNumber.Damage / Math.Pow(10, digitCount - i - 1)) % 10;
                Vector2 digitPos = dmgNumber.Position + new Vector2(i * digitWidth - totalWidth / 2, 0);
                
                DrawDigit(_spriteBatch, pixel, digit, digitPos, dmgNumber.GetColor(), dmgNumber.GetScale());
            }
            
            pixel.Dispose();
        }
    }
    
    private void DrawDigit(SpriteBatch spriteBatch, Texture2D pixel, int digit, Vector2 position, Color color, float scale)
    {
        int[][] digitPatterns = new int[][]
        {
            new[] {1,1,1, 1,0,1, 1,0,1, 1,0,1, 1,1,1}, // 0
            new[] {0,1,0, 1,1,0, 0,1,0, 0,1,0, 1,1,1}, // 1
            new[] {1,1,1, 0,0,1, 1,1,1, 1,0,0, 1,1,1}, // 2
            new[] {1,1,1, 0,0,1, 1,1,1, 0,0,1, 1,1,1}, // 3
            new[] {1,0,1, 1,0,1, 1,1,1, 0,0,1, 0,0,1}, // 4
            new[] {1,1,1, 1,0,0, 1,1,1, 0,0,1, 1,1,1}, // 5
            new[] {1,1,1, 1,0,0, 1,1,1, 1,0,1, 1,1,1}, // 6
            new[] {1,1,1, 0,0,1, 0,0,1, 0,0,1, 0,0,1}, // 7
            new[] {1,1,1, 1,0,1, 1,1,1, 1,0,1, 1,1,1}, // 8
            new[] {1,1,1, 1,0,1, 1,1,1, 0,0,1, 1,1,1}  // 9
        };
        
        int[] pattern = digitPatterns[digit];
        float pixelSize = 2f * scale;
        
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == 1)
            {
                int x = i % 3;
                int y = i / 3;
                Vector2 pixelPos = position + new Vector2(x * pixelSize - pixelSize, y * pixelSize - pixelSize * 2.5f);
                
                spriteBatch.Draw(pixel, pixelPos, null, color, 0f, Vector2.Zero, pixelSize, SpriteEffects.None, 0f);
            }
        }
    }
    
    private void DrawUI()
    {
        Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        
        if (_player.CombatSystem != null && _player.CombatSystem.IsInCombo)
        {
            int comboCount = _player.CombatSystem.CurrentComboIndex + 1;
            string comboText = $"COMBO x{comboCount}";
            
            Vector2 comboPos = new Vector2(50, 50);
            float scale = 3f;
            
            DrawText(_spriteBatch, pixel, comboText, comboPos, Color.Gold, scale);
        }
        
        DrawHealthBar(pixel, new Vector2(50, GraphicsDevice.Viewport.Height - 100));
        
        pixel.Dispose();
    }
    
    private void DrawText(SpriteBatch spriteBatch, Texture2D pixel, string text, Vector2 position, Color color, float scale)
    {
        float charWidth = 6f * scale;
        float charHeight = 8f * scale;
        
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            Vector2 charPos = position + new Vector2(i * charWidth, 0);
            
            if (c == ' ')
                continue;
            
            if (char.IsDigit(c))
            {
                DrawDigit(spriteBatch, pixel, c - '0', charPos, color, scale);
            }
            else
            {
                for (int y = 0; y < 5; y++)
                {
                    spriteBatch.Draw(pixel, charPos + new Vector2(0, y * scale), null, color, 0f, Vector2.Zero, new Vector2(charWidth * 0.8f, scale), SpriteEffects.None, 0f);
                }
            }
        }
    }
    
    private void DrawHealthBar(Texture2D pixel, Vector2 position)
    {
        int barWidth = 300;
        int barHeight = 20;
        
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X - 2, (int)position.Y - 2, barWidth + 4, barHeight + 4), Color.Black);
        
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, barWidth, barHeight), Color.DarkRed);
        
        float healthPercentage = _player.Health / _player.MaxHealth;
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, (int)(barWidth * healthPercentage), barHeight), Color.Red);
        
        string healthText = $"{_player.Health:0} / {_player.MaxHealth:0}";
        DrawText(_spriteBatch, pixel, healthText, position + new Vector2(barWidth / 2 - healthText.Length * 3, -25), Color.White, 2f);
    }
    
    private void RestartGame()
    {
        _gameOverScreen.Hide();
        
        // Reset player
        _player = new Player(new Vector2(GAME_WIDTH / 2, GAME_HEIGHT / 2));
        _player.LoadContent(GraphicsDevice);
        
        // Reset enemies
        _enemies.Clear();
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
        
        // Reset camera
        _camera = new Camera2D(GraphicsDevice.Viewport);
    }
}
