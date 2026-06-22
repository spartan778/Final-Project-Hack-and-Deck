using Godot;
using System;

public partial class CardSlotControl : Control // mostly a dummy control node to allow it to work with HBoxContainer
{
    [Export] public CardSlotBase CardSlotBase { get; private set;}
}
