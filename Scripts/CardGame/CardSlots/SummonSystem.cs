using Godot;
using System;

public partial class SummonSystem : Node2D, ICardSlotModule
{
    [Export] public CardSlotModule CardSlotModule { get;private set; }
    [Export] private DrawPile drawPileRef;
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
        coolDownTimer.Start();
        summonAnimation.Visible = false;
        CardSlotModule.IsSlotOpen = false;
    }

    private void ReturnToDiscard()
    {
        if(SummonedPoker == null) return;
        switch (SummonedPoker.PokerModifiersManager.PokerState)
        {
            // send summoned poker to discard pile and decrease rank by 1
            case PokerState.Inversed:
            {
                if (SummonedPoker.PokerContent.PokerInfo.Rank == 0)
                {
                    SummonedPoker.PokerContent.PokerInfo.Rank = 12; // convert an "A" into a "K" (overflow)
                }
                else
                {
                    SummonedPoker.PokerContent.PokerInfo.Rank--;
                }
                break;
            }
            // send summoned poker to discard pile and increase rank by 1
            case PokerState.Normal:
            {
                if (SummonedPoker.PokerContent.PokerInfo.Rank == 12)
                {
                    SummonedPoker.PokerContent.PokerInfo.Rank = 0; // convert an "K" into a "A" (overflow)
                }
                else
                {
                    SummonedPoker.PokerContent.PokerInfo.Rank++;
                }
                break;
            }
        }

        discardPileRef.AddToDiscardPile(SummonedPoker);
        SummonedPoker.QueueFree();
        SummonedPoker = null;
    }

    private void AddToDraw()
    {
        if(SummonedPoker == null) return;
        switch (SummonedPoker.PokerModifiersManager.PokerState)
        {
            // send summoned poker to draw pile and decrease rank by 1
            case PokerState.Inversed:
            {
                if (SummonedPoker.PokerContent.PokerInfo.Rank == 0)
                {
                    SummonedPoker.PokerContent.PokerInfo.Rank = 12; // convert an "A" into a "K" (overflow)
                }
                else
                {
                    SummonedPoker.PokerContent.PokerInfo.Rank--;
                }
                break;
            }
            // send summoned poker to draw pile and increase rank by 1
            case PokerState.Normal:
            {
                if (SummonedPoker.PokerContent.PokerInfo.Rank == 12)
                {
                    SummonedPoker.PokerContent.PokerInfo.Rank = 0; // convert an "K" into a "A" (overflow)
                }
                else
                {
                    SummonedPoker.PokerContent.PokerInfo.Rank++;
                }
                break;
            }
        }
        drawPileRef.CardStorage.InsertPoker(SummonedPoker.PokerContent.PokerInfo);
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
        AddToDraw();
    }

    private void OnSummonPokerUsedUp()
    {
        GD.Print("(Received RPC) Poker Used Up");
        ReturnToDiscard();
    }
    
    
}
