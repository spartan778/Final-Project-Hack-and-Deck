using Godot;
using System;
using Godot.Collections;

public partial class ActionRpcHandler : Node
{
    public static ActionRpcHandler Instance{get; private set;}
    public Action<PokerInfo, Dictionary> SlotPokerAction, TriggerPokerAction;
    public Action<PokerInfo, PokerState> SummonPokerAction;
    public Action<Vector2> SyncSummonedPokerPositionAction;
    public Action SummonedPokerTimeOutAction;
    public Action<int, int> SlottedColorCountAction;
    

    public override void _EnterTree()
    {
        Instance = this;
        RpcManager.Instance.SetActionRpcHandler(this);
    }

    public void HandlePokerSlotted(Vector2 pokerVector2, Dictionary modifiers)
    {
        var receivedPokerInfo = new PokerInfo(pokerVector2);
        // GD.Print($"Poker Modifier: {modifiers}");
        SlotPokerAction?.Invoke(receivedPokerInfo, modifiers);
    }

    public void HandlePokerSlotTriggered(Vector2 pokerVector2, Dictionary modifiers)
    {
        var receivedPokerInfo = new PokerInfo(pokerVector2);
        TriggerPokerAction?.Invoke(receivedPokerInfo, modifiers);
    }

    public void HandleTriggerSummonPoker(Vector2 pokerVector2, PokerState pokerState)
    {
        var receivedPokerInfo = new PokerInfo(pokerVector2);
        GD.Print($"Triggered summoning: {receivedPokerInfo}: {pokerState}");
        SummonPokerAction?.Invoke(receivedPokerInfo, pokerState);
    }
    
    public void SyncSummonedPokerPosition(Vector2 pokerPlacementRatio)
    {
        var screenSize = GetScreenPosByRatio(pokerPlacementRatio);
        // GD.Print($"Syncing Summoned Poker Position: {screenSize}");
        SyncSummonedPokerPositionAction?.Invoke(screenSize);
    }

    public void HandleSummonedPokerTimeOut()
    {
        SummonedPokerTimeOutAction?.Invoke();
    }

    public void HandleSlottedColorCount(int blackCount, int redCount)
    { 
        SlottedColorCountAction?.Invoke(blackCount, redCount);
    }
    
    public Vector2 GetScreenPosByRatio(Vector2 ratioVector) // helper for any cross-game position syncing
    {
        var renderSize = GetViewport().GetVisibleRect().Size;
        var vectorPos = ratioVector * renderSize;
        return vectorPos;
    }
}
