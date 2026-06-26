using Godot;
using System;
using Godot.Collections;

public partial class HandSlots : Node2D
{
    [Export] public int MaxHandCount {get; private set;}
    [Export] public Area2D PanelArea { get; private set; }
    [Export] public Array<PokerBase> Pokers { get; private set; }
    [Export] public PokerGameManager PokerGameManager { get; private set; }
    [Export] private Sprite2D handPanel;
    [Export] private float pokerSpacing;
    
    public int HandCount => Pokers.Count;
    private Vector2 PanelAreaSize => handPanel.RegionRect.Size;
    private float maxWidth;
    

    public override void _Ready()
    {
        PanelArea.AreaEntered += OnPanelAreaEntered;
        PokerGameManager.ReleasingPoker += OnReleasingPoker;
        maxWidth = PanelAreaSize.X;
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
            OrganizePokers();
        }
    }

    private void MoveToPanel(PokerBase poker)
    {
        poker.Position = PanelArea.GlobalPosition;
        GD.Print($"Adding to slot: {HandCount}");
    }

    private void OnHoldingPoker(PokerBase heldPoker)
    {
        
    }

    private void OnReleasingPoker(PokerBase releasedPoker)
    {
        // if (!PanelArea.OverlapsArea(releasedPoker))
        // {
        //     GD.Print($"{releasedPoker.PokerContent.PokerInfo} is out of hand panel");
        //     return;
        // }
        var overlappingAreas = PanelArea.GetOverlappingAreas();
        // GD.Print($"{overlappingAreas}");
        CardGameHelperSingleton.TryFilterPokerBases(overlappingAreas, out var pokerBaseArray);
        GD.Print($"Filtered Pokers: {pokerBaseArray}");
        if (!pokerBaseArray.Contains(releasedPoker)) return;
        
        GD.Print($"{releasedPoker.PokerContent.PokerInfo} is inside Hand Panel");
        if (Pokers.Contains(releasedPoker))
        {
            GD.Print($"Poker: {releasedPoker} already in hand panel");
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

    public void OrganizePokers() // Arrange pokers' position into a line
    {
        GD.Print($"Hand Panel size: {PanelAreaSize}");
        
    }
}
