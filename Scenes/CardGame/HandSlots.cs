using Godot;
using System;
using Godot.Collections;

public partial class HandSlots : Node2D
{
    [Export] public Area2D PanelArea { get; private set; }
    [Export] public Array<PokerBase> Pokers { get; private set; }
    public int HandCount => Pokers.Count;

    public void AddToHand(PokerBase poker)
    {
        Pokers.Add(poker);
    }

    private void MoveToPanel(PokerBase poker)
    {
        poker.Position = PanelArea.GlobalPosition;
        GD.Print($"Adding to slot: {HandCount}");
    }
}
