using Godot;
using System;
using Godot.NativeInterop;

public partial class mobileTextField : LineEdit
{
    public override void _Ready()
    {
        GuiInput += OnGuiInput;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touchEvent && touchEvent.Pressed)
        {
            GrabFocus(); // simulate the "focus" state from mouse input
            DisplayServer.VirtualKeyboardShow(""); // show mobile keyboard
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (HasFocus() && @event is InputEventScreenTouch touchEvent && touchEvent.Pressed)
        {
            
            Rect2 inputArea = GetGlobalRect(); // Get the area(rect) of this LineEdit
            
            if (!inputArea.HasPoint(touchEvent.Position)) // if touch position is outside of
            {
                ReleaseFocus(); // simulate the "unfocus" state from mouse input
                DisplayServer.VirtualKeyboardHide();
            }
        }
    }
}
