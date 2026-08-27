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
    public bool IsInvincible { get; private set; }

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

    public void TakeDamage(float amount)
    {
        if (IsInvincible) return; 
        HealthSystem.TakeDamage(amount);
    }
}
