using Godot;
using System;

public partial class MagicBullet : BulletBase
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void InitBullet(Vector2 direction)
    {
        base.InitBullet(direction);
        Rotation = Direction.Angle() - Mathf.Pi / 2;
    }
}
