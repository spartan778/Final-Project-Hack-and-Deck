using Godot;
using System;

public partial class CardSlotBase : Node2D
{
    [Export] Area2D slotArea;
    public PokerBase SlottedPoker {get; private set;}
    private PokerGameManager pokerGameManagerRef;
    private RpcManager rpcManager;

    public override void _Ready()
    {
        
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
        pokerGameManagerRef.ReleasingPoker += OnPokerReleased;
        pokerGameManagerRef.HoldingPoker += OnHoldingPoker;
        rpcManager = RpcManager.Instance;
    }

    private void OnHoldingPoker(PokerBase poker)
    {
        if (SlottedPoker is null) return; 
        if (SlottedPoker != poker) return; // only care if the Slotted poker is picked
        SlottedPoker = null; // remove slotted poker because it's picked
        GD.Print($"Unslotted: {poker.Name} at {GetParent().Name}");
    }
    private void OnPokerReleased(PokerBase poker)
    {
        var overlappingAreas = slotArea.GetOverlappingAreas();
        if(overlappingAreas.Count == 0) return; // skip all logic when nothing is overlapping
        foreach (var area in overlappingAreas)
        {
            if (area is not PokerDragging draggedPoker) continue; // filter out all non-pokers
            if (draggedPoker.PokerBaseRef != poker) continue; // filter out  all pokers not being dragged
            if (SlottedPoker != null && SlottedPoker != draggedPoker.PokerBaseRef)
            {
                RejectPoker(draggedPoker.PokerBaseRef);
                break;
            };
            SlotPoker(draggedPoker);
            break; // stop once a poker is slotted
        }
    }

    private void SlotPoker(PokerDragging poker)
    {
        SlottedPoker = poker.PokerBaseRef;
        GD.Print($"Slotted: {poker.PokerBaseRef.Name} at {GetParent().Name}");
        SlottedPoker.Position = GlobalPosition;
        rpcManager.SlotPokerRpc(poker.PokerBaseRef.PokerContent.PokerInfo);
    }

    private void RejectPoker(PokerBase poker)
    {
        GD.Print($"{GetParent().Name} already has {SlottedPoker.PokerContent.PokerInfo}, rejecting poker {poker.PokerContent.PokerInfo}");
    }
}
