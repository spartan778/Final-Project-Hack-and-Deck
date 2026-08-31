using Godot;
using System;
using Godot.Collections;

public partial class StraightHand : Node2D
{
    [Export] private PlayerManager playerManagerRef;
    [Export] private Area2D attackRange;
    [Export] public Timer IntervalTimer { get; private set; }
    [Export] public Timer DurationTimer { get; private set; }
    [Export] public float DefaultInterval { get; private set; }
    [Export] public float DefaultDuration { get; private set; }
    [Export] private ChainLightening chainLighteningAttack;
    private ActionRpcHandler actionRpcHandler;

    public override void _Ready()
    {
        actionRpcHandler = ActionRpcHandler.Instance;
        IntervalTimer.WaitTime = DefaultInterval;
        DurationTimer.WaitTime = DefaultDuration;
        ConnectSignals();
        
    }

    private void ConnectSignals()
    {
        actionRpcHandler.ReleasedPokerHandAction += OnReleasedPokerHandAction;
        IntervalTimer.Timeout += IntervalTimerOnTimeout;
        DurationTimer.Timeout += DurationTimerOnTimeout;
    }

    private void DurationTimerOnTimeout()
    {
        DurationTimer.Stop();
        IntervalTimer.Stop();
    }

    private void IntervalTimerOnTimeout()
    {
        var enemy = GetClosestEnemyInArea();
        chainLighteningAttack.MakeChainAttack(this, enemy);
    }
    private void OnReleasedPokerHandAction(PokerHandBase pokerHand, ReleaseMode releaseMode)
    {
        if(pokerHand is not PokerHandBase.Straight) return;
        IntervalTimer.Start();
        DurationTimer.Start();
        var enemy = GetClosestEnemyInArea();
        chainLighteningAttack.MakeChainAttack(this, enemy);
    }
    private EnemyBase GetClosestEnemyInArea()
    {
        EnemyBase closestEnemy = null;
        var area2Ds = attackRange.GetOverlappingAreas();
        if (area2Ds.Count <= 0) return null;
        var enemiesInRange = new Array<EnemyBase>();
        foreach (var area2D in area2Ds)
        {
            if (area2D is EnemyHitbox enemyHitbox)
            {
                enemiesInRange.Add(enemyHitbox.Enemy);
            }
        }
        // GD.Print(enemiesInRange);
        var shortestDistance = -1f; // -1 is used as a control value
        foreach (var enemy in enemiesInRange)
        {
            float distanceSq = playerManagerRef.GlobalPosition.DistanceSquaredTo(enemy.GlobalPosition);
            if (shortestDistance < 0)
            {
                shortestDistance = distanceSq; // to make sure the first recorded distance is treated as shortest at the start
                closestEnemy = enemy;
            }
            if (distanceSq < shortestDistance)
            {
                shortestDistance = distanceSq;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }
    
    /*public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("test_make_handAttack"))
        {
            GD.Print("StraightHand: Debug trigger");
            OnReleasedPokerHandAction(PokerHandBase.Straight, ReleaseMode.Charged);
        };
    }*/
}
