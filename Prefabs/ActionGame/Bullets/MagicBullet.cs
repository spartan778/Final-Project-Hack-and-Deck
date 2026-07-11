using Godot;
using System;

public partial class MagicBullet : BulletBase
{
    public override void InitBullet(Vector2 direction)
    {
        base.InitBullet(direction);
        Rotation = Direction.Angle() + Mathf.Pi / 2; // match sprite rotation to shooting direction
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
}
