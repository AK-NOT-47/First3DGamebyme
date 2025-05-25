using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace First3DGamebyme.Engine.Input;

public class InputManager
{
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;
    private MouseState _currentMouseState;
    private MouseState _previousMouseState;
    private GamePadState _currentGamePadState;
    private GamePadState _previousGamePadState;
    
    public Vector2 MousePosition => _currentMouseState.Position.ToVector2();
    public Vector2 MouseDelta => MousePosition - _previousMouseState.Position.ToVector2();
    
    public void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _previousMouseState = _currentMouseState;
        _previousGamePadState = _currentGamePadState;
        
        _currentKeyboardState = Keyboard.GetState();
        _currentMouseState = Mouse.GetState();
        _currentGamePadState = GamePad.GetState(PlayerIndex.One);
    }
    
    public bool IsKeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
    }
    
    public bool IsKeyHeld(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key);
    }
    
    public bool IsKeyReleased(Keys key)
    {
        return !_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
    }
    
    public bool IsMouseButtonPressed(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return _currentMouseState.LeftButton == ButtonState.Pressed && 
                       _previousMouseState.LeftButton == ButtonState.Released;
            case MouseButton.Right:
                return _currentMouseState.RightButton == ButtonState.Pressed && 
                       _previousMouseState.RightButton == ButtonState.Released;
            case MouseButton.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Pressed && 
                       _previousMouseState.MiddleButton == ButtonState.Released;
            default:
                return false;
        }
    }
    
    public bool IsMouseButtonHeld(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return _currentMouseState.LeftButton == ButtonState.Pressed;
            case MouseButton.Right:
                return _currentMouseState.RightButton == ButtonState.Pressed;
            case MouseButton.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Pressed;
            default:
                return false;
        }
    }
    
    public Vector2 GetMovementVector()
    {
        Vector2 movement = Vector2.Zero;
        
        if (IsKeyHeld(Keys.W) || IsKeyHeld(Keys.Up))
            movement.Y -= 1;
        if (IsKeyHeld(Keys.S) || IsKeyHeld(Keys.Down))
            movement.Y += 1;
        if (IsKeyHeld(Keys.A) || IsKeyHeld(Keys.Left))
            movement.X -= 1;
        if (IsKeyHeld(Keys.D) || IsKeyHeld(Keys.Right))
            movement.X += 1;
            
        if (_currentGamePadState.IsConnected)
        {
            movement += _currentGamePadState.ThumbSticks.Left * new Vector2(1, -1);
        }
        
        if (movement.LengthSquared() > 1)
            movement.Normalize();
            
        return movement;
    }
}

public enum MouseButton
{
    Left,
    Right,
    Middle
}