using Godot;
using System;

public partial class PlayerManager : CharacterBody2D
{
    [Export] public PlayerForm PlayerForm { get; private set; }
    private ActionGameBase actionGameBase;
    private BulletManager bulletManager;
    

    public Action<PlayerForm> PlayerFormChanged;
    public Action<bool> SettingAllowPlayerInput;
    

    public override void _Ready()
    {
        SettingAllowPlayerInput?.Invoke(true); // make sure input is allowed when game is ready
        actionGameBase = ActionGameBase.Instance;
        bulletManager = actionGameBase.BulletManagerRef;
    }

    public Vector2 GetMouseToPlayerVector() // reusable method for getting vector (direction) between player and mouse
    {
        var rawVector = GetGlobalMousePosition() - GlobalPosition;
        return rawVector.Normalized();
    }
}

public enum PlayerForm
{
    Defensive,
    Aggressive
}
