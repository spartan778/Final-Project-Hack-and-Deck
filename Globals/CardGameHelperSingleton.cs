using Godot;
using System;
using Godot.Collections;
using Array = System.Array;

public partial class CardGameHelperSingleton : Node
{
    
    public static CardGameHelperSingleton Instance{get; private set;}
    private RandomNumberGenerator rng;
    [Export] private Vector2 pokerBoundaries;
    [Export] public PokerArray StartingDeck { get; private set; }
    [Export] public PackedScene PokerPrefab { get; private set; }

    [Export] public int DefaultRankLimit { get; private set; } = 9; // rank 1-10 (No face cards)
    [Export] public int DefaultSuitLimit { get; private set; } = 1; // Diamond and Clubs
    
    public PokerGameManager PokerGameManager {get; private set;}
    public override void _EnterTree()
    {
        Instance = this;
        rng = new RandomNumberGenerator(); // create a godot built-in Random Number Generator 
        rng.Randomize();
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

    public static bool IsPokerValid(PokerInfo poker)
    {
        if (poker.Suit < 0 || (int)poker.Suit > 3 || poker.Rank < 0 || poker.Rank > 12) return false;
        return true;
    }

    public static bool IsPokerValid(Vector2 pokerVector)
    {
        if (pokerVector.X < 0 || pokerVector.X > 3 || pokerVector.Y < 0 || pokerVector.Y > 12) return false;
        return true;
    }

    public Vector2 GetPokerPlacementVector(PokerBase pokerBase)
    {
        var basePos = pokerBase.GetGlobalPosition();
        var renderSize = GetViewport().GetVisibleRect().Size;
        var vectorPos = new Vector2 (basePos.X/renderSize.X, basePos.Y/renderSize.Y);
        // GD.Print($"PosVector: {vectorPos}");
        return vectorPos;
    }

    public PokerInfo GenerateRandomPoker(bool isSpecial = false)
    {
        var rankLimit = DefaultRankLimit;
        var suitLimit = DefaultSuitLimit;
        if (isSpecial)
        {
            rankLimit = 12; // normal rank limit of a poker (13)
            suitLimit = 3; // 4 suit for pokers
        }
        var finalRank = rng.RandiRange(0, rankLimit);
        var finalSuit = rng.RandiRange(0, suitLimit);
        return new PokerInfo((CardSuit)finalSuit, finalRank);
    }
}
