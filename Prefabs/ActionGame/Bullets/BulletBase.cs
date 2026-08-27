using Godot;
using System;

public interface IBlockable // General interface for a blockable (destroyable) bullets and attacks
{
    bool IsBlockable { get; set; }
    bool IsAbsorbing { get; set; }
    bool IsFromEnemy {get;set;}
    bool IsFromPlayer {get;set;}
    void Blocked();

    void SetBlockingCollision(bool isBlockable, bool isAbsorbing);
    public static int DefaultBlockableLayer { get; } = 5;
}

public abstract partial class BulletBase : Area2D, IBlockable //base class for bullets
{
    [Export] public float BulletSpeed { get; private set; }
    [Export] public float Damage { get; private set; }
    [Export] public int PassThroughCount;
    [Export] public bool IsBlockable { get; set; }
    [Export] public bool IsAbsorbing { get; set; }
    [Export] public bool IsFromEnemy { get; set; }
    [Export] public bool IsFromPlayer { get; set; }
    public Vector2 Direction;

    protected bool IsReady;
    
    


    public override void _Ready()
    {
        // Direction = (GetGlobalMousePosition() - Position).Normalized();
        IsReady = true;
        AreaEntered += OnAreaEntered;
        SetBlockingCollision(IsBlockable, IsAbsorbing);
        
    }
    public virtual void InitBullet(Vector2 direction)
    {
        Direction = direction.Normalized();
    }

    public virtual void RotateToDirection() //helper to rotate non-sphere sprites to bullet vector
    {
        Rotation = Direction.Angle() + Mathf.Pi / 2; // match sprite rotation to shooting direction
    }
    public void SetBulletSpeed(float bulletSpeed){
        BulletSpeed = bulletSpeed;
    }
    public void SetDamage(float damage){
        Damage = damage;
    }
    
    public void OnAreaEntered(Area2D hitArea)
    {
        HandleHitProcess(hitArea);
        HandleBlockingProcess(hitArea);
    }

    private void HandleHitProcess(Area2D hitArea)
    {
        if (hitArea is not IDamageable target) return;
        target.TakeDamage(Damage);
        PassThroughCount--;
        if (PassThroughCount < 0)
        {
            QueueFree();
        }
    }

    protected virtual void HandleBlockingProcess(Area2D hitArea)
    {
        if(hitArea is not IBlockable blockable) return;
        // GD.Print("Bullet collision");
        if (IsFromEnemy && blockable.IsFromEnemy) // no interaction as both are enemy attacks
        {
            return;
        }

        if (IsFromPlayer && blockable.IsFromPlayer)
        {
            return;
        }
        if (blockable.IsAbsorbing) // if bullet is absorbing, they will destroy each other
        {
            Blocked();
            return;
        }
        if (blockable.IsBlockable)
        {
            blockable.Blocked();
            return;
        }
        /*if (blockable.IsAbsorbing)
        {
            Blocked();
            return;
        }
        if (blockable.IsBlockable)
        {
            blockable.Blocked();
        }*/
    }
    public override void _PhysicsProcess(double delta)
    {
        if(IsReady)
        {
            Position += Direction * BulletSpeed * (float)delta;
        }
    }

    public void Blocked()
    {
        QueueFree();
    }

    public void SetBlockingCollision(bool isBlockable, bool isAbsorbing)
    {
        IsBlockable = isBlockable;
        IsAbsorbing = isAbsorbing;
        
        SetCollisionMaskValue(IBlockable.DefaultBlockableLayer, IsAbsorbing || IsBlockable); 
        SetCollisionLayerValue(IBlockable.DefaultBlockableLayer, IsAbsorbing || IsBlockable);
    }
}

