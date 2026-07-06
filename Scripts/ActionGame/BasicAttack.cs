using Godot;
using System;
using System.Collections;
using HCoroutines;

public partial class BasicAttack : Node
{
    [Export] protected PlayerManager PlayerManagerRef;
    [Export] private PackedScene basicBulletPrefab;
    [Export] public int BulletCount;
    [Export] public float BasicAttackFrequency, TimeBetweenShots;
    [Export] private Timer basicAttackTimer;
    private BulletManager bulletManagerRef;
    private ActionRpcHandler actionRpcHandler;
    
    private PlayerForm playerForm;
    private bool isAllowPlayerInput, isAllowAttack;
    
    public override void _Ready()
    {
        playerForm = PlayerManagerRef.PlayerForm;
        bulletManagerRef = ActionGameBase.Instance.BulletManagerRef;
        actionRpcHandler = ActionRpcHandler.Instance;
        basicAttackTimer.WaitTime = BasicAttackFrequency;
        basicAttackTimer.Start();
        ConnectSignals();
    }
    private void ConnectSignals()
    {
        PlayerManagerRef.PlayerFormChanged += OnPlayerFormChange;
        PlayerManagerRef.SettingAllowPlayerInput += OnSettingAllowPlayerInput;
        basicAttackTimer.Timeout += MakeBasicAttack;
        actionRpcHandler.SlotPokerAction += (info, dictionary) => //temp lambda for testing
        {
            var bulletCount = info.Rank + 1; // +1 because poker index start a 0
            // MakeBasicAttack(bulletCount);
            GD.Print($"Making poker slotted attack, strength: {bulletCount}");
        };
    }

    public void MakeBasicAttack() //normal version
    {
        Co.Run(ShootBullets(BulletCount));
    }

    public void MakeBasicAttack(int shots) //frequency override version
    {
        Co.Run(ShootBullets(shots));
    }

    private IEnumerator ShootBullets(int count) //Coroutine to shoot bullets
    {
        for (var i = 0; i < count; i++)
        {
            ShootBasicBullet();
            yield return Co.Wait(TimeBetweenShots);
        }
    }
    
    private void ShootBasicBullet() //base function for shooting each bullet
    {
        var bullet = basicBulletPrefab.Instantiate<BasicBullet>();
        bullet.GlobalPosition = PlayerManagerRef.GlobalPosition;
        bullet.InitBullet(PlayerManagerRef.GetMouseToPlayerVector());
        AddChild(bullet); // temp AddChild function to make sure node is not orphaned.
        bulletManagerRef.BulletSpawned?.Invoke(bullet);
        GD.Print("Bullet Spawned");
    }

    

    private void OnPlayerFormChange(PlayerForm form)
    {
        playerForm = form;
    }

    private void OnSettingAllowPlayerInput(bool value)
    {
        isAllowPlayerInput = value;
        isAllowAttack =  value;
    }
}
