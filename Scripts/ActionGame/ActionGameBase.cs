using Godot;
using System;

public partial class ActionGameBase : Node // mostly used as a place to store reference
{
    [Export] public PlayerManager PlayerManagerRef { get; private set; }
    [Export] public BulletManager BulletManagerRef { get; private set; }
    
    public static ActionGameBase Instance { get; private set; }
    
    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }
}
