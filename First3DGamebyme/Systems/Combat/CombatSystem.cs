using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace First3DGamebyme.Systems.Combat;

public class ComboAttack
{
    public int ComboIndex { get; set; }
    public float DamageMultiplier { get; set; }
    public float SpeedMultiplier { get; set; }
    public float RangeMultiplier { get; set; }
    public float KnockbackMultiplier { get; set; }
    public float WindowDuration { get; set; }
    public string AnimationName { get; set; }
    
    public ComboAttack(int index, float damage, float speed, float range, float knockback, float window)
    {
        ComboIndex = index;
        DamageMultiplier = damage;
        SpeedMultiplier = speed;
        RangeMultiplier = range;
        KnockbackMultiplier = knockback;
        WindowDuration = window;
    }
}

public class CombatSystem
{
    private Dictionary<WeaponType, List<ComboAttack>> _combos;
    private int _currentComboIndex;
    private float _comboWindowTimer;
    private float _hitPauseDuration = 0.1f;
    private float _hitPauseTimer;
    private bool _isHitPaused;
    
    public float ScreenShakeAmount { get; private set; }
    public float ScreenShakeDuration { get; private set; }
    private float _screenShakeTimer;
    
    public List<DamageNumber> ActiveDamageNumbers { get; private set; }
    public List<HitParticle> ActiveParticles { get; private set; }
    
    public bool IsInCombo => _comboWindowTimer > 0;
    public int CurrentComboIndex => _currentComboIndex;
    public bool IsHitPaused => _isHitPaused;
    
    public CombatSystem()
    {
        InitializeCombos();
        ActiveDamageNumbers = new List<DamageNumber>();
        ActiveParticles = new List<HitParticle>();
    }
    
    private void InitializeCombos()
    {
        _combos = new Dictionary<WeaponType, List<ComboAttack>>();
        
        _combos[WeaponType.Sword] = new List<ComboAttack>
        {
            new ComboAttack(0, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f),
            new ComboAttack(1, 1.2f, 0.9f, 1.1f, 1.2f, 0.4f),
            new ComboAttack(2, 1.5f, 0.8f, 1.2f, 1.5f, 0.0f)
        };
        
        _combos[WeaponType.Axe] = new List<ComboAttack>
        {
            new ComboAttack(0, 1.2f, 0.8f, 1.0f, 1.3f, 0.6f),
            new ComboAttack(1, 1.4f, 0.7f, 1.1f, 1.5f, 0.5f),
            new ComboAttack(2, 2.0f, 0.6f, 1.3f, 2.0f, 0.0f)
        };
        
        _combos[WeaponType.Hammer] = new List<ComboAttack>
        {
            new ComboAttack(0, 1.5f, 0.6f, 0.9f, 1.5f, 0.7f),
            new ComboAttack(1, 1.8f, 0.5f, 1.0f, 1.8f, 0.6f),
            new ComboAttack(2, 2.5f, 0.4f, 1.2f, 2.5f, 0.0f)
        };
        
        _combos[WeaponType.Dagger] = new List<ComboAttack>
        {
            new ComboAttack(0, 0.7f, 1.5f, 0.8f, 0.5f, 0.3f),
            new ComboAttack(1, 0.8f, 1.4f, 0.8f, 0.6f, 0.3f),
            new ComboAttack(2, 0.9f, 1.3f, 0.9f, 0.7f, 0.3f),
            new ComboAttack(3, 1.2f, 1.2f, 1.0f, 1.0f, 0.0f)
        };
    }
    
    public ComboAttack GetNextComboAttack(WeaponType weaponType, bool isAttacking)
    {
        if (!_combos.ContainsKey(weaponType))
            return new ComboAttack(0, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f);
        
        var combos = _combos[weaponType];
        
        if (!isAttacking || _comboWindowTimer <= 0)
        {
            _currentComboIndex = 0;
        }
        else if (isAttacking && _comboWindowTimer > 0)
        {
            _currentComboIndex = Math.Min(_currentComboIndex + 1, combos.Count - 1);
        }
        
        var combo = combos[_currentComboIndex];
        _comboWindowTimer = combo.WindowDuration;
        
        return combo;
    }
    
