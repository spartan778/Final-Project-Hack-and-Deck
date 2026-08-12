using Godot;
using System;

[GlobalClass]
public partial class TouchScreenButtonModule : Node
{
    private BaseButton button;

    public override void _Ready()
    {
        button = GetParent<BaseButton>();
        if (button is null)
        {
            GD.PrintErr("Button not found");
            return;
        }

        button.GuiInput += OnTouchScreenButtonPressed;
    }

    private void OnTouchScreenButtonPressed(InputEvent touchEvent)
    {
        if (@touchEvent is InputEventScreenTouch screenTouch && screenTouch.IsPressed())
        {
            button.EmitSignal(BaseButton.SignalName.Pressed);
            button.AcceptEvent();
        }
    }
}
