using Godot;
using System;

public partial class CardSummonSystem : Node2D
{
    [Export] private PackedScene actionGamePokerPrefab;
    [Export] private PlayerManager playerManagerRef;
    public ActionGamePoker SummonedActionPoker{get; private set;}

    private ActionRpcHandler actionRpcHandlerRef;

    public override void _EnterTree()
    {
        // var pokerBase = pokerBasePrefab.InstantiateOrNull<PokerBase>();
        var actionPoker = actionGamePokerPrefab.InstantiateOrNull<ActionGamePoker>();
        // if (pokerBase == null)
        // {
        //     GD.PrintErr("Could not instantiate PokerBase");
        //     return;
        // }
        if (actionPoker == null)
        {
            GD.PrintErr("Could not instantiate ActionPoker");
            return;
        }
        // pokerBase.QueueFree();
        actionPoker.QueueFree();
        
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
        actionRpcHandlerRef.SummonedPokerTimeOutAction += OnSummonedPokerTimeOut;
    }

    private void OnSummonPoker(PokerInfo pokerInfo, PokerState pokerState)
    {
        GD.Print($"Triggered summoning: {pokerInfo}: {pokerState}");
        var pokerTemp = actionGamePokerPrefab.Instantiate<ActionGamePoker>();
        pokerTemp.InitActionPoker(pokerInfo, pokerState);
        pokerTemp.PokerModifiersManager.SetPokerState(pokerState);
        SummonedActionPoker = pokerTemp;
        playerManagerRef.AddChild(SummonedActionPoker);
    }

    private void OnSyncSummonedPokerPosition(Vector2 summonedPokerPosition)
    {
        if(SummonedActionPoker == null) return;
        var offset = GetViewport().GetVisibleRect().Size/2; // to properly align card placement on both screens
        // SummonedActionPoker.Position = summonedPokerPosition - playerManagerRef.Position + offset;
        // SummonedActionPoker.Position = summonedPokerPosition + offset;
        SummonedActionPoker.Position = summonedPokerPosition - offset;
    }

    private void OnSummonedPokerTimeOut()
    {
        SummonedActionPoker.QueueFree();
        SummonedActionPoker = null;
    }
}
