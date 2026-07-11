using Godot;
using System;

public partial class RedSkull : EnemyBase, IRangeAttacker
{
    [Export] public RangeAttackModule RangeAttackModule{get;set;}
    [Export] public float TimeBetweenAttacks{get;set;}
    [Export] public Timer AttackTimer;

    public override void _Ready()
    {
        base._Ready();
        AttackTimer.Timeout += OnAttackTimerTimeOut;
        AttackTimer.Start(TimeBetweenAttacks);
    }

    private void OnAttackTimerTimeOut()
    {
        RangeAttackModule.MakeRangedAttack_Player();
    }
}
