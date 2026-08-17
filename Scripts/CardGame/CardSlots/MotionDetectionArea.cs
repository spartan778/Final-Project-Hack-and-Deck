using Godot;
using System;
using System.Collections;
using HCoroutines;

public partial class MotionDetectionArea : Area2D
{
    [Export] public CardSlots CardSlots { get; private set; }
    [Export] public PokerGameManager PokerGameManagerRef { get; private set; }
    [Export] private float swipeDistance;
    [Export] private TextureProgressBar swipeProgress;
    private bool isHovered, isPressed = false;
    private Coroutine currentSwipeCoroutine;
    private Vector2 startPosition, endPosition;
    private int touchIndex;
    public override void _Ready()
    {
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        // MouseEntered += OnMouseEntered; // disabling raw mouse input for testing
        // MouseExited += OnMouseExited;
    }
    private void OnMouseEntered()
    {
        isHovered = true;
    }
    private void OnMouseExited()
    {
        isHovered = false;
    }
    
    
    public override void _Input(InputEvent @event) // legacy method used by mouse input
    {
        if(PokerGameManagerRef.IsDragging) return; // only detect if not dragging poker
        if(CardSlots.IsInCoolDown) return; // only detect if cardSlots are not in cooldown
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed && !isPressed && isHovered) //must start in detection area
            {
                startPosition = GetGlobalMousePosition();
                GD.Print($"Starting swipe at position {startPosition}");
                currentSwipeCoroutine = Co.Run(TrackSwipeCoroutine);
                isPressed = true;
            }
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.IsReleased())
            {
                isPressed = false;
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event) // later learnt that _Input should only be used for UI level input
    {
        if(PokerGameManagerRef.IsDragging) return; // only detect if not dragging poker
        if(CardSlots.IsInCoolDown) return; // only detect if cardSlots are not in cooldown
        if (@event is InputEventScreenTouch screenTouch)
        {
            if ( screenTouch.Pressed && IsPosInsideArea(screenTouch.Position) )
            {
                startPosition = screenTouch.Position;
                GD.Print($"Starting (touch) swipe at position {startPosition}");
                isPressed = true;
                touchIndex = screenTouch.Index;
                startPosition = screenTouch.Position;
            }
            if (screenTouch.Index == touchIndex && !screenTouch.Pressed && isPressed)
            {
                GD.Print("Swipe cancelled (released)");
                swipeProgress.Value = 0;
                isPressed = false;
            }
        }
        if(!isPressed) return; // no need to track touch motion if the area is not pressed
        if (@event is InputEventScreenDrag screenDrag)
        {
            var currentTouchPosition = screenDrag.Position;
            var distance = currentTouchPosition - startPosition;
            swipeProgress.Value = (distance.X / swipeDistance)*100;
            if (distance.X > swipeDistance)
            {
                GD.Print("Swipe successful(Touch)");
                CardSlots.SwipeSuccess?.Invoke();
            }
        }
    }
    
    private bool IsPosInsideArea(Vector2 pos) // raycast to detect if touch hits the interaction area
    {
        var spaceState = GetWorld2D().DirectSpaceState; // get reference to the physical state
        var query = new PhysicsPointQueryParameters2D // set raycast to detect Area2D ONLY
        {
            Position = pos,
            CollideWithAreas = true,
            CollideWithBodies = false
        };
        var results = spaceState.IntersectPoint(query); // the actual raycast query

        foreach (var result in results)
        {
            if (result["collider"].As<Node>() == this) // find node from collider
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator TrackSwipeCoroutine() // legacy method used by mouse input
    {
        while (isPressed)
        {
            endPosition = GetGlobalMousePosition();
            var delta = endPosition - startPosition;
            swipeProgress.Value = (delta.X / swipeDistance)*100;
            if (delta.X > swipeDistance)
            {
                GD.Print("Swipe successful");
                CardSlots.SwipeSuccess?.Invoke();
                yield break;
            }
            GD.Print($"Swiped distance {delta.X}");
            yield return null;
        }
        GD.Print("stopped swiping");
        isPressed = false;
        swipeProgress.Value = 0;
    }
}

public enum InputMotions
{
    Swipe,
    Pinch
}
