using Godot;
using System;

public partial class CoolDownProgress : TextureProgressBar
{
    [Export]private Timer coolDownTimer;
    
    public override void _Ready()
    {
        Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!coolDownTimer.IsStopped())
        {
            var waitTime = coolDownTimer.WaitTime;
            var timePassed = coolDownTimer.TimeLeft;
            Value = (1 - timePassed / waitTime) * 100;
            // GD.Print(Value);
        }
        else
        {
            Visible = false;
        }
    }
}
