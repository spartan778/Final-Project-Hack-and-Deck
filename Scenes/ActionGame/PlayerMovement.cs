using Godot;
using System;
using System.Collections;
using HCoroutines;

public partial class PlayerMovement : Node //testing for component style coding
{
    [Export] private PlayerManager playerManagerRef;
    
    [Export] public float Speed { get; private set; }
    [Export] public float DashSpeed { get; private set; }
    
    private PlayerForm playerForm;
    private bool isAllowPlayerInput, isAllowMovement, isDashing;

    public override void _Ready()
    {
        ConnectSignals();
        playerForm = playerManagerRef.PlayerForm;
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
            playerManagerRef.Velocity = direction * Speed;
        }
        else
        {
            // Stop moving when no keys are pressed
            playerManagerRef.Velocity = Vector2.Zero; 
        }
        // Move the character and handle collisions
        playerManagerRef.MoveAndSlide();
    }
    private void MovementSkillProcess()
    {
        switch (playerForm)
        {
            case PlayerForm.Defensive:
            { 
                Co.Run(DashCoroutine());
                break;
            }
            case PlayerForm.Aggressive:
            {
                if(playerManagerRef.Velocity == Vector2.Zero) break; // can not roll if not moving
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
        GD.Print(dashVector);
        playerManagerRef.Velocity = dashVector * DashSpeed;
        GD.Print(playerManagerRef.Velocity);
        // playerManagerRef.MoveAndSlide();
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
        // playerManagerRef.MoveAndSlide();
        yield return Co.Wait(duration);
        isAllowPlayerInput = true;
        isDashing = false;
    }
    private void OnPlayerFormChange(PlayerForm form)
    {
        playerForm = form;
    }

    private void OnSettingAllowPlayerInput(bool value)
    {
        isAllowPlayerInput =  value;
        isAllowMovement = value;
    }
}
