using Godot;
using System;

public partial class SummonSystem : Node2D
{
    [Export] public Area2D SlotArea { get; private set; }
    public PokerBase SlottedPoker { get; set; }
    
    public void SlotPoker(PokerDragging poker)
    {
        SlottedPoker = poker.PokerBaseRef;
        GD.Print($"Slotted: {poker.PokerBaseRef.Name} at {GetParent().Name}");
        SlottedPoker.Position = GlobalPosition;
    }
}
