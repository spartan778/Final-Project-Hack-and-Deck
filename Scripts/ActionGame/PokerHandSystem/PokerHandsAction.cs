using Godot;
using System;

public partial class PokerHandsAction : Node2D
{
    private ActionRpcHandler actionRpcHandler;
    
    public override void _Ready()
    {
        actionRpcHandler = ActionRpcHandler.Instance;
    }
}
