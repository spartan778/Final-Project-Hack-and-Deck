using Godot;
using System;

public partial class PokerContent : Node
{
    [Export] private AnimatedSprite2D cardDisplay;
    [Export] private CardSuit cardSuit;
    [Export] private int cardValue;
    public PokerInfo PokerInfo;
    public override void _EnterTree()
    {
        PokerInfo = new PokerInfo(cardSuit, cardValue);
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
        var newPokerInfo = new PokerInfo(suitEnum, value);
        UpdateDisplayInfo();
    }
    
    private void UpdateDisplayInfo()
    {
        var suitName = GetCardAnimationName(PokerInfo.CardSuit);
        if (suitName != null)
        {
            cardDisplay.SetAnimation(suitName);
            cardDisplay.SetFrame(PokerInfo.CardValue);
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

public struct PokerInfo
{
	public CardSuit CardSuit;
    public int CardValue;

    public PokerInfo()
    {
    }

    public PokerInfo(CardSuit cardSuit, int cardValue)
    {
        CardSuit = cardSuit;
        CardValue = cardValue;
    }
}
public enum CardSuit 
{
    Diamonds,
    Clubs, 
    Hearts, 
    Spades 
}

