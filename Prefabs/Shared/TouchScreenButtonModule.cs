using Godot;
using System;

[GlobalClass]
public partial class TouchScreenButtonModule : Node // component-like module to make UI buttons work with touch input
{
    private BaseButton button;

    public override void _Ready()
    {
        button = GetParent<BaseButton>();
        if (button is null)
        {
            GD.PrintErr("Target Button not found\n" +
                        "Parent Node should be a Button");
            return;
        }

        button.GuiInput += OnTouchScreenButtonPressed; // subscribe to build-in GUI input event
    }

    private void OnTouchScreenButtonPressed(InputEvent touchEvent)
    {
        if (@touchEvent is InputEventScreenTouch screenTouch && screenTouch.IsPressed())
        {
            button.EmitSignal(BaseButton.SignalName.Pressed); // shorthand(string) for "pressed" signal
            button.AcceptEvent(); // mark as handled event
        }
    }
}
