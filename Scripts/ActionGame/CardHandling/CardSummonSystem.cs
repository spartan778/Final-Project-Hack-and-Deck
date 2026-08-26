using Godot;
using System;

public partial class CardSummonSystem : Node2D
{
    [Export] private PackedScene actionGamePokerPrefab;
    [Export] private PlayerManager playerManagerRef;
    public ActionGamePoker SummonedActionPoker{get; private set;}

    public Action SummonPokerUsedUp;
    
    private RpcManager rpcManager;
    private ActionRpcHandler actionRpcHandlerRef;

    public override void _EnterTree()
    {
        var actionPoker = actionGamePokerPrefab.InstantiateOrNull<ActionGamePoker>();
        if (actionPoker == null)
        {
            GD.PrintErr("Could not instantiate ActionPoker");
            return;
        }
        actionPoker.QueueFree();
        
    }
    public override void _Ready()
    {
        actionRpcHandlerRef = ActionRpcHandler.Instance;
        rpcManager = RpcManager.Instance;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        actionRpcHandlerRef.SummonPokerAction += OnSummonPoker;
        actionRpcHandlerRef.SyncSummonedPokerPositionAction += OnSyncSummonedPokerPosition;
        actionRpcHandlerRef.SummonedPokerTimeOutAction += OnSummonedPokerTimeOut;
        SummonPokerUsedUp += OnSummonPokerUsedUp;
    }

    private void OnSummonPoker(PokerInfo pokerInfo, PokerState pokerState)
    {
        GD.Print($"Triggered summoning: {pokerInfo}: {pokerState}");
        var pokerTemp = actionGamePokerPrefab.Instantiate<ActionGamePoker>();
        pokerTemp.InitActionPoker(pokerInfo, pokerState);
        pokerTemp.PokerModifiersManager.SetPokerState(pokerState);
        SummonedActionPoker = pokerTemp;
        playerManagerRef.AddChild(SummonedActionPoker);
        SummonedActionPoker.SetCardSummonSystem(this);
    }

    private void OnSyncSummonedPokerPosition(Vector2 summonedPokerPosition)
    {
        if(SummonedActionPoker == null) return;
        var offset = GetViewport().GetVisibleRect().Size/2; // to properly align card placement on both screens
        SummonedActionPoker.Position = summonedPokerPosition - offset;
    }

    private void OnSummonedPokerTimeOut()
    {
        if(SummonedActionPoker == null) return;
        SummonedActionPoker.QueueFree();
        SummonedActionPoker = null;
    }

    private void OnSummonPokerUsedUp()
    {
        SummonedActionPoker = null;
        rpcManager.SummonedPokerUsedUp_Send();
    }
}
