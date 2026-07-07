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

    public void TakeDamage(float amount)
    {
        HealthSystem.TakeDamage(amount);
    }
}
