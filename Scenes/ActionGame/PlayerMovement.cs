using Godot;
using System;
using System.Collections;
using HCoroutines;

public partial class PlayerMovement : Node //testing for component style coding
{
    [Export] private PlayerManager playerManagerRef;
    [Export] private QiSystem qiSystemRef;
    
    [Export] public float Speed { get; private set; }
    [Export] public float DashSpeed { get; private set; }
    [Export] public float DefaultDashDuration { get; private set; } = 0.5f;
    public float CurrentDashDuration { get; private set; }
    [Export] public float DefaultDashCost { get; private set; } = 50f;
    public float CurrentDashCost;
    [Export]public float MovementSkillCoolDown { get; private set; } = 10f;
    [Export] public float DefenceFormSpeedMod { get; private set; } = 1.5f;
    [Export] public float AggressiveFormSpeedMod { get; private set; } = 1f;

    public Action<Vector2> ChangeFacingDirection;

    public Action<float> StartDashing;
    public Vector2 CurrentFacingDirection { get; private set; }
    
    private PlayerForm playerForm;
    private bool isAllowPlayerInput, isAllowMovement, isDashing;
    private float currentSpeedMod = 1;
    

    public override void _Ready()
    {
        ConnectSignals();
        playerForm = playerManagerRef.PlayerForm;
        CurrentFacingDirection = Vector2.Zero;
        ChangeFacingDirection?.Invoke(CurrentFacingDirection);
        CurrentDashCost = DefaultDashCost;
    }

    public void ConnectSignals()
    {
        playerManagerRef.PlayerFormChanged += OnPlayerFormChange;
        playerManagerRef.SettingAllowPlayerInput += OnSettingAllowPlayerInput;
    }
    public override void _PhysicsProcess(double delta)
    {
        MovementProcess();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("action_dash"))
        {
            MovementSkillProcess();
        }
    }
    private void MovementProcess()
    {
        if(!isAllowMovement) return; // stop if input is not allowed
        if (isDashing)
        {
            playerManagerRef.MoveAndSlide();
            return;
        }
        // Referenced from Godot Engine 4.7 documentation (official)
        
        Vector2 direction = Vector2.Zero; // init movement direction as (0,0)
        // Get user input
        // GetVector automatically normalizes the output, preventing faster diagonal movement.
        if(isAllowPlayerInput)
        {
            direction = Input.GetVector("action_left", "action_right", "action_up", "action_down");
        }
        // Apply movement
        if (direction != Vector2.Zero)
        {
            playerManagerRef.Velocity = direction * Speed * currentSpeedMod;
        }
        else
        {
            // Stop moving when no keys are pressed
            playerManagerRef.Velocity = Vector2.Zero; 
        }
        // Move the character and handle collisions
        playerManagerRef.MoveAndSlide();
        UpdateCurrentFacingDirection(direction);
    }

    private void UpdateCurrentFacingDirection(Vector2 newDirection)
    {
        if (CurrentFacingDirection != newDirection)
        {
            CurrentFacingDirection = newDirection;
            ChangeFacingDirection?.Invoke(CurrentFacingDirection);
        }
    }
        
    private void MovementSkillProcess()
    {
        switch (playerForm)
        {
            case PlayerForm.Defensive:
            {
                if(!qiSystemRef.HasEnoughQi(CurrentDashCost)) return;
                qiSystemRef.ConsumeQi(CurrentDashCost);
                Co.Run(DashCoroutine());
                break;
            }
            case PlayerForm.Aggressive:
            {
                if(playerManagerRef.Velocity == Vector2.Zero) break; // can not roll if not moving
                if(!qiSystemRef.HasEnoughQi(CurrentDashCost)) return;
                qiSystemRef.ConsumeQi(CurrentDashCost);
                Co.Run(RollCoroutine());
                break;
            }
        }
    }

    private IEnumerator DashCoroutine(float duration = 0.5f)
    {
        isAllowPlayerInput = false;
        isDashing = true;
        var dashVector = playerManagerRef.GetMouseToPlayerVector();
        playerManagerRef.Velocity = dashVector * DashSpeed;
        StartDashing?.Invoke(duration);
        yield return Co.Wait(duration);
        isAllowPlayerInput = true;
        isDashing = false;
    }

    private IEnumerator RollCoroutine(float duration = 0.5f)
    {
        isAllowPlayerInput = false;
        isDashing = true;
        var rollVector = playerManagerRef.Velocity.Normalized();
        playerManagerRef.Velocity = rollVector * DashSpeed;
        yield return Co.Wait(duration);
        isAllowPlayerInput = true;
        isDashing = false;
    }
    private void OnPlayerFormChange(PlayerForm form)
    {
        playerForm = form;
        switch (playerForm)
        {
            case PlayerForm.Defensive:
            {
                currentSpeedMod = DefenceFormSpeedMod;
                break;
            }
            case PlayerForm.Aggressive:
            {
                currentSpeedMod = AggressiveFormSpeedMod;
                break;
            }
        }
    }

    public void ChangeFormSpeedMod(PlayerForm targetForm, float value)
    {
        switch (targetForm)
        {
            case PlayerForm.Defensive:
            {
                DefenceFormSpeedMod = value;
                break;
            }
            case PlayerForm.Aggressive:
            {
                AggressiveFormSpeedMod = value;
                break;
            }
        }
        if (targetForm == playerManagerRef.PlayerForm) // speed mod should apply immediately if the form matches
        {
            currentSpeedMod = value;
        }
    }
    private void OnSettingAllowPlayerInput(bool value)
    {
        isAllowPlayerInput =  value;
        isAllowMovement = value;
    }

    public void SetCurrentDashDuration(float value)
    {
        CurrentDashDuration = value;
    }
}
