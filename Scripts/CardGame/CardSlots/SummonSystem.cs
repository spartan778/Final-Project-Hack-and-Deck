using Godot;
using System;

public partial class SummonSystem : Node2D, ICardSlotModule
{
    [Export] public CardSlotModule CardSlotModule { get;private set; }
    [Export] private Timer networkTickTimer;
    public PokerBase SlottedPoker => CardSlotModule.SlottedPoker;
    public PokerDragging DraggedPoker;
    public PokerBase SummonedPoker{ get;private set; }
    private RpcManager rpcManager;

    public override void _Ready()
    {
        rpcManager = RpcManager.Instance;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        CardSlotModule.PokerSlotted += OnPokerSlotted;
        CardSlotModule.PokerUnslotted += OnPokerUnslotted;
        networkTickTimer.Timeout += OnNetworkTick;
    }

    private void OnPokerSlotted(PokerDragging poker)
    {
        var pokerBase = poker.PokerBaseRef;
        SummonedPoker = pokerBase;
    }

    private void OnPokerUnslotted(PokerBase poker)
    {
        SummonedPoker.PokerSummoned?.Invoke();
        rpcManager.TriggerPokerSummonRpc(poker);
    }
    
    private void OnNetworkTick()
    {
        //TODO
    }
}
