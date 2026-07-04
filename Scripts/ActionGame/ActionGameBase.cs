using Godot;
using System;

public partial class ActionGameBase : Node
{
    [Export] public PlayerManager PlayerManagerRef { get; private set; }
    [Export] public BulletManager BulletManagerRef { get; private set; }
}
