using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class PokerArray : Resource
{
    [Export] public Array<PokerInfo> SavedPokers;
}
