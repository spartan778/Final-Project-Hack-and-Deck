using Godot;
using System;

public partial class PokerPhysics : Area2D
{
    [Export] private PokerBase pokerBaseRef;
    
    private bool isDragging = false;
    private Vector2 moveOffset;
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left }) // specifically check for left mouse button changes
        {
            if (@event.IsPressed())
            {
                isDragging = true;
                moveOffset = pokerBaseRef.GlobalPosition - GetGlobalMousePosition();
                // GetParent().GetParent().MoveChild(this, -1);
                GetViewport().SetInputAsHandled(); // avoid event fallthrough
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!isDragging) return;
        if (@event is InputEventMouseMotion)
        {
            pokerBaseRef.GlobalPosition = GetGlobalMousePosition() + moveOffset;
            
        }
        else if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left })
        {
            if (!@event.IsPressed())
            {
                isDragging = false;
            }

            GetViewport().SetInputAsHandled();
        }
    }
}
