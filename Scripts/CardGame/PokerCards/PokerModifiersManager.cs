using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class PokerModifiersManager : Node
{
    public PokerState PokerState;
    public PokerType PokerType;
    
    [Export] private AnimatedSprite2D cardDisplayRef;
    [Export] private Material shaderMaterial;
    

    public void SetPokerState(PokerState pokerState)
    {
        PokerState = pokerState;
        UpdatePokerStateDisplay();
    }

    private void UpdatePokerStateDisplay()
    {
        switch (PokerState)
        {
            case PokerState.Normal:
            {
                cardDisplayRef.Material = null;
                break;
            }
            case PokerState.Inversed:
            {
                cardDisplayRef.Material = shaderMaterial;
                break;
            }
            default:
            {
                break;
            }
        }
    }
public Dictionary ToDictionary()
    {
        var enumDict = new Godot.Collections.Dictionary
        {
            { nameof(PokerState), (int)PokerState },
            { nameof(PokerType), (int)PokerType },
        };
        GD.Print(enumDict);
        return enumDict;
    }
}

