using Godot;
using System;

public partial class CardGameBase : Node
{
    [Export] public PokerGameManager PokerGameManager { get; private set; }
    [Export] public Node2D GameBase2D { get; private set; }
    
}
