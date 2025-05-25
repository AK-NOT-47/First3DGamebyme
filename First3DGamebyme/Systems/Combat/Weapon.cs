using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace First3DGamebyme.Systems.Combat;

public enum WeaponType
{
    Sword,
    Axe,
    Hammer,
    Dagger,
    Staff
}

public class Weapon
{
    public string Name { get; set; }
    public WeaponType Type { get; set; }
    public float Damage { get; set; }
    public float AttackSpeed { get; set; }
    public float Range { get; set; }
    public float KnockbackForce { get; set; }
    public float SwingAngle { get; set; }
    
    public Weapon(string name, WeaponType type, float damage, float attackSpeed, float range)
    {
        Name = name;
        Type = type;
        Damage = damage;
        AttackSpeed = attackSpeed;
        Range = range;
        KnockbackForce = 150f;
        SwingAngle = MathHelper.PiOver2;
    }
}

public class Attack
{
    public Vector2 Position { get; set; }
    public float Direction { get; set; }
    public float Range { get; set; }
    public float Damage { get; set; }
    public float SwingAngle { get; set; }
    public float Duration { get; set; }
    public float Timer { get; set; }
    public bool IsActive { get; set; }
    public HashSet<object> HitTargets { get; private set; }
    
    public Attack()
    {
        HitTargets = new HashSet<object>();
    }
    
    public void Start(Vector2 position, float direction, Weapon weapon)
    {
        Position = position;
        Direction = direction;
        Range = weapon.Range;
        Damage = weapon.Damage;
        SwingAngle = weapon.SwingAngle;
        Duration = 1f / weapon.AttackSpeed;
        Timer = Duration;
        IsActive = true;
        HitTargets.Clear();
    }
    
    public void Update(float deltaTime, Vector2 position)
    {
        if (!IsActive) return;
        
        Position = position;
        Timer -= deltaTime;
        
        if (Timer <= 0)
        {
            IsActive = false;
        }
    }
    
    public bool IsInHitArea(Vector2 targetPosition)
    {
        if (!IsActive) return false;
        
        Vector2 toTarget = targetPosition - Position;
        float distance = toTarget.Length();
        
        if (distance > Range) return false;
        
        float targetAngle = (float)System.Math.Atan2(toTarget.Y, toTarget.X);
        float angleDiff = MathHelper.WrapAngle(targetAngle - Direction);
        
        return System.Math.Abs(angleDiff) <= SwingAngle / 2f;
    }
    
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!IsActive) return;
        
        float progress = 1f - (Timer / Duration);
        float currentAngle = Direction - SwingAngle / 2f + SwingAngle * progress;
        
        Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        
        int segments = 20;
        for (int i = 0; i < segments; i++)
        {
            float angle = Direction - SwingAngle / 2f + (SwingAngle * i / segments);
            if (System.Math.Abs(angle - currentAngle) < SwingAngle / segments)
            {
                Vector2 endPoint = Position + new Vector2(
                    (float)System.Math.Cos(angle) * Range,
                    (float)System.Math.Sin(angle) * Range
                );
                
                DrawLine(spriteBatch, pixel, Position, endPoint, new Color(255, 255, 255, 100), 3f);
            }
        }
        
        pixel.Dispose();
    }
    
    private void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness)
    {
        Vector2 edge = end - start;
        float angle = (float)System.Math.Atan2(edge.Y, edge.X);
        
        spriteBatch.Draw(pixel,
            new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), (int)thickness),
            null,
            color,
            angle,
            new Vector2(0, 0.5f),
            SpriteEffects.None,
            0);
    }
}