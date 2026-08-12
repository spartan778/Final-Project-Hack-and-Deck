using Godot;
using System;

public partial class PokerDragging : Area2D
{
	[Export] public PokerBase PokerBaseRef;
	private PokerGameManager pokerGameManager;
	
	public bool IsDragging = false;
	public Vector2 PickUpOffset;

	public override void _Ready()
	{
		pokerGameManager = CardGameHelperSingleton.Instance.PokerGameManager;
	}

	/*public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) // refactored with raycast
	{
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left }) // specifically check for left mouse button changes
		{
			if (@event.IsPressed())
			{
				IsDragging = true;
				PickUpOffset = PokerBaseRef.GlobalPosition - GetGlobalMousePosition(); // store the offset between mouse and center point of card
				
				pokerGameManager.HoldingPoker?.Invoke(PokerBaseRef);
				GetViewport().SetInputAsHandled(); // avoid event fallthrough
			}
		}
	}*/

	public override void _Input(InputEvent @event)
	{
		if (!IsDragging) return;
		switch (@event)
		{
			/*case InputEventMouseMotion: // triggered everytime(frame) when there is mouse motion
			{
				var finalMousePosition = CardGameHelperSingleton.Instance.CheckScreenBoundaries(GetGlobalMousePosition());
				PokerBaseRef.GlobalPosition = finalMousePosition + PickUpOffset; // offset the card to avoid snapping to card center
				break;
			}
			case InputEventMouseButton { ButtonIndex: MouseButton.Left }:
			{
				if (@event.IsReleased())
				{
					IsDragging = false;
					pokerGameManager.ReleasingPoker?.Invoke(PokerBaseRef);
				}
				GetViewport().SetInputAsHandled();
				break;
			}*/
			
			case InputEventScreenDrag dragEvent: // same feature as "InputEventMouseMotion"
			{
				// var finalDragPosition = CardGameHelperSingleton.Instance.CheckScreenBoundaries(GetGlobalMousePosition());
				var finalDragPosition = CardGameHelperSingleton.Instance.CheckScreenBoundaries(dragEvent.Position);
				PokerBaseRef.GlobalPosition = finalDragPosition + PickUpOffset;
				break;
			}
			case InputEventScreenTouch touchEvent: // same feature as "InputEventMouseButton"
			{
				if (touchEvent.IsReleased() && touchEvent.Index == 0)
				{
					IsDragging = false;
					pokerGameManager.ReleasingPoker?.Invoke(PokerBaseRef);
				}
				GetViewport().SetInputAsHandled();
				break;
			}
		}
	}
}
