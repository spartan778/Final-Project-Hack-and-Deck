using Godot;
using System;

public partial class BasicAttack : Node
{
    [Export] private PlayerManager playerManagerRef;
    [Export] private PackedScene basicBulletPrefab;
    [Export] public int BasicAttackFrequency;
    private BulletManager bulletManagerRef;
    
    private PlayerForm playerForm;
    private bool isAllowPlayerInput, isAllowAttack;
    
    public override void _Ready()
    {
        ConnectSignals();
        playerForm = playerManagerRef.PlayerForm;
        bulletManagerRef = ActionGameBase.Instance.BulletManagerRef;
    }

    public void MakeBasicAttack() //normal version
    {
        
    }

    public void MakeBasicAttack(int shots) //frequency override version
    {
        
    }

    private void ShootBasicBullet()
    {
        
    }

    public void ConnectSignals()
    {
        playerManagerRef.PlayerFormChanged += OnPlayerFormChange;
        playerManagerRef.SettingAllowPlayerInput += OnSettingAllowPlayerInput;
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
