using Godot;
using System;

[GlobalClass]
public partial class PokerInfo : Resource
{
    [Export] public CardSuit Suit;
    [Export(PropertyHint.Range, "0, 13")] public int Rank;
    
    public override string ToString() => $"{Rank+1} of {Suit}";
}
