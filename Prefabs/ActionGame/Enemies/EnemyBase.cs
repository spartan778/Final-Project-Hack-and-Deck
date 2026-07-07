using Godot;
using System;

public partial class EnemyBase : CharacterBody2D
{
    [Export] public float BaseHealth, BaseDamage, BaseSpeed;
    protected float CurrentDamage, CurrentSpeed;
    [Export] public HealthSystem HealthSystem { get; private set; }
    [Export] public MovementBehaviour MovementBehaviour{ get; private set; }
    [Export] protected EnemyHitbox Hitbox;
    
    protected PlayerManager PlayerManagerRef;

    public override void _Ready()
    {
        HealthSystem.InitHealthSystem(BaseHealth);
        PlayerManagerRef = ActionGameBase.Instance.PlayerManagerRef;
        CurrentDamage = BaseDamage;
        CurrentSpeed = BaseSpeed;
    }
    public Vector2 GetDirectionToPlayer()
    {
        var rawVector = PlayerManagerRef.GlobalPosition - GetGlobalPosition();
        return rawVector.Normalized();
    }

    protected void PursuitPlayerProcess()
    {
        var direction = GetDirectionToPlayer();
        if (direction != Vector2.Zero)
        {
            Velocity = direction * CurrentSpeed;
        }
        MoveAndSlide();
    }
}

public enum MovementBehaviour
{
    Pursuit,
    Dash,
    Stationary
}
