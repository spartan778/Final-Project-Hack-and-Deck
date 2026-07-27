using Godot;
using System;

public partial class CardRpcHandler : Node
{
    public static CardRpcHandler Instance {get; private set;}
    public Action SummonPokerUsedUpAction;
    
    public override void _EnterTree()
    {
        Instance = this;
        RpcManager.Instance.SetCardRpcHandler(Instance);
    }

    public void HandleSummonPokerUsedUp()
    {
        SummonPokerUsedUpAction?.Invoke();
    }
}
