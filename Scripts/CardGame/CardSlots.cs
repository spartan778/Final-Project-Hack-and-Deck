using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class CardSlots : Node2D
{
    [Export] public Array<CardSlotControl> CardSlotControls { get; private set;}
    
}
