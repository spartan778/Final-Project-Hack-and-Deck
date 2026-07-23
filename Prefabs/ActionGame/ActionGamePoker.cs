using Godot;
using System;

public partial class ActionGamePoker : Node2D
{
    [Export] public PokerContent PokerContent { get; private set; }
    [Export] public Area2D PokerArea{ get; private set; }
    [Export] public PokerModifiersManager PokerModifiersManager { get; private set; }
    [Export] public PokerSummonManager PokerSummonManager { get; private set; }
    
    public PokerState PokerState => PokerModifiersManager.PokerState;
    public PokerType PokerType => PokerModifiersManager.PokerType;

    public override void _Ready()
    {
        
    }
    
    public void InitActionPoker(PokerInfo pokerInfo, PokerState pokerState)
    {
        PokerContent.ChangePokerInfo(pokerInfo);
        PokerModifiersManager.SetPokerState(pokerState);
        PokerSummonManager.SetShowSummonEffect(true);
    }
}
