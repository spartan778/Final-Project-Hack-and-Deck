using Godot;
using System;

public partial class MagicBullet : BulletBase
{
    public override void InitBullet(Vector2 direction)
    {
        base.InitBullet(direction);
        RotateToDirection();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
}
