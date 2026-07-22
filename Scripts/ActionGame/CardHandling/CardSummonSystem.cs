using Godot;
using System;

public partial class CardSummonSystem : Node2D
{
    [Export] private PackedScene pokerBasePrefab;
    [Export] private PlayerManager playerManagerRef;
    public PokerBase SummonedPokerBase{get; private set;}

    private ActionRpcHandler actionRpcHandlerRef; 

    public override void _EnterTree()
    {
        var pokerBase = pokerBasePrefab.InstantiateOrNull<PokerBase>();
        if (pokerBase == null)
        {
            GD.PrintErr("Could not instantiate PokerBase");
            return;
        }
        pokerBase.QueueFree();
        
    }
    public override void _Ready()
    {
        actionRpcHandlerRef = ActionRpcHandler.Instance;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        actionRpcHandlerRef.SummonPokerAction += OnSummonPoker;
        actionRpcHandlerRef.SyncSummonedPokerPositionAction += OnSyncSummonedPokerPosition;
    }

    private void OnSummonPoker(PokerInfo pokerInfo, PokerState pokerState)
    {
        GD.Print($"Triggered summoning: {pokerInfo}: {pokerState}");
        var pokerTemp = pokerBasePrefab.Instantiate<PokerBase>();
        pokerTemp.InitPoker(pokerInfo);
        pokerTemp.PokerModifiersManager.SetPokerState(pokerState);
        SummonedPokerBase = pokerTemp;
        playerManagerRef.AddChild(SummonedPokerBase);
    }

    private void OnSyncSummonedPokerPosition(Vector2 summonedPokerPosition)
    {
        if(SummonedPokerBase == null) return;
        var offset = new Vector2(0, 100);
        SummonedPokerBase.Position = summonedPokerPosition - playerManagerRef.Position + offset;
    }
}
