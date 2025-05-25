using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using First3DGamebyme.Engine.Input;
using First3DGamebyme.Systems.Combat;
using System;

namespace First3DGamebyme.Entities;

public class Player : Entity
{
    private float _moveSpeed = 300f;
    private float _dodgeSpeed = 600f;
    private float _dodgeDuration = 0.3f;
    private float _dodgeCooldown = 0.5f;
    
    private float _dodgeTimer = 0f;
    private float _dodgeCooldownTimer = 0f;
    private Vector2 _dodgeDirection;
    private bool _isDodging = false;
    
    private Weapon _currentWeapon;
    private Attack _currentAttack;
    private float _attackCooldownTimer = 0f;
    private CombatSystem _combatSystem;
    private float _perfectDodgeWindow = 0.15f;
    private bool _isPerfectDodgeActive = false;
    private float _perfectDodgeSlowMotion = 0.3f;
    private float _deathTimer = 0f;
    private float _deathDuration = 2f;
    private bool _isDying = false;
    
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public bool CanAttack => !_isDodging && _attackCooldownTimer <= 0 && !_combatSystem.IsHitPaused;
    public bool CanDodge => !_isDodging && _dodgeCooldownTimer <= 0;
    public Weapon CurrentWeapon => _currentWeapon;
    public Attack CurrentAttack => _currentAttack;
    public CombatSystem CombatSystem => _combatSystem;
    public bool IsPerfectDodgeActive => _isPerfectDodgeActive;
    
    public Player(Vector2 position) : base(position)
    {
        MaxHealth = 100f;
        Health = MaxHealth;
        
        _currentWeapon = new Weapon("Basic Sword", WeaponType.Sword, 20f, 2f, 150f);
        _currentAttack = new Attack();
        _combatSystem = new CombatSystem();
    }
    
    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _texture = new Texture2D(graphicsDevice, 32, 32);
        Color[] data = new Color[32 * 32];
        for (int i = 0; i < data.Length; i++)
            data[i] = Color.White;
        _texture.SetData(data);
        
        _origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
    }
    
    public void HandleInput(InputManager input, GameTime gameTime, Vector2 worldMousePosition)
    {
        if (_isDodging || _combatSystem.IsHitPaused) return;
        
        Vector2 movement = input.GetMovementVector();
        
        if (movement != Vector2.Zero)
        {
            Velocity = movement * _moveSpeed;
            Rotation = (float)System.Math.Atan2(movement.Y, movement.X) + MathHelper.PiOver2;
        }
        else
        {
            Velocity = Vector2.Zero;
        }
        
        if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space) && CanDodge)
        {
            StartDodge(movement != Vector2.Zero ? movement : new Vector2(0, -1));
        }
        
        if (input.IsMouseButtonPressed(MouseButton.Left) && CanAttack)
        {
            PerformAttack(worldMousePosition);
        }
    }
    
    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_isDying)
        {
            _deathTimer += deltaTime;
            if (_deathTimer >= _deathDuration)
            {
                IsActive = false;
            }
            return;
        }
        
        if (_combatSystem.IsHitPaused)
        {
            deltaTime *= 0.1f;
        }
        else if (_isPerfectDodgeActive)
        {
            deltaTime *= _perfectDodgeSlowMotion;
        }
        
        if (_isDodging)
        {
            _dodgeTimer -= deltaTime;
            if (_dodgeTimer <= 0)
            {
                _isDodging = false;
                _isPerfectDodgeActive = false;
                Velocity = Vector2.Zero;
            }
            else
            {
                Velocity = _dodgeDirection * _dodgeSpeed;
            }
        }
        
        if (_dodgeCooldownTimer > 0)
            _dodgeCooldownTimer -= deltaTime;
            
        if (_attackCooldownTimer > 0)
            _attackCooldownTimer -= deltaTime;
            
        _currentAttack.Update(deltaTime, Position);
        _combatSystem.Update(deltaTime);
        
        base.Update(gameTime);
    }
    
    private void StartDodge(Vector2 direction)
    {
        _isDodging = true;
        _dodgeTimer = _dodgeDuration;
        _dodgeCooldownTimer = _dodgeCooldown;
        _dodgeDirection = direction;
        _dodgeDirection.Normalize();
    }
    
    public void TakeDamage(float damage, Vector2 attackerPosition)
    {
        if (_isDodging)
        {
            if (_dodgeTimer >= _dodgeDuration - _perfectDodgeWindow)
            {
                _isPerfectDodgeActive = true;
                _combatSystem.TriggerScreenShake(5f, 0.2f);
            }
            return;
        }
        
        Health -= damage;
        _combatSystem.SpawnDamageNumber(Position, damage, false);
        
        if (Health <= 0)
        {
            Health = 0;
            _isDying = true;
            _deathTimer = 0f;
        }
    }
    
    public void Heal(float amount)
    {
        Health = MathHelper.Clamp(Health + amount, 0, MaxHealth);
    }
    
    private void PerformAttack(Vector2 mousePosition)
    {
        Vector2 direction = mousePosition - Position;
        float attackDirection = (float)System.Math.Atan2(direction.Y, direction.X);
        
        var combo = _combatSystem.GetNextComboAttack(_currentWeapon.Type, _currentAttack.IsActive);
        _currentAttack.Start(Position, attackDirection, _currentWeapon, combo);
        _attackCooldownTimer = (1f / _currentWeapon.AttackSpeed) / combo.SpeedMultiplier;
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        Color tint = Color.White;
        float scale = 1f;
        float rotation = Rotation;
        
        if (_isDying)
        {
            float deathProgress = _deathTimer / _deathDuration;
            scale = 1f - deathProgress * 0.5f;
            rotation += deathProgress * MathHelper.TwoPi * 3;
            tint = new Color(255, (int)(255 * (1f - deathProgress)), (int)(255 * (1f - deathProgress)));
            
            // Draw death particles
            Random rand = new Random((int)(_deathTimer * 1000));
            for (int i = 0; i < 5; i++)
            {
                float angle = (float)(rand.NextDouble() * MathHelper.TwoPi);
                float distance = deathProgress * 100f * (float)rand.NextDouble();
                Vector2 particlePos = Position + new Vector2(
                    (float)Math.Cos(angle) * distance,
                    (float)Math.Sin(angle) * distance
                );
                
                Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                
                Color particleColor = new Color(255, 0, 0, (int)(255 * (1f - deathProgress)));
                spriteBatch.Draw(pixel, particlePos, null, particleColor, 0f, new Vector2(0.5f, 0.5f), 4f, SpriteEffects.None, 0f);
                
                pixel.Dispose();
            }
        }
        else if (_isPerfectDodgeActive)
            tint = Color.Gold;
        else if (_isDodging)
            tint = new Color(150, 150, 150);
        
        spriteBatch.Draw(_texture, Position, null, tint, rotation, _origin, scale, SpriteEffects.None, 0f);
        
        if (_currentAttack.IsActive)
        {
            _currentAttack.Draw(spriteBatch, spriteBatch.GraphicsDevice);
        }
    }
}