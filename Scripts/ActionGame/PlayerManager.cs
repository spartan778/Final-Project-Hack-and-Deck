using Godot;
using System;

public partial class PlayerManager : CharacterBody2D
{
    [Export] public PlayerForm PlayerForm { get; private set; }
    private EnemySystem enemySystemRef;
    public static PlayerManager Instance { get; private set; }
    private ActionGameBase actionGameBase;
    private BulletManager bulletManager;
    

    public Action<PlayerForm> PlayerFormChanged;
    public Action<bool> SettingAllowPlayerInput;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public override void _Ready()
    {
        SettingAllowPlayerInput?.Invoke(true); // making sure input is allowed when game is ready
        SetPlayerForm(PlayerForm); // set default mode for player
        actionGameBase = ActionGameBase.Instance;
        bulletManager = actionGameBase.BulletManagerRef;
        enemySystemRef = EnemySystem.Instance;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("action_switch_form")) // handle form switching
        {
            TogglePlayerForm();
        }
    }

    

    public Vector2 GetMouseToPlayerVector() // reusable method for getting vector (direction) between player and mouse
    {
        var rawVector = GetGlobalMousePosition() - GlobalPosition;
        return rawVector.Normalized();
    }
    public Vector2 GetTargetToPlayerVector(Node2D target)
    {
        var rawVector = target.GlobalPosition - GlobalPosition;
        return rawVector.Normalized();
    }
    public Vector2 GetTargetToPlayerVector(Vector2 targetPos)
    {
        var rawVector = targetPos - GlobalPosition;
        return rawVector.Normalized();
    }
    
    public void SetPlayerForm(PlayerForm playerForm)
    {
        PlayerForm = playerForm;
        GD.Print($"Set to {PlayerForm}");
        PlayerFormChanged?.Invoke(playerForm);
    }
    public void TogglePlayerForm()
    {
        if (PlayerForm == PlayerForm.Defensive)
        {
            SetPlayerForm(PlayerForm.Aggressive);
        }
        else
        {
            SetPlayerForm(PlayerForm.Defensive);
        }
    }
}

public enum PlayerForm
{
    Defensive,
    Aggressive
}
