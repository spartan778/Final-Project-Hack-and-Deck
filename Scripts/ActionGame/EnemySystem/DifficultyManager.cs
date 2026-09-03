using Godot;
using System;

public partial class DifficultyManager : Node
{
    [Export] private EnemySystem enemySystemRef; 
    [Export] public float DifficultyValue { get; private set; }
    [Export] public int DifficultyIncreaseInterval;
    [Export] public int DifficultyIncreaseValue;
    public int DifficultyLevel { get; private set; } = 0;

    public void IncreaseDifficulty(int value = -1)
    {
        if (value < 0) // default case
        {
            DifficultyValue += DifficultyIncreaseValue;
            DifficultyLevel++;
            return;
        }
        DifficultyValue += value;
    }
}
