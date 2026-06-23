using Godot;
using System;

public partial class DeckArea : Area2D
{
    private Node2D deckParent;
    private bool isHovered = false;

    public Action IsClicked;
    public override void _Ready()
    {
        deckParent = GetParent<Node2D>();
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
        if (!isHovered) return;
        switch (@event)
        {
            case InputEventMouseButton { ButtonIndex: MouseButton.Right }:
            {
                if(@event.IsPressed())
                {
                    IsClicked?.Invoke();
                }
                break;
            }
        }
    }
}
