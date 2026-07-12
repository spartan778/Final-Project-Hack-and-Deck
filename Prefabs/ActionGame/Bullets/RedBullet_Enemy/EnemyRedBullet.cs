using Godot;
using System;

public partial class EnemyRedBullet : BulletBase
{
    public override void InitBullet(Vector2 direction)
    {
        base.InitBullet(direction);
        RotateToDirection(); // this is a directional sprite
    }
}
