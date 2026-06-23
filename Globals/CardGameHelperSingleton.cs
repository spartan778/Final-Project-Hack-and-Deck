using Godot;
using System;

public partial class CardGameHelperSingleton : Node
{
    [Export] private Vector2 pokerBoundaries;
    [Export] public PokerArray StartingDeck { get; private set; }
    [Export] public PackedScene PokerPrefab { get; private set; }
    public static CardGameHelperSingleton Instance{get; private set;}
    
    public PokerGameManager PokerGameManager {get; private set;}
    public override void _EnterTree()
    {
        Instance = this;
    }

    public void SetPokerGameManager(PokerGameManager pokerGameManager)
    {
        PokerGameManager = pokerGameManager;
    }

    public Vector2 CheckScreenBoundaries(Vector2 mousePosition) // helper for all mouse movements
    {
        var size = GetViewport().GetVisibleRect().Size;
        Vector2 minBounds = pokerBoundaries;
        Vector2 maxBounds = size - pokerBoundaries;
        return mousePosition.Clamp(minBounds, maxBounds);
    }
}
