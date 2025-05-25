using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace First3DGamebyme.Entities;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Hurt,
    Dead
}

public abstract class Enemy : Entity
{
    protected EnemyState _state;
    protected Entity _target;
    protected float _health;
    protected float _maxHealth;
    protected float _moveSpeed;
    protected float _attackRange;
    protected float _detectionRange;
    protected float _attackCooldown;
    protected float _attackTimer;
    protected float _damage;
    protected Random _random;
    protected float _hitStunTimer;
    protected float _knockbackTimer;
    protected Vector2 _knockbackVelocity;
    
    public float Health => _health;
    public float MaxHealth => _maxHealth;
    public EnemyState State => _state;
    public bool IsStunned => _hitStunTimer > 0;
    
    public Enemy(Vector2 position, float health, float moveSpeed) : base(position)
    {
        _health = health;
        _maxHealth = health;
        _moveSpeed = moveSpeed;
        _state = EnemyState.Idle;
        _random = new Random();
        _attackRange = 50f;
        _detectionRange = 300f;
        _attackCooldown = 1f;
        _damage = 10f;
    }
    
    public virtual void SetTarget(Entity target)
    {
        _target = target;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_attackTimer > 0)
            _attackTimer -= deltaTime;
        
        if (_hitStunTimer > 0)
        {
            _hitStunTimer -= deltaTime;
            if (_hitStunTimer <= 0)
                _state = EnemyState.Idle;
        }
        
        if (_knockbackTimer > 0)
        {
            _knockbackTimer -= deltaTime;
            _knockbackVelocity *= 0.9f;
            if (_knockbackTimer <= 0)
            {
                _knockbackVelocity = Vector2.Zero;
            }
        }
        
        if (!IsStunned)
        {
            UpdateState(deltaTime);
            UpdateBehavior(deltaTime);
        }
        else
        {
            Velocity = _knockbackVelocity;
        }
        
        base.Update(gameTime);
    }
    
    protected virtual void UpdateState(float deltaTime)
    {
        switch (_state)
        {
            case EnemyState.Idle:
                if (_target != null && _target.IsActive)
                {
                    float distanceToTarget = Vector2.Distance(Position, _target.Position);
                    if (distanceToTarget <= _detectionRange)
                    {
                        _state = EnemyState.Chase;
                    }
                }
                break;
                
            case EnemyState.Chase:
                if (_target == null || !_target.IsActive)
                {
                    _state = EnemyState.Idle;
                }
                else
                {
                    float distanceToTarget = Vector2.Distance(Position, _target.Position);
                    if (distanceToTarget <= _attackRange)
                    {
                        _state = EnemyState.Attack;
                    }
                    else if (distanceToTarget > _detectionRange * 1.5f)
                    {
                        _state = EnemyState.Idle;
                    }
                }
                break;
                
            case EnemyState.Attack:
                if (_target == null || !_target.IsActive)
                {
                    _state = EnemyState.Idle;
                }
                else
                {
                    float distanceToTarget = Vector2.Distance(Position, _target.Position);
                    if (distanceToTarget > _attackRange * 1.2f)
                    {
                        _state = EnemyState.Chase;
                    }
                }
                break;
                
            case EnemyState.Hurt:
                break;
                
            case EnemyState.Dead:
                IsActive = false;
                break;
        }
    }
    
    protected virtual void UpdateBehavior(float deltaTime)
    {
        switch (_state)
        {
            case EnemyState.Idle:
                Velocity = Vector2.Zero;
                break;
                
            case EnemyState.Chase:
                if (_target != null)
                {
                    Vector2 direction = _target.Position - Position;
                    if (direction.LengthSquared() > 0)
                    {
                        direction.Normalize();
                        Velocity = direction * _moveSpeed;
                        Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
                    }
                }
                break;
                
            case EnemyState.Attack:
                Velocity = Vector2.Zero;
                if (_attackTimer <= 0 && _target != null)
                {
                    PerformAttack();
                    _attackTimer = _attackCooldown;
                }
                break;
                
            case EnemyState.Hurt:
                Velocity = _knockbackVelocity;
                break;
        }
    }
    
    protected virtual void PerformAttack()
    {
        if (_target is Player player)
        {
            player.TakeDamage(_damage, Position);
        }
    }
    
    public virtual void TakeDamage(float damage, Vector2 attackPosition, float knockbackForce, float hitStunDuration, bool isCritical)
    {
        if (_state == EnemyState.Dead) return;
        
        _health -= damage;
        _state = EnemyState.Hurt;
        _hitStunTimer = hitStunDuration;
        
        Vector2 knockbackDirection = Position - attackPosition;
        if (knockbackDirection.LengthSquared() > 0)
            knockbackDirection.Normalize();
        else
            knockbackDirection = new Vector2(_random.Next(-1, 2), _random.Next(-1, 2));
        
        _knockbackVelocity = knockbackDirection * knockbackForce;
        _knockbackTimer = 0.3f;
        
        if (_health <= 0)
        {
            _health = 0;
            _state = EnemyState.Dead;
            _knockbackVelocity *= 2f;
        }
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        Color tint = Color.White;
        if (_state == EnemyState.Hurt && _hitStunTimer > 0)
        {
            float flash = (float)Math.Sin(_hitStunTimer * 20) * 0.5f + 0.5f;
            tint = Color.Lerp(Color.White, Color.Red, flash);
        }
        
        spriteBatch.Draw(_texture, Position, null, tint, Rotation, _origin, 1f, SpriteEffects.None, 0f);
        
        if (IsActive && _health < _maxHealth)
        {
            DrawHealthBar(spriteBatch);
        }
    }
    
    protected virtual void DrawHealthBar(SpriteBatch spriteBatch)
    {
        int barWidth = 40;
        int barHeight = 4;
        Vector2 barPosition = Position - new Vector2(barWidth / 2, 30);
        
        Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        
        spriteBatch.Draw(pixel, new Rectangle((int)barPosition.X - 1, (int)barPosition.Y - 1, barWidth + 2, barHeight + 2), Color.Black);
        
        spriteBatch.Draw(pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.DarkRed);
        
        float healthPercentage = _health / _maxHealth;
        spriteBatch.Draw(pixel, new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthPercentage), barHeight), Color.Red);
        
        pixel.Dispose();
    }
}