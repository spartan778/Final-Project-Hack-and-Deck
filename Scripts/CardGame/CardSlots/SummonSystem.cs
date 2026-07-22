using Godot;
using System;

public partial class SummonSystem : Node2D, ICardSlotModule
{
    [Export] public CardSlotModule CardSlotModule { get;private set; }
    [Export] private DiscardPile discardPileRef;
    [Export] private Timer networkTickTimer, inverseTimer;
    [Export] private float inverseTime = 5f;
    public PokerBase SlottedPoker => CardSlotModule.SlottedPoker;
    public PokerDragging DraggedPoker;
    public PokerBase SummonedPoker{ get;private set; }
    private RpcManager rpcManager;

    public override void _Ready()
    {
        rpcManager = RpcManager.Instance;
        inverseTimer.WaitTime = inverseTime;
        inverseTimer.OneShot = true;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        CardSlotModule.PokerSlotted += OnPokerSlotted;
        CardSlotModule.PokerUnslotted += OnPokerUnslotted;
        networkTickTimer.Timeout += OnNetworkTick;
        inverseTimer.Timeout += OnInverseTimerTimeout;
        // CardGameBase.Instance.PokerGameManager.ReleasingPoker += OnReleasingPoker;
    }

    private void OnPokerSlotted(PokerDragging poker)
    {
        var pokerBase = poker.PokerBaseRef;
        SummonedPoker = pokerBase;
        inverseTimer.Start();
    }

    private void OnPokerUnslotted(PokerBase poker)
    {
        SummonedPoker.PokerSummoned?.Invoke();
        rpcManager.TriggerPokerSummonRpc(poker);
        networkTickTimer.Start();
        inverseTimer.Stop();
    }
    
    private void OnReleasingPoker(PokerBase poker)
    {
        if(poker != SummonedPoker) return;
    }

    public void ReturnToDiscard()
    {
        if(SummonedPoker == null) return;
        discardPileRef.AddToDiscardPile(SummonedPoker);
        SummonedPoker.QueueFree();
        SummonedPoker = null;
    }
    
    private void OnNetworkTick()
    {
        // rpcManager.SyncPokerSummonRpc_Send();
        if(SummonedPoker == null) return;
        var posVector = CardGameHelperSingleton.Instance.GetPokerPlacementVector(SummonedPoker);
        rpcManager.SyncPokerSummonRpc_Send(posVector);
        
    }
    private void OnInverseTimerTimeout()
    {
        SummonedPoker.PokerModifiersManager.SetPokerState(PokerState.Inversed);
    }
}
