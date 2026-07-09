using Godot;
using System;

public partial class BasicRat : EnemyBase
{
    public override void _PhysicsProcess(double delta)
    {
        PursuitPlayerProcess();
    }
}
