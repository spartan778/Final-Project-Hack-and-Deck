using Godot;
using System;
using Godot.Collections;

public partial class HandSlots : Node2D
{
    [Export] public int MaxHandCount {get; private set;}
    [Export] public Area2D PanelArea { get; private set; }
    [Export] public Array<PokerBase> Pokers { get; private set; }
    [Export] public PokerGameManager PokerGameManager { get; private set; }
    public int HandCount => Pokers.Count;

    public override void _Ready()
    {
        // PanelArea.AreaEntered += OnPanelAreaEntered;
        PokerGameManager.ReleasingPoker += OnReleasingPoker;
    }

    public void AddToHand(PokerBase poker)
    {
        if (HandCount >= MaxHandCount)
        {
            GD.Print("Too many pokers in hand");
            return;
        }
        if (!Pokers.Contains(poker))
        {
            Pokers.Add(poker);
        }
    }

    private void MoveToPanel(PokerBase poker)
    {
        poker.Position = PanelArea.GlobalPosition;
        GD.Print($"Adding to slot: {HandCount}");
    }

    private void OnReleasingPoker(PokerBase releasedPoker)
    {
        if (Pokers.Contains(releasedPoker))
        {
            GD.Print($"Poker: {releasedPoker} already in hand panel");
            OrganizePokers();
            return;
        }
        var overlappingAreas = PanelArea.GetOverlappingAreas();
        if (overlappingAreas.Count == 0) return;
        if (!CardGameHelperSingleton.TryFilterPokerBases(overlappingAreas, out var pokerArray)) return;
        foreach (var pokerBase in pokerArray)
        {
            
        }

    }
    private void OnPanelAreaEntered(Area2D area)
    {
        if (CardGameHelperSingleton.TryCheckForPokerBase(area, out var poker))
        {
            GD.Print($"Poker detected: {poker.PokerContent.PokerInfo}");
        }
    }

    public void OrganizePokers()
    {
        
    }
}
