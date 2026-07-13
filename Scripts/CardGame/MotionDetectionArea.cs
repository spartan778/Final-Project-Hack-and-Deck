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
    public override void _Ready()
    {
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    private void OnMouseEntered()
    {
        isHovered = true;
    }
    private void OnMouseExited()
    {
        isHovered = false;
    }
    
    
    public override void _Input(InputEvent @event)
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
    
    private IEnumerator TrackSwipeCoroutine()
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
