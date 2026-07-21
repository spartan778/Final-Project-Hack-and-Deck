using Godot;
using System;
using System.Collections;

public partial class PokerSummonManager : Node
{
    [Export] private PokerBase pokerBaseRef;
    [Export] private ColorRect summonEffect;
    private float tickInterval = 0.05f;

    public override void _Ready()
    {
        pokerBaseRef.PokerSummoned += OnPokerSummoned;
    }

    private void OnPokerSummoned()
    {
        summonEffect.Visible = true;
    }
}
