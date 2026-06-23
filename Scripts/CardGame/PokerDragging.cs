using Godot;
using System;

public partial class PokerDragging : Area2D
{
	[Export] public PokerBase PokerBaseRef;
	private PokerGameManager pokerGameManager;
	
	private bool isDragging = false;
	private Vector2 pickUpOffset;

	public override void _Ready()
	{
		pokerGameManager = CardGameHelperSingleton.Instance.PokerGameManager;
		GD.Print("PokerDragging");
	}

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left }) // specifically check for left mouse button changes
		{
			if (@event.IsPressed())
			{
				isDragging = true;
				pickUpOffset = PokerBaseRef.GlobalPosition - GetGlobalMousePosition(); // store the offset between mouse and center point of card
				// GetParent().GetParent().MoveChild(this, -1);
				pokerGameManager.HoldingPoker?.Invoke(PokerBaseRef);
				GetViewport().SetInputAsHandled(); // avoid event fallthrough
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (!isDragging) return;
		switch (@event)
		{
			case InputEventMouseMotion: // triggered everytime(frame) when there is mouse motion
			{
				var finalMousePosition = CardGameHelperSingleton.Instance.CheckScreenBoundaries(GetGlobalMousePosition());
				PokerBaseRef.GlobalPosition = finalMousePosition + pickUpOffset; // offset the card to avoid snapping to card center
				break;
			}
			case InputEventMouseButton { ButtonIndex: MouseButton.Left }:
			{
				if (!@event.IsPressed() || @event.IsReleased())
				{
					isDragging = false;
					pokerGameManager.ReleasingPoker?.Invoke(PokerBaseRef);
				}
				GetViewport().SetInputAsHandled();
				break;
			}
		}
	}
}
