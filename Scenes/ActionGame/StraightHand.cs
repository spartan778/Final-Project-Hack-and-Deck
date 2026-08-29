using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class StraightHand : Node2D
{
    [Export] private PackedScene chainLighteningPrefab;
    [Export] private PlayerManager playerManagerRef;
    [Export] private Area2D attackRange;
    [Export] private Timer intervalTimer;


    public override void _Ready()
    {
        intervalTimer.Timeout += IntervalTimerOnTimeout;
    }

    private void IntervalTimerOnTimeout()
    {
        GD.Print("Time out");
        var enemy = GetClosestEnemyInArea();
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
        GD.Print(enemiesInRange);
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
}
