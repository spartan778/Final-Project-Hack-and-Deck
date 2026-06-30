using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class PokerModifiersManager : Node
{
    public PokerState PokerState;
    public PokerType PokerType;

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

