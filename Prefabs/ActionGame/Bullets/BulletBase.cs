using Godot;
using System;

public interface IBlockable // General interface for a blockable (destroyable) bullets and attacks
{
    void Blocked();
}

public abstract partial class BulletBase : Area2D, IBlockable //base class for bullets
{
    [Export] public float BulletSpeed { get; private set; }
    [Export] public float Damage { get; private set; }
    
    public Vector2 Direction;

    protected bool IsReady;


    public override void _Ready()
    {
        Direction = (GetGlobalMousePosition() - Position).Normalized();
        IsReady = true;
    }
    public virtual void InitBullet(Vector2 direction)
    {
        Direction = direction;
    }
    
    public void SetBulletSpeed(float bulletSpeed){
        BulletSpeed = bulletSpeed;
        
    }
    public void SetDamage(float damage){
        Damage = damage;
    }
    
    public void _AreaEntered(Area2D hitArea)
    {
        if (hitArea is IDamageable target)
        {
            target.TakeDamage(Damage);
        }
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
}

