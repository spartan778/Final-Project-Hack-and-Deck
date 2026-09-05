using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class EnemySystem : Node
{
    public static EnemySystem Instance { get; private set; }
    [Export] public EnemyList StandardEnemies { get; private set; }
    [Export] public DifficultyManager DifficultyManagerRef{ get; private set; }
    [Export] public WaveManager WaveManagerRef{ get; private set; }
    [Export] public EnemySpawner EnemySpawnerRef{ get; private set; }
    private CardGameHelperSingleton cardGameHelperSingleton;
    private RpcManager rpcManager;
    private Array<EnemyBase> spawnedEnemies; 
    public PlayerManager PlayerManagerRef { get; private set; }
    
    public Action WaveChanged;
    public Action<int> UpdateEnemyDefeatCount;
    
    [Export] public int CardRewardTarget { get; private set; } = 15;
    public int WaveNumber => WaveManagerRef.WaveNumber;
    public int DifficultyLevel => DifficultyManagerRef.DifficultyLevel;
    public int EnemyDefeatedCount { get; private set; } = 0;
    
    
    
    
    
    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    

    public override void _Ready()
    {
        spawnedEnemies = new Array<EnemyBase>();
        WaveManagerRef.WaveTimer.Timeout += OnWaveTimerTimeout;
        PlayerManagerRef = ActionGameBase.Instance.PlayerManagerRef;
        cardGameHelperSingleton = CardGameHelperSingleton.Instance;
        rpcManager = RpcManager.Instance;
        RegularEnemySpawning();
    }

    private void OnWaveTimerTimeout()
    {
        RegularEnemySpawning();
    }

    private void RegularEnemySpawning()
    {
        SpawnRoutineEnemy();
        var waveNumber= WaveManagerRef.WaveNumber++;
        WaveChanged?.Invoke();
        if (waveNumber % DifficultyManagerRef.DifficultyIncreaseInterval == 0) // increase difficulty per interval
        {
            DifficultyManagerRef.IncreaseDifficulty();
        }
    }

    private void SpawnRoutineEnemy()
    {
        var availableValue = DifficultyManagerRef.DifficultyValue;
        var thresholdValue = StandardEnemies.DefaultEnemy.SpawnValue;
        var validEnemies = new List<EnemyInfo>();
        // GD.Print("Current Value: " +availableValue);
        while (availableValue >= thresholdValue)
        {
            validEnemies.Clear();
            foreach (var enemyInfo in StandardEnemies.EnemyInfos)
            {
                if (enemyInfo.SpawnValue <= availableValue)
                {
                    validEnemies.Add(enemyInfo);
                }
            }
            if (validEnemies.Count == 0)
            {
                break;
            }
            var pickedEnemyInfo = validEnemies[Random.Shared.Next(validEnemies.Count)]; // standard implementation from Microsoft: https://learn.microsoft.com/en-us/dotnet/api/system.random.shared?view=net-6.0
            EnemySpawnerRef.SpawnEnemy(pickedEnemyInfo, out var enemy);
            enemy.EnemyDefeated += OnEnemyDefeated;
            EnemySpawnerRef.PlaceRandomToPlayer_Default(enemy);
            AddChild(enemy);
            availableValue -= pickedEnemyInfo.SpawnValue;
        }
    }

    public EnemyInfo PickRandomEnemy(EnemyList enemyList)
    {
        var enemyInfos = enemyList.EnemyInfos;
        if (enemyInfos is null || enemyInfos.Count == 0)
        {
            GD.PrintErr("List is empty or null");
            return null;
        }
        var pickedEnemy= enemyInfos[Random.Shared.Next(enemyInfos.Count)];
        return pickedEnemy;
    }

    public void OnEnemyDefeated(EnemyBase enemy, Vector2 position)
    {
        // GD.Print($"{enemy} is defeated at {position}");
        EnemyDefeatedCount++;
        UpdateEnemyDefeatCount?.Invoke(EnemyDefeatedCount);
        if (EnemyDefeatedCount % CardRewardTarget == 0) // give card reward every time player reach the defeat enemy amount
        {
            SendNewPoker();
        }
    }
    public void SendNewPoker()
    {
        var newPoker = cardGameHelperSingleton.GenerateRandomPoker();
        rpcManager.GenerateNewPoker_Send(newPoker);
        GD.Print("Sending New Poker");
        
    }
}
