using Godot;
using System;
using Godot.Collections;

public partial class ICardOrganizer : Node //helper interface for organizing poker position
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
        var anchorPos = organizeArea.GlobalPosition;
        var startPosX = anchorPos.X - pokerArrayRef.Count * pokerSpacing/2 + pokerSpacing/4;
        var startPosY = anchorPos.Y;
        if (pokerArrayRef is null || pokerArrayRef.Count == 0) return; //avoid edge cases of empty array
        for (var i = 0; i < pokerArrayRef.Count; i++)
        {
            var poker = pokerArrayRef[i];
            poker.SetPosition(new Vector2(startPosX + pokerSpacing*i, startPosY));
        }
    }   
}
