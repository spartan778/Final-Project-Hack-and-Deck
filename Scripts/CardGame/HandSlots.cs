using Godot;
using System;
using Godot.Collections;

public partial class HandSlots : Node2D
{
    [Export] public int MaxHandCount {get; private set;}
    [Export] private int startHandCount;
    [Export] public Area2D PanelArea { get; private set; }
    [Export] public Array<PokerBase> Pokers { get; private set; }
    [Export] public PokerGameManager PokerGameManager { get; private set; }
    [Export] public DrawPile DrawPile { get; private set; }
    [Export] private ICardOrganizer cardOrganizer;
    
    
    public int HandCount => Pokers.Count;
    
    private float maxWidth;
    

    public override void _Ready()
    {
        ConnectSignals();
        InitPokerHand();
    }

    private void ConnectSignals()
    {
        PanelArea.AreaEntered += OnPanelAreaEntered;
        PokerGameManager.HoldingPoker += OnHoldingPoker;
        PokerGameManager.ReleasingPoker += OnReleasingPoker;
        PokerGameManager.DrawingPoker += AddToHand;
    }

    public void InitPokerHand()
    {
        for (var i = 0; i < startHandCount; i++)
        {
            var newPoker = DrawPile.DrawPokerInit();
            Pokers.Add(newPoker);
            
        }
        cardOrganizer.OrganizePokers();
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
        cardOrganizer.OrganizePokers();
    }

    public void RemoveFromHand(PokerBase poker)
    {
        if (!Pokers.Contains(poker)) return;
        Pokers.Remove(poker);
        cardOrganizer.OrganizePokers();
    }
    
    private void OnHoldingPoker(PokerBase heldPoker)
    {
        if (!Pokers.Contains(heldPoker))
        {
            GD.Print($"Not in hand: {heldPoker.PokerContent.PokerInfo}");
            return;
        }
        RemoveFromHand(heldPoker);
    }

    private void OnReleasingPoker(PokerBase releasedPoker)
    {
        if (!PanelArea.OverlapsArea(releasedPoker.PokerDraggingRef))
        {
            // GD.Print($"{releasedPoker.PokerContent.PokerInfo} is out of hand panel");
            return;
        }
        // GD.Print($"{releasedPoker.PokerContent.PokerInfo} is inside Hand Panel");
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
