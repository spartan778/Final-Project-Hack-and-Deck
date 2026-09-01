using Godot;
using System;
using Godot.NativeInterop;

public partial class WaveManager : Node
{
    [Export] private EnemySpawner enemySpawnerRef;
    [Export] public Timer WaveTimer{ get; private set; }
    [Export] public float WaveInterval;
    public int WaveNumber;

    public override void _Ready()
    {
        WaveTimer.WaitTime = WaveInterval;
        WaveTimer.Start();
    }

    public void UpdateWaveInterval(float interval)
    {
        WaveTimer.WaitTime = interval;
    }
    
    
}
