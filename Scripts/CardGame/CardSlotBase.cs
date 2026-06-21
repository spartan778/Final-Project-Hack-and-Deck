using Godot;
using System;

public partial class CardSlotBase : Node2D
{
    [Export] Area2D slotArea;
    public PokerBase SlottedPoker {get; private set;}
    private PokerGameManager pokerGameManagerRef;

    public override void _Ready()
    {
        
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
        pokerGameManagerRef.ReleasingPoker += OnPokerReleased;
    }

    private void OnPokerReleased(PokerBase poker)
    {
        var overlappingAreas = slotArea.GetOverlappingAreas();
        foreach (var area in overlappingAreas)
        {
            if (area is not PokerDragging draggedPoker) continue;
            if (draggedPoker.PokerBaseRef != poker) continue;
            SlotPoker(draggedPoker);
            break; // stop once a poker is slotted
        }
    }

    private void SlotPoker(PokerDragging poker)
    {
        SlottedPoker = poker.PokerBaseRef;
        GD.Print($"Slotted: {poker.PokerBaseRef} at {GetParent().Name}");
    }
    
}
