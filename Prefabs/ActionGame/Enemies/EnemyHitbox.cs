using Godot;
using System;

public partial class EnemyHitbox : Area2D, IDamageable
{
    [Export] public HealthSystem HealthSystem { get;private set; }
    [Export] private EnemyBase enemy;
    public EnemyBase Enemy => enemy;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }
    public void TakeDamage(float damage)
    {
        HealthSystem.TakeDamage(damage);
    }

    protected void OnAreaEntered(Area2D area)
    {
        if (area is PlayerHitbox hitbox)
        {
           hitbox.TakeDamage(enemy.CurrentDamage); 
        }
    }

    public void KnockBack(float strength)
    {
        enemy.KnockAwayFromPlayer(strength);
    }
}
