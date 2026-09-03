using Godot;
using System;

public partial class CardRpcHandler : Node
{
    public static CardRpcHandler Instance {get; private set;}
    public Action SummonPokerUsedUpAction;
    public Action<PokerInfo> GeneratingNewPokerAction;
    
    public override void _EnterTree()
    {
        Instance = this;
        RpcManager.Instance.SetCardRpcHandler(Instance);
    }

    public void HandleSummonPokerUsedUp()
    {
        SummonPokerUsedUpAction?.Invoke();
    }

    public void HandleGeneratingNewPoker(PokerInfo pokerInfo)
    {
        GeneratingNewPokerAction?.Invoke(pokerInfo);
    }
}
