using Godot;
using System;

public partial class CardSlotBase : Node2D, ICardSlotModule
{
    [Export] public Area2D SlotArea { get; private set;}
    [Export] public CardSlotModule CardSlotModule { get; private set;}
    public PokerBase SlottedPoker => CardSlotModule.SlottedPoker;
    private PokerGameManager pokerGameManagerRef;
    private RpcManager rpcManager;
    private DiscardPile discardPileRef;

    public bool IsLockingPoker => CardSlotModule.IsLockingPoker;

    public override void _Ready()
    {
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
        rpcManager = RpcManager.Instance;
        discardPileRef = pokerGameManagerRef.DiscardPile;
        CardSlotModule.PokerSlotted += OnPokerSlotted;
        
        CardSlotModule.SetPokerLock(true);
    }
    

    public void SendToDiscard()
    {
        if (SlottedPoker is null) return;
        discardPileRef.AddToDiscardPile(SlottedPoker);
        CardSlotModule.DeletePoker();
    }

    private void OnPokerSlotted(PokerDragging poker)
    {
        rpcManager.SlotPokerRpc(poker.PokerBaseRef.PokerContent.PokerInfo,
            poker.PokerBaseRef.PokerModifiersManager.ToDictionary());
    }
    
    public void TriggerPoker()
    {
        if(SlottedPoker is null) return;
        rpcManager.TriggerPokerRpc(SlottedPoker.PokerContent.PokerInfo,
            SlottedPoker.PokerModifiersManager.ToDictionary());
    }

    
}
