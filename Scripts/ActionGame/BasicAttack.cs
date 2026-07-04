using Godot;
using System;

public partial class BasicAttack : Node
{
    [Export] private PlayerManager playerManagerRef;
    
    private PlayerForm playerForm;
    private bool isAllowPlayerInput, isAllowAttack;
    
    public override void _Ready()
    {
        ConnectSignals();
        playerForm = playerManagerRef.PlayerForm;
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
