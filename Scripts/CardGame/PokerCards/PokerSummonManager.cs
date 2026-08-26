using Godot;
using System;
using System.Collections;

public partial class PokerSummonManager : Node
{
    [Export] private ColorRect summonEffect;
    private float tickInterval = 0.05f;

    public override void _Ready()
    {
        
    }

    private void OnPokerSummoned()
    {
        summonEffect.Visible = true;
    }

    public void SetShowSummonEffect(bool value)
    {
        summonEffect.Visible = value;
    }
}
