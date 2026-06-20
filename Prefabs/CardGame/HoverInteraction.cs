using Godot;
using System;

public partial class HoverInteraction : Node
{
    [Export] private Area2D pokerArea;
    [Export] private PokerBase pokerBase;
    private Vector2 originalScale;

    public override void _Ready()
    {
        pokerArea.MouseEntered += OnMouseEntered;
        pokerArea.MouseExited += OnMouseExit;
    }

    private void OnMouseEntered()
    {
        if(pokerBase.PokerGameManager.IsDragging) return; // ignore mouse if already holding a card
        pokerBase.Modulate = new Color(pokerBase.Modulate, 0.6f);
    }

    private void OnMouseExit()
    {
        if(pokerBase.PokerGameManager.IsDragging) return; // ignore mouse if already holding a card
        pokerBase.Modulate = new Color(pokerBase.Modulate, 1f);
    }
}
