using Godot;
using System;
using Godot.Collections;

public partial class ActionRpcHandler : Node
{
    public static ActionRpcHandler Instance{get; private set;}
    public event Action<PokerInfo, Dictionary> SlotPokerAction, TriggerPokerAction;
    

    public override void _EnterTree()
    {
        Instance = this;
        RpcManager.Instance.SetActionRpcHandler(this);
    }

    public void HandlePokerSlotted(Vector2 pokerVector2, Dictionary modifiers)
    {
        var receivedPokerInfo = new PokerInfo(pokerVector2);
        GD.Print($"Poker Modifier: {modifiers}");
        SlotPokerAction?.Invoke(receivedPokerInfo, modifiers);
    }

    public void HandlePokerSlotTriggered(Vector2 pokerVector2, Dictionary modifiers)
    {
        var receivedPokerInfo = new PokerInfo(pokerVector2);
        // GD.Print($"Poker Modifier: {modifiers}");
        TriggerPokerAction?.Invoke(receivedPokerInfo, modifiers);
    }
}
