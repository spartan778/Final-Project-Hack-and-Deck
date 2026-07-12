using Godot;
using System;

public partial class BasicRat : EnemyBase, IBumpAttacker
{
    [Export]public BumpAttackModule BumpAttackModule { get; set; }
    public override void _PhysicsProcess(double delta)
    {
        PursuitPlayerProcess();
    }
}
