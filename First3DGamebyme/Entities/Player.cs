using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using First3DGamebyme.Engine.Input;
using First3DGamebyme.Systems.Combat;

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
    
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public bool CanAttack => !_isDodging && _attackCooldownTimer <= 0;
    public bool CanDodge => !_isDodging && _dodgeCooldownTimer <= 0;
    public Weapon CurrentWeapon => _currentWeapon;
    public Attack CurrentAttack => _currentAttack;
    
    public Player(Vector2 position) : base(position)
    {
        MaxHealth = 100f;
        Health = MaxHealth;
        
        _currentWeapon = new Weapon("Basic Sword", WeaponType.Sword, 20f, 2f, 80f);
        _currentAttack = new Attack();
    }
    
    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _texture = new Texture2D(graphicsDevice, 32, 32);
        Color[] data = new Color[32 * 32];
        for (int i = 0; i < data.Length; i++)
            data[i] = Color.Red;
        _texture.SetData(data);
        
        _origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
    }
    
    public void HandleInput(InputManager input, GameTime gameTime, Vector2 worldMousePosition)
    {
        if (_isDodging) return;
        
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
        
        if (_isDodging)
        {
            _dodgeTimer -= deltaTime;
            if (_dodgeTimer <= 0)
            {
                _isDodging = false;
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
    
    public void TakeDamage(float damage)
    {
        if (_isDodging) return;
        
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            IsActive = false;
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
        
        _currentAttack.Start(Position, attackDirection, _currentWeapon);
        _attackCooldownTimer = 1f / _currentWeapon.AttackSpeed;
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        
        if (_currentAttack.IsActive)
        {
            _currentAttack.Draw(spriteBatch, spriteBatch.GraphicsDevice);
        }
    }
}