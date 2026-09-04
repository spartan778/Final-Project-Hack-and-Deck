using Godot;
using System;

public interface ICardSlotModule
{
    CardSlotModule CardSlotModule { get; }
}

public partial class CardSlotModule : Node
{
    [Export] private Node2D parentNode2D;
    [Export] public Area2D SlotArea { get; private set;}
    [Export] private AudioStreamPlayer slotSoundPlayer;
    public PokerBase SlottedPoker {get; set;}
    public bool IsLockingPoker{get; set;}
    public bool IsSlotOpen;
    
    public Action<PokerDragging> PokerSlotted { get; set; }
    public Action<PokerBase> PokerUnslotted { get; set; }
    
    private PokerGameManager pokerGameManagerRef;

    public override void _Ready()
    {
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
        pokerGameManagerRef.ReleasingPoker += OnPokerReleased;
        pokerGameManagerRef.HoldingPoker += OnHoldingPoker;
        IsSlotOpen = true;
    }

    private void OnHoldingPoker(PokerBase poker)
    {
        if (SlottedPoker is null) return; 
        if (IsLockingPoker)
        {
            // GD.Print($"Slot is locked with {SlottedPoker.PokerContent.PokerInfo}");
            return;
        }
        if (SlottedPoker != poker) return; // only care if the Slotted poker is picked
        SlottedPoker = null; // remove slotted poker because it's picked
        GD.Print($"Unslotted: {poker.Name} at {parentNode2D.Name}");
        PokerUnslotted?.Invoke(poker);
    }
    
    private void OnPokerReleased(PokerBase poker)
    {
        if(!IsSlotOpen) return;
        var overlappingAreas = SlotArea.GetOverlappingAreas();
        if(overlappingAreas.Count == 0) return; // skip all logic when nothing is overlapping
        foreach (var area in overlappingAreas)
        {
            if (area is not PokerDragging draggedPoker) continue; // filter out all non-pokers
            if (draggedPoker.PokerBaseRef != poker) continue; // filter out all pokers not being dragged
            
            if (SlottedPoker != null && SlottedPoker != draggedPoker.PokerBaseRef)
            {
                RejectPoker(draggedPoker.PokerBaseRef);
                break;
            } ;
            if(!IsAccepted(poker)) return;
            SlotPoker(draggedPoker);
            break; // stop once a poker is slotted
        }
    }
    private void SlotPoker(PokerDragging poker)
    {
        SlottedPoker = poker.PokerBaseRef;
        // GD.Print($"Slotted: {poker.PokerBaseRef.Name} at {parentNode2D.Name}");
        SlottedPoker.Position = parentNode2D.GlobalPosition;
        if (IsLockingPoker) // lock poker to slot by default
        {
            poker.PokerBaseRef.SetPokerLock(IsLockingPoker);
        }
        slotSoundPlayer.Play();
        PokerSlotted?.Invoke(poker);
    }

    private void RejectPoker(PokerBase poker)
    {
        GD.Print($"{parentNode2D.Name} already has {SlottedPoker.PokerContent.PokerInfo}, rejecting poker {poker.PokerContent.PokerInfo}");
    }

    private bool IsAccepted(PokerBase poker) // a helper to filter out all not accepted pokers
    {
        if(poker.IsSummoned)
        {
            GD.Print($"Poker in Summoned state is not accepted");
            return false;
        }
        return true;
    }

    public void DeletePoker()
    {
        SlottedPoker.QueueFree();
        SlottedPoker = null;
    }

    public void SetPokerLock(bool value)
    {
        IsLockingPoker = value;
    }
    
    
}
