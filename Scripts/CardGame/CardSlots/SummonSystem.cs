using Godot;
using System;

public partial class SummonSystem : Node2D, ICardSlotModule
{
    [Export] public CardSlotModule CardSlotModule { get;private set; }
    [Export] private DiscardPile discardPileRef;
    [Export] private CardRpcHandler cardRpcHandlerRef;
    [Export] private Timer networkTickTimer, inverseTimer, coolDownTimer;
    [Export] private float inverseTime = 5f;
    [Export] private float coolDownTime = 15f;
    [Export] private AnimatedSprite2D summonAnimation;
    public PokerBase SlottedPoker => CardSlotModule.SlottedPoker;
    public PokerDragging DraggedPoker;
    public PokerBase SummonedPoker{ get;private set; }
    private RpcManager rpcManager;
    

    public override void _Ready()
    {
        rpcManager = RpcManager.Instance;
        inverseTimer.WaitTime = inverseTime;
        inverseTimer.OneShot = true;
        coolDownTimer.WaitTime = coolDownTime;
        coolDownTimer.OneShot = true;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        CardSlotModule.PokerSlotted += OnPokerSlotted;
        CardSlotModule.PokerUnslotted += OnPokerUnslotted;
        networkTickTimer.Timeout += OnNetworkTick;
        inverseTimer.Timeout += OnInverseTimerTimeout;
        coolDownTimer.Timeout += OnCoolDownTimerTimeout;
        cardRpcHandlerRef.SummonPokerUsedUpAction += OnSummonPokerUsedUp;
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
        poker.PokerDraggingRef.SetCollisionLayer(0); // make this card unscannable by card slots
        poker.PokerDraggingRef.SetCollisionLayerValue(4, true); // turn detection back on layer 4 ONLY
        GD.Print("PokerUnslotted Poker");
        coolDownTimer.Start();
        summonAnimation.Visible = false;
        CardSlotModule.IsSlotOpen = false;
    }

    public void ReturnToDiscard() // send summoned poker to discard pile
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

    private void OnCoolDownTimerTimeout()
    {
        summonAnimation.Visible = true;
        CardSlotModule.IsSlotOpen = true;
        rpcManager.SummonedPokerTimeOut_Send();
        ReturnToDiscard();
    }

    private void OnSummonPokerUsedUp()
    {
        GD.Print("Poker Used Up");
        ReturnToDiscard();
    }
    
}
