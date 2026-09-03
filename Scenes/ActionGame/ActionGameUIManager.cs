using Godot;
using System;

public partial class ActionGameUIManager : Control
{
    [Export] private Label testLabel;
    [Export] private Label mainLabel, subLabel, defeatCountLabel;
    [Export] private EnemySystem enemySystemRef;
    
    public override void _Ready()
    {
        var rpcManager = RpcManager.GetInstance();
        rpcManager.TestNumberChanged += OnTestNumberChanged;
        enemySystemRef.WaveChanged += OnWaveChanged;
        enemySystemRef.UpdateEnemyDefeatCount += OnUpdateEnemyDefeatCount;
        subLabel.Text = "";
        OnWaveChanged(); // init UI display
    }

    private void OnUpdateEnemyDefeatCount(int newDefeatCount)
    {
        defeatCountLabel.Text = $"Defeated Count: {newDefeatCount}";
    }

    private void OnWaveChanged()
    {
        mainLabel.Text = $"Wave: {enemySystemRef.WaveNumber}, Difficulty Level: {enemySystemRef.DifficultyLevel}";
    }

    private async void OnPokerHandActive()
    {
        
    }

    private void OnTestNumberChanged(int newValue)
    {
        testLabel.Text = $"Current count: {newValue}";
    }
    
    
}
