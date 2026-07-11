using Godot;
using System;

public partial class EnemyBase : CharacterBody2D
{
    [Export] public float BaseHealth, BaseDamage, BaseSpeed;
    public float CurrentDamage { get; private set; }
    public float CurrentSpeed { get; private set; }
    [Export] public HealthSystem HealthSystem { get; private set; }
    [Export] public MovementBehaviour MovementBehaviour{ get; private set; }
    [Export] private Node2D enemySpriteBase;
    [Export] protected EnemyHitbox Hitbox;
    [Export] public int[] DefaultCollisionLayers;
    
    protected PlayerManager PlayerManagerRef;
    
    protected Vector2 VectorToPlayer;

    public override void _Ready()
    {
        HealthSystem.InitHealthSystem(BaseHealth);
        // GD.Print($"{Name}: Initializing health system: {HealthSystem}");
        PlayerManagerRef = ActionGameBase.Instance.PlayerManagerRef;
        CurrentDamage = BaseDamage;
        CurrentSpeed = BaseSpeed;
        // SetCollisionLayers(DefaultCollisionLayers);
        ConnectSignals();
    }
    protected virtual void SetCollisionLayers(int[] layers)
    {
        foreach (var layer in layers)
        {
            SetCollisionLayerValue(layer, true);
            SetCollisionMaskValue(layer, true);
        }
    }

    protected virtual void ConnectSignals()
    {
        HealthSystem.Dying += OnDying;
        HealthSystem.TakingDamage += OnTakingDamage;
        HealthSystem.Healing += OnHealing;

    }

    protected virtual void OnDying()
    {
        GD.Print($"Deleting: {Name}");
        QueueFree();
    }
    protected virtual void OnTakingDamage(float damage)
    {
        
    }

    protected virtual void OnHealing(float health)
    {
        
    }
    public Vector2 GetDirectionToPlayer()
    {
        var rawVector = PlayerManagerRef.GlobalPosition - GetGlobalPosition();
        return rawVector.Normalized();
    }

    protected virtual void TrackPlayer()
    {
        VectorToPlayer = GetDirectionToPlayer();
    }
    protected virtual void PursuitPlayerProcess()
    {
        TrackPlayer();
        if (VectorToPlayer != Vector2.Zero)
        {
            Velocity = VectorToPlayer * CurrentSpeed;
        }

        FlipToPlayer();
        MoveAndSlide();
    }
    
    

    protected virtual void FlipToPlayer() // Abs function used to avoid vector and value confusion
    {
        if (VectorToPlayer.X < 0) // flip to left
        {
            enemySpriteBase.Scale = new Vector2(-Mathf.Abs(enemySpriteBase.Scale.X), enemySpriteBase.Scale.Y);
        }
        else // flip to right
        {
            enemySpriteBase.Scale = new Vector2(Mathf.Abs(enemySpriteBase.Scale.X), enemySpriteBase.Scale.Y);
        }
    }
}

public enum MovementBehaviour
{
    Pursuit,
    Dash,
    Stationary
}

