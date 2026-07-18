using Godot;
using System;

public partial class DrawPile : Node2D
{
    [Export] public ICardStorage CardStorage;
    [Export] public Node CardSystemRef { get;private set; }
    [Export] private DeckArea deckArea;
    [Export] private HandSlots handSlotsRef;
    [Export] private DiscardPile discardPileRef;
    
    private PackedScene pokerPrefab;
    private PokerGameManager pokerGameManagerRef;
    

    public override void _Ready()
    {
        CardStorage.StoredPokers = CardGameHelperSingleton.Instance.StartingDeck.SavedPokers;
        pokerPrefab = CardGameHelperSingleton.Instance.PokerPrefab;
        deckArea.IsClicked += DrawPoker;
        CardStorage.ReshufflePokers();
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
    }
    
    public void DrawPoker()
    {
        var drawnPokerInfo = CardStorage.DrawPoker();
        if (drawnPokerInfo == null)
        {
            return;
        }

        if (handSlotsRef.IsHandSlotFull)
        {
            GD.Print("HandSlot is full");
            var randomOffset = EnemySpawner.GetRandomPos(50, 150); //assign a random point on screen
            var overflowPoker = pokerPrefab.Instantiate<PokerBase>();
            overflowPoker.InitPoker(drawnPokerInfo);
            CardSystemRef.AddChild(overflowPoker);
            overflowPoker.GlobalPosition = GetGlobalMousePosition() + randomOffset;
            GD.Print($"DrawPoker(overflow): {drawnPokerInfo}");
            GD.Print($"Pos: {overflowPoker.GlobalPosition}");
            return;
        }
        var newPoker = pokerPrefab.Instantiate<PokerBase>();
        newPoker.InitPoker(drawnPokerInfo);
        CardSystemRef.AddChild(newPoker);
        // newPoker.GlobalPosition = new Vector2(500,500);
        pokerGameManagerRef.DrawingPoker?.Invoke(newPoker);
        GD.Print($"DrawPoker: {drawnPokerInfo}");
    }

    public PokerBase DrawPokerInit() // special init method to avoid race condition with card array
    {
        var drawnPokerInfo = CardStorage.DrawPoker();
        if (drawnPokerInfo == null)
        {
            GD.PrintErr("DrawPoker init failed");
            return null;
        }
        var newPoker = pokerPrefab.Instantiate<PokerBase>();
        newPoker.InitPoker(drawnPokerInfo);
        // GD.Print($"Draw Starting Poker: {drawnPokerInfo}");
        CardSystemRef.AddChild(newPoker);
        return newPoker;
    } 
}
