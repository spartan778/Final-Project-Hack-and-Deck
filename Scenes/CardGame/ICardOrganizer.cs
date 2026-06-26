using Godot;
using System;
using Godot.Collections;

public partial class ICardOrganizer : Node
{
    [Export] private Sprite2D organizeArea;
    [Export] private HandSlots handSlots;
    [Export] private float pokerSpacing;

    private Vector2 PanelAreaSize => organizeArea.RegionRect.Size;
    private Array<PokerBase> pokerArrayRef;

    public override void _Ready()
    {
        pokerArrayRef = handSlots.Pokers;
    }
    
    public void OrganizePokers()
    {
        
    }
}
