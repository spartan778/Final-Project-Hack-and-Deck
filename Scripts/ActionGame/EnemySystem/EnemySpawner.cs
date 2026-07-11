using Godot;
using System;

public partial class EnemySpawner : Node
{
    [Export] private PlayerManager playerManager;
    private Vector2 PlayerPosition => playerManager.GlobalPosition;

    public void SpawnEnemy(EnemyInfo info, out EnemyBase enemy)
    {
        var packedScene = info.EnemyPrefab;
        if (packedScene is null)
        {
            GD.PrintErr("can not find packed scene");
            enemy = null;
            return;
        }
        var temp = packedScene.Instantiate();
        if (temp is EnemyBase enemyBase)
        {
            enemy = enemyBase;
        }
        else
        {
            GD.PrintErr("Packed scene is not an enemy");
            enemy = null;
        }
    }

    public void PlaceRandomToPlayer_Default(EnemyBase enemy)
    {
        enemy.GlobalPosition = PlayerPosition + GetRandomPos();
    }

    public void PlaceRandomToPlayer_Range(EnemyBase enemy, float minDistance, float maxDistance)
    {
        enemy.GlobalPosition =  PlayerPosition + GetRandomPos(minDistance, maxDistance);
    }
    
    public static Vector2 GetRandomPos(float minDistance = 200f, float maxDistance = 400f) // General helper for generating a random offset
    {
        // Reference: https://stackoverflow.com/questions/5837572/generate-a-random-point-within-a-circle-uniformly
        float randomAngleRad = GD.Randf() * Mathf.Pi * 2; // Generate a random angle(0-360) in radian
        float randomDistance = (float)GD.RandRange(minDistance, maxDistance); // Generate a random distance between min and max distance
        Vector2 randomPos = Vector2.Right.Rotated(randomAngleRad) * randomDistance; // rotate a (1,0) vector by generated radian and multiply by distance
        return randomPos;
    }
}
