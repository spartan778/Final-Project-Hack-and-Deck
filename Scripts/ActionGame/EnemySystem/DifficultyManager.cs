using Godot;
using System;

public partial class DifficultyManager : Node
{
    [Export] private EnemySystem enemySystemRef; 
    [Export] public float DifficultyValue { get; private set; }
}
