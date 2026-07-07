using Godot;
using System;

public partial class EnemyHitbox : Area2D, IDamageable
{
    [Export] public HealthSystem HealthSystem { get;private set; }
    [Export] private EnemyBase enemy;

    public void TakeDamage(float damage)
    {
        HealthSystem.TakeDamage(damage);
    }
}