    public void Update(float deltaTime)
    {
        if (_comboWindowTimer > 0)
            _comboWindowTimer -= deltaTime;
        
        if (_hitPauseTimer > 0)
        {
            _hitPauseTimer -= deltaTime;
            if (_hitPauseTimer <= 0)
                _isHitPaused = false;
        }
        
        if (_screenShakeTimer > 0)
        {
            _screenShakeTimer -= deltaTime;
            ScreenShakeAmount = (_screenShakeTimer / ScreenShakeDuration) * 10f;
        }
        else
        {
            ScreenShakeAmount = 0;
        }
        
        for (int i = ActiveDamageNumbers.Count - 1; i >= 0; i--)
        {
            ActiveDamageNumbers[i].Update(deltaTime);
            if (!ActiveDamageNumbers[i].IsActive)
                ActiveDamageNumbers.RemoveAt(i);
        }
        
        for (int i = ActiveParticles.Count - 1; i >= 0; i--)
        {
            ActiveParticles[i].Update(deltaTime);
            if (!ActiveParticles[i].IsActive)
                ActiveParticles.RemoveAt(i);
        }
    }
    
    public void TriggerHitPause(float duration = 0.1f)
    {
        _isHitPaused = true;
        _hitPauseTimer = duration;
        _hitPauseDuration = duration;
    }
    
    public void TriggerScreenShake(float amount, float duration)
    {
        ScreenShakeAmount = amount;
        ScreenShakeDuration = duration;
        _screenShakeTimer = duration;
    }
    
    public void SpawnDamageNumber(Vector2 position, float damage, bool isCritical)
    {
        ActiveDamageNumbers.Add(new DamageNumber(position, damage, isCritical));
    }
    
    public void SpawnHitParticles(Vector2 position, Vector2 direction, int count)
    {
        Random rand = new Random();
        for (int i = 0; i < count; i++)
        {
            float angle = (float)Math.Atan2(direction.Y, direction.X) + (float)(rand.NextDouble() - 0.5) * MathHelper.Pi / 3;
            Vector2 particleDir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            float speed = 100f + (float)rand.NextDouble() * 200f;
            ActiveParticles.Add(new HitParticle(position, particleDir * speed));
        }
    }
    
    public void ResetCombo()
    {
        _currentComboIndex = 0;
        _comboWindowTimer = 0;
    }
}

public class DamageNumber
{
    public Vector2 Position { get; set; }
    public float Damage { get; set; }
    public bool IsCritical { get; set; }
    public float Timer { get; set; }
    public float Duration { get; set; }
    public bool IsActive { get; set; }
    
    public DamageNumber(Vector2 position, float damage, bool isCritical)
    {
        Position = position + new Vector2(0, -20);
        Damage = damage;
        IsCritical = isCritical;
        Duration = 1.0f;
        Timer = Duration;
        IsActive = true;
    }
    
    public void Update(float deltaTime)
    {
        Timer -= deltaTime;
        Position += new Vector2(0, -30) * deltaTime;
        if (Timer <= 0)
            IsActive = false;
    }
    
    public Color GetColor()
    {
        float alpha = Timer / Duration;
        if (IsCritical)
            return new Color(255, 215, 0, (int)(255 * alpha));
        else
            return new Color(255, 255, 255, (int)(255 * alpha));
    }
    
    public float GetScale()
    {
        if (IsCritical)
            return 1.5f + (1f - Timer / Duration) * 0.5f;
        else
            return 1.0f + (1f - Timer / Duration) * 0.3f;
    }
}

public class HitParticle
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Timer { get; set; }
    public float Duration { get; set; }
    public bool IsActive { get; set; }
    public Color Color { get; set; }
    
    public HitParticle(Vector2 position, Vector2 velocity)
    {
        Position = position;
        Velocity = velocity;
        Duration = 0.5f;
        Timer = Duration;
        IsActive = true;
        Color = Color.Yellow;
    }
    
    public void Update(float deltaTime)
    {
        Timer -= deltaTime;
        Position += Velocity * deltaTime;
        Velocity *= 0.95f;
        
        if (Timer <= 0)
            IsActive = false;
    }
    
    public Color GetColor()
    {
        float alpha = Timer / Duration;
        return new Color(Color.R, Color.G, Color.B, (int)(255 * alpha));
    }
}