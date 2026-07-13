using Godot;
using System;

public partial class CardGameBase : Node
{
    [Export] public PokerGameManager PokerGameManager { get; private set; }
    [Export] public Node2D GameBase2D { get; private set; }
    [Export] public Timer NetworkTickTimer { get; private set; }
    
    public static CardGameBase Instance { get; private set; }
    
    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }
}
