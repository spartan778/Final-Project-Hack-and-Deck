using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;

[GlobalClass]
public partial class PokerInfo : Resource
{
    [Export] public CardSuit Suit;
    [Export(PropertyHint.Range, "0, 13")] public int Rank;

    public PokerInfo() // Intentional: keep empty constructor for edge case
    { }

    public PokerInfo(CardSuit suit, int rank)
    {
        Suit = suit;
        if (rank < 0 || rank > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(rank), rank, "Rank not between 0 and 12");
        }
        Rank = rank;
    }

    public PokerInfo(Vector2 pokerVector)
    {
        if (pokerVector.X is < 0 or > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(pokerVector.X), pokerVector, "PokerVector.X must be between 0 and 3");
        }
        Suit = (CardSuit)pokerVector.X;
        if (pokerVector.Y < 0 || pokerVector.Y > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(pokerVector.Y), pokerVector, "PokerVector.Y must be between 0 and 12");
        }
        Rank = (int)pokerVector.Y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2((int)Suit, (int)Rank);
    }
    
    public override string ToString() => $"{Rank+1} of {Suit}";
    
}
