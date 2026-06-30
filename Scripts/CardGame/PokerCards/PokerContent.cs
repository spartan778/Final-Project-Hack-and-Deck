using Godot;
using System;

public partial class PokerContent : Node
{
    [Export] private AnimatedSprite2D cardDisplay;
    [Export] public PokerInfo PokerInfo;
    public override void _EnterTree()
    {
        if (PokerInfo == null)
        {
            GD.Print("PokerInfo not set");
        }
    }

    public override void _Ready()
    {
        UpdatePokerDisplay();
    }
    
    public void UpdatePokerDisplay()
    {
        UpdateDisplayInfo();
    }

    public void ChangePokerInfo(PokerInfo pokerInfo)
    {
        PokerInfo = pokerInfo;
        UpdateDisplayInfo();
    }

    public void ChangePokerInfo(int suit, int value)
    {
        var suitEnum = (CardSuit)suit;
        // var newPokerInfo = new PokerInfo(suitEnum, value);
        
        UpdateDisplayInfo();
    }
    
    private void UpdateDisplayInfo()
    {
        var suitName = GetCardAnimationName(PokerInfo.Suit);
        if (suitName != null)
        {
            cardDisplay.SetAnimation(suitName);
            cardDisplay.SetFrame(PokerInfo.Rank);
        }
        
    }

    public static string GetCardAnimationName(CardSuit cardSuit) // static helper for getting card suit
    {
        switch (cardSuit)
        {
            case CardSuit.Diamonds:
            {
                return "DiamondCardSprites";
            }
            case CardSuit.Clubs:
            {
                return "ClubCardSprites";
            }
            case CardSuit.Hearts:
            {
                return "HeartCardSprites";
            }
            case CardSuit.Spades:
            {
                return "SpadeCardSprites";
            }
        }
        return null;
    }
    
}

public enum CardSuit 
{
    Diamonds,
    Clubs, 
    Hearts, 
    Spades 
}

public enum PokerState // all cards are assumed as Normal
{
    Normal,
    Inversed
}

public enum PokerType // all cards are assumed as Basic
{
    Basic,
    OneOff
}

