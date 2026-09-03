using Godot;
using System;

public partial class ActionGameUIManager : Control
{
    [Export] private Label testLabel;
    [Export] private Label mainLabel, subLabel;
    [Export] private EnemySystem enemySystemRef;
    
    public override void _Ready()
    {
        var rpcManager = RpcManager.GetInstance();
        rpcManager.TestNumberChanged += OnTestNumberChanged;
        enemySystemRef.WaveChanged += OnWaveChanged;
        OnWaveChanged(); // init UI display
    }

    private void OnWaveChanged()
    {
        mainLabel.Text = $"Wave: {enemySystemRef.WaveNumber}, Difficulty Level: {enemySystemRef.DifficultyLevel}";
    }

    private void OnTestNumberChanged(int newValue)
    {
        testLabel.Text = $"Current count: {newValue}";
    }
    
    
}
