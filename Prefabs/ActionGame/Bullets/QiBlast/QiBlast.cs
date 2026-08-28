using Godot;
using System;

public partial class QiBlast : BulletBase
{
    public override void InitBullet(Vector2 direction)
    {
        base.InitBullet(direction);
        RotateToDirection();
    }
}
