using Godot;
using System;
public interface IDamageable // General interface for damageable things in game
{
    public HealthSystem HealthSystem { get; }
    public void TakeDamage(float damage);
}
public partial class PlayerHitbox : Area2D, IDamageable
{
    [Export] public HealthSystem HealthSystem { get; private set; }
    [Export] private PlayerMovement playerMovementRef;
    [Export] public Timer InvincibilityTimer { get; private set; }
    
    public Action<bool> InvincibilityStateChanged;

    private bool isInvincible;
    public bool IsInvincible
    {
        get => isInvincible;
        private set
        {
            if (isInvincible == value) return; // no change in value
            isInvincible = value;
            GD.Print("IsInvincible: " + isInvincible);
            InvincibilityStateChanged?.Invoke(isInvincible);
        }
    }

    public override void _Ready()
    {
        playerMovementRef.StartDashing += OnStartDashing;
        InvincibilityTimer.Timeout += OnInvincibilityTimerTimeout;
    }

    private void OnStartDashing(float duration) // add Invincible time during dash
    {
        IsInvincible = true;
        if(!InvincibilityTimer.IsStopped() && InvincibilityTimer.TimeLeft >= duration) return; // do not override other source of Invincibility
        InvincibilityTimer.Start(duration);
    }
    private void OnInvincibilityTimerTimeout()
    {
        IsInvincible = false;
    }

    public void StartInvincibility(float duration)
    {
        if (!InvincibilityTimer.IsStopped() && InvincibilityTimer.TimeLeft >= duration) return;
        else
        {
            InvincibilityTimer.Start(duration);
            IsInvincible = true;
        }
        
    }
    
    public void TakeDamage(float amount)
    {
        if (IsInvincible) return; 
        HealthSystem.TakeDamage(amount);
    }
}
