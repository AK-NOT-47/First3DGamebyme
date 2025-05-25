using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace First3DGamebyme.UI;

public class GameOverScreen
{
    private bool _isActive;
    private float _fadeTimer;
    private float _fadeDuration = 1f;
    private float _textPulseTimer;
    private KeyboardState _previousKeyState;
    
    public bool IsActive => _isActive;
    public bool ShouldRestart { get; private set; }
    
    public void Show()
    {
        _isActive = true;
        _fadeTimer = 0f;
        _textPulseTimer = 0f;
        ShouldRestart = false;
    }
    
    public void Hide()
    {
        _isActive = false;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_isActive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_fadeTimer < _fadeDuration)
            _fadeTimer += deltaTime;
        
        _textPulseTimer += deltaTime;
        
        KeyboardState keyState = Keyboard.GetState();
        
        if (_fadeTimer >= _fadeDuration)
        {
            if (keyState.IsKeyDown(Keys.R) && !_previousKeyState.IsKeyDown(Keys.R))
            {
                ShouldRestart = true;
            }
            else if (keyState.IsKeyDown(Keys.Escape) && !_previousKeyState.IsKeyDown(Keys.Escape))
            {
                Environment.Exit(0);
            }
        }
        
        _previousKeyState = keyState;
    }
    
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, int screenWidth, int screenHeight)
    {
        if (!_isActive) return;
        
        Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        
        // Draw darkened background
        float fadeAlpha = Math.Min(_fadeTimer / _fadeDuration, 1f);
        Color bgColor = new Color(0, 0, 0, (int)(200 * fadeAlpha));
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), bgColor);
        
        if (_fadeTimer >= _fadeDuration * 0.5f)
        {
            // Draw "GAME OVER" text
            string gameOverText = "GAME OVER";
            Vector2 gameOverPos = new Vector2(screenWidth / 2 - gameOverText.Length * 20, screenHeight / 2 - 100);
            DrawLargeText(spriteBatch, pixel, gameOverText, gameOverPos, Color.Red, 5f);
            
            if (_fadeTimer >= _fadeDuration)
            {
                // Draw restart prompt with pulsing effect
                float pulse = (float)Math.Sin(_textPulseTimer * 3) * 0.3f + 0.7f;
                Color promptColor = new Color((int)(255 * pulse), (int)(255 * pulse), (int)(255 * pulse));
                
                string restartText = "PRESS R TO RESTART";
                Vector2 restartPos = new Vector2(screenWidth / 2 - restartText.Length * 10, screenHeight / 2 + 50);
                DrawLargeText(spriteBatch, pixel, restartText, restartPos, promptColor, 2f);
                
                string exitText = "PRESS ESC TO EXIT";
                Vector2 exitPos = new Vector2(screenWidth / 2 - exitText.Length * 10, screenHeight / 2 + 100);
                DrawLargeText(spriteBatch, pixel, exitText, exitPos, promptColor, 2f);
            }
        }
        
        pixel.Dispose();
    }
    
    private void DrawLargeText(SpriteBatch spriteBatch, Texture2D pixel, string text, Vector2 position, Color color, float scale)
    {
        float charWidth = 8f * scale;
        float charHeight = 10f * scale;
        float spacing = 2f * scale;
        
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            Vector2 charPos = position + new Vector2(i * (charWidth + spacing), 0);
            
            if (c == ' ')
                continue;
            
            DrawCharacter(spriteBatch, pixel, c, charPos, color, scale);
        }
    }
    
    private void DrawCharacter(SpriteBatch spriteBatch, Texture2D pixel, char c, Vector2 position, Color color, float scale)
    {
        // Simple block letters
        bool[,] charMap = GetCharacterMap(c);
        
        if (charMap != null)
        {
            float pixelSize = scale;
            for (int y = 0; y < charMap.GetLength(0); y++)
            {
                for (int x = 0; x < charMap.GetLength(1); x++)
                {
                    if (charMap[y, x])
                    {
                        Vector2 pixelPos = position + new Vector2(x * pixelSize, y * pixelSize);
                        spriteBatch.Draw(pixel, pixelPos, null, color, 0f, Vector2.Zero, pixelSize, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
    
    private bool[,] GetCharacterMap(char c)
    {
        switch (c)
        {
            case 'A': return new bool[,] {
                {false,true,true,true,false},
                {true,false,false,false,true},
                {true,true,true,true,true},
                {true,false,false,false,true},
                {true,false,false,false,true}
            };
            case 'E': return new bool[,] {
                {true,true,true,true,true},
                {true,false,false,false,false},
                {true,true,true,true,false},
                {true,false,false,false,false},
                {true,true,true,true,true}
            };
            case 'G': return new bool[,] {
                {false,true,true,true,true},
                {true,false,false,false,false},
                {true,false,true,true,true},
                {true,false,false,false,true},
                {false,true,true,true,true}
            };
            case 'M': return new bool[,] {
                {true,false,false,false,true},
                {true,true,false,true,true},
                {true,false,true,false,true},
                {true,false,false,false,true},
                {true,false,false,false,true}
            };
            case 'O': return new bool[,] {
                {false,true,true,true,false},
                {true,false,false,false,true},
                {true,false,false,false,true},
                {true,false,false,false,true},
                {false,true,true,true,false}
            };
            case 'P': return new bool[,] {
                {true,true,true,true,false},
                {true,false,false,false,true},
                {true,true,true,true,false},
                {true,false,false,false,false},
                {true,false,false,false,false}
            };
            case 'R': return new bool[,] {
                {true,true,true,true,false},
                {true,false,false,false,true},
                {true,true,true,true,false},
                {true,false,false,true,false},
                {true,false,false,false,true}
            };
            case 'S': return new bool[,] {
                {false,true,true,true,true},
                {true,false,false,false,false},
                {false,true,true,true,false},
                {false,false,false,false,true},
                {true,true,true,true,false}
            };
            case 'T': return new bool[,] {
                {true,true,true,true,true},
                {false,false,true,false,false},
                {false,false,true,false,false},
                {false,false,true,false,false},
                {false,false,true,false,false}
            };
            case 'V': return new bool[,] {
                {true,false,false,false,true},
                {true,false,false,false,true},
                {true,false,false,false,true},
                {false,true,false,true,false},
                {false,false,true,false,false}
            };
            case 'X': return new bool[,] {
                {true,false,false,false,true},
                {false,true,false,true,false},
                {false,false,true,false,false},
                {false,true,false,true,false},
                {true,false,false,false,true}
            };
            case 'I': return new bool[,] {
                {false,true,true,true,false},
                {false,false,true,false,false},
                {false,false,true,false,false},
                {false,false,true,false,false},
                {false,true,true,true,false}
            };
            case 'C': return new bool[,] {
                {false,true,true,true,true},
                {true,false,false,false,false},
                {true,false,false,false,false},
                {true,false,false,false,false},
                {false,true,true,true,true}
            };
            default: return null;
        }
    }
}