using Godot;
using System;
using Godot.Collections;

public partial class HandSlots : Node2D
{
    [Export] public int MaxHandCount {get; private set;}
    [Export] public Area2D PanelArea { get; private set; }
    [Export] public Array<PokerBase> Pokers { get; private set; }
    [Export] public PokerGameManager PokerGameManager { get; private set; }
    
    [Export] private ICardOrganizer cardOrganizer;
    
    
    public int HandCount => Pokers.Count;
    
    private float maxWidth;
    

    public override void _Ready()
    {
        PanelArea.AreaEntered += OnPanelAreaEntered;
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
            cardOrganizer.OrganizePokers();
        }
    }

    private void MoveToPanel(PokerBase poker)
    {
        poker.Position = PanelArea.GlobalPosition;
        GD.Print($"Adding to slot: {HandCount}");
    }

    private void OnHoldingPoker(PokerBase heldPoker)
    {
        if (!Pokers.Contains(heldPoker))
        {
            GD.Print($"Not in hand: {heldPoker.PokerContent.PokerInfo}");
            return;
        }
        Pokers.Remove(heldPoker);
    }

    private void OnReleasingPoker(PokerBase releasedPoker)
    {
        if (!PanelArea.OverlapsArea(releasedPoker.PokerDraggingRef))
        {
            GD.Print($"{releasedPoker.PokerContent.PokerInfo} is out of hand panel");
            return;
        }
        GD.Print($"{releasedPoker.PokerContent.PokerInfo} is inside Hand Panel");
        if (Pokers.Contains(releasedPoker))
        {
            GD.Print($"Poker: {releasedPoker.PokerContent.PokerInfo} already in hand");
        }
        else
        {
            AddToHand(releasedPoker);
        }

    }
    private void OnPanelAreaEntered(Area2D area)
    {
        if (CardGameHelperSingleton.TryCheckForPokerBase(area, out var poker))
        {
            GD.Print($"Poker detected: {poker.PokerContent.PokerInfo}");
        }
    }
}
