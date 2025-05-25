using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace First3DGamebyme.Entities;

public abstract class Entity
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
    public Rectangle Bounds { get; protected set; }
    public bool IsActive { get; set; }
    public Color TintColor { get; set; }
    
    protected Texture2D _texture;
    protected Vector2 _origin;
    
    public Entity(Vector2 position)
    {
        Position = position;
        Velocity = Vector2.Zero;
        Rotation = 0f;
        Scale = Vector2.One;
        IsActive = true;
        TintColor = Color.White;
    }
    
    public virtual void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        UpdateBounds();
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive || _texture == null) return;
        
        spriteBatch.Draw(
            _texture,
            Position,
            null,
            TintColor,
            Rotation,
            _origin,
            Scale,
            SpriteEffects.None,
            0f
        );
    }
    
    protected virtual void UpdateBounds()
    {
        if (_texture != null)
        {
            Bounds = new Rectangle(
                (int)(Position.X - _origin.X * Scale.X),
                (int)(Position.Y - _origin.Y * Scale.Y),
                (int)(_texture.Width * Scale.X),
                (int)(_texture.Height * Scale.Y)
            );
        }
    }
    
    public bool CollidesWith(Entity other)
    {
        return IsActive && other.IsActive && Bounds.Intersects(other.Bounds);
    }
    
    public bool CollidesWith(Rectangle rect)
    {
        return IsActive && Bounds.Intersects(rect);
    }
}