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
        ConnectSignals();
        UpdateAttackBehaviour();
    }
    private void ConnectSignals()
    {
        PlayerManagerRef.PlayerFormChanged += OnPlayerFormChange;
        PlayerManagerRef.SettingAllowPlayerInput += OnSettingAllowPlayerInput;
        basicAttackTimer.Timeout += MakeBasicAttack;
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
        bulletManagerRef.BulletSpawned?.Invoke(bullet);
        // GD.Print("Bullet Spawned");
    }

    

    private void OnPlayerFormChange(PlayerForm form)
    {
        playerForm = form;
        UpdateAttackBehaviour();
    }

    private void UpdateAttackBehaviour()
    {
        if (playerForm is PlayerForm.Aggressive)
        {
            basicAttackTimer.Start();
            MakeBasicAttack();
        }
        else
        {
            basicAttackTimer.Stop(); // basic attack should only work when in offensive mode
        }
    }

    private void OnSettingAllowPlayerInput(bool value)
    {
        isAllowPlayerInput = value;
        isAllowAttack =  value;
    }
}
