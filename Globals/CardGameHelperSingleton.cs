using Godot;
using System;

public partial class CardGameHelperSingleton : Node
{
    [Export] private Vector2 pokerBoundaries;
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

    public Vector2 CheckPokerBoundaries(Vector2 mousePosition) // helper for all mouse movements
    {
        var size = GetViewport().GetVisibleRect().Size;
        Vector2 minBounds = pokerBoundaries;
        Vector2 maxBounds = size - pokerBoundaries;
        return mousePosition.Clamp(minBounds, maxBounds);
    }
}
