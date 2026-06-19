using Godot;
using System;

public partial class CardGameRefSingleton : Node
{
    public static CardGameRefSingleton Instance{get; private set;}
    
    public PokerGameManager PokerGameManager {get; private set;}
    public override void _EnterTree()
    {
        Instance = this; //set a global (static) reference
    }

    public void SetPokerGameManager(PokerGameManager pokerGameManager)
    {
        PokerGameManager = pokerGameManager;
    }
}
