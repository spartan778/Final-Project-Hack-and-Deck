using Godot;
using System;
using Godot.Collections;
using Array = System.Array;

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

    public static bool TryFilterPokerBases(Array<Area2D> areas, out Array<PokerBase> pokerBaseArray)
    {
        pokerBaseArray = new Array<PokerBase>();
        foreach (var area in areas)
        {
            if (area.GetParent() is PokerBase)
            {
                pokerBaseArray.Add(area.GetParent() as PokerBase);
            }
        }
        if (pokerBaseArray.Count != 0) return true;
        GD.Print("No Pokers found");
        return false;
    }

    public static bool TryCheckForPokerBase(Area2D area2D, out PokerBase pokerBase)
    {
        pokerBase = area2D?.GetParent() as PokerBase;
        return pokerBase != null;
    }
}
