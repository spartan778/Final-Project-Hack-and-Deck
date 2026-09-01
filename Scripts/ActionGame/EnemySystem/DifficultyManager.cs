using Godot;
using System;

public partial class DifficultyManager : Node
{
    [Export] private EnemySystem enemySystemRef; 
    [Export] public float DifficultyValue { get; private set; }
    [Export] public int DifficultyIncreaseInterval;
    [Export] public int DifficultyIncreaseValue;

    public void IncreaseDifficulty(int value = -1)
    {
        if (value < 0)
        {
            DifficultyValue += DifficultyIncreaseValue;
            return;
        }
        DifficultyValue += value;
    }
}
