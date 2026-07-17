using Godot;
using System;

public partial class HoverInteraction : Node
{
    [Export] private Area2D pokerArea;
    [Export] private PokerBase pokerBase;
    private Vector2 originalScale;
    private PokerGameManager pokerGameManager;

    public override void _Ready()
    {
        pokerArea.MouseEntered += OnMouseEntered;
        pokerArea.MouseExited += OnMouseExit;
        pokerGameManager = CardGameHelperSingleton.Instance.PokerGameManager;
    }

    private void OnMouseEntered()
    {
        if(pokerGameManager.IsDragging) return; // ignore mouse if already holding a card
        if(pokerGameManager.HoveredPoker != null && pokerGameManager.HoveredPoker != pokerBase) return; // make sure only ONE poker is being hovered
        pokerBase.Modulate = new Color(pokerBase.Modulate, 0.6f);
        pokerGameManager.HoveringPoker?.Invoke(pokerBase);
        
    }

    private void OnMouseExit()
    {
        if(pokerGameManager.IsDragging) return; // ignore mouse if already holding a card
        pokerBase.Modulate = new Color(pokerBase.Modulate, 1f);
        pokerGameManager.UnHoveringPoker?.Invoke(pokerBase);
    }
}
