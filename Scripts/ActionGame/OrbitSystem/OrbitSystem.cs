using Godot;
using System;

public partial class OrbitSystem : Node2D
{
    [Export] private OrbitAttack basicOrbitAttack;
    [Export] private PlayerManager playerManagerRef;
    
    private RpcManager rpcManagerRef;
    private ActionRpcHandler actionRpcHandlerRef;
    private PlayerForm playerForm;

    public override void _Ready()
    {
        playerForm = playerManagerRef.PlayerForm;
        actionRpcHandlerRef = ActionRpcHandler.Instance;
        ConnectSignals();
        // UpdateBasicOrbitAttack();
    }

    private void ConnectSignals()
    {
        playerManagerRef.PlayerFormChanged += OnPlayerFormChanged;
        actionRpcHandlerRef.SlottedColorCountAction += OnSlottedColorCountAction;
    }
    private void OnPlayerFormChanged(PlayerForm form)
    {
        playerForm = form;
        UpdateBasicOrbitAttack();
    }

    private void UpdateBasicOrbitAttack()
    {
        if (playerForm is PlayerForm.Aggressive)
        {
            basicOrbitAttack.SetProcessMode(ProcessModeEnum.Disabled); // disable the basic orbit attack
            basicOrbitAttack.Visible = false;
        }
        else
        {
            basicOrbitAttack.SetProcessMode(ProcessModeEnum.Inherit); // return to default behavior
            basicOrbitAttack.Visible = true;
            basicOrbitAttack.UpdateOrbit();
        }
    }

    private void OnSlottedColorCountAction(int blackCount, int redCount)
    {
        basicOrbitAttack.SetBonusBulletCount(blackCount);
        basicOrbitAttack.UpdateOrbit();
    }
}
