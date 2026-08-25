using Godot;
using System;

public partial class MovementAnimation : Node
{
    [Export] private PlayerMovement playerMovement;
    [Export] private AnimatedSprite2D playerAnimatedSprite;

    public override void _Ready()
    {
        playerMovement.ChangeFacingDirection += OnChangingFacingDirection;
        playerAnimatedSprite.Animation = "idle_front";
        playerAnimatedSprite.Play();
    }

    private void OnChangingFacingDirection(Vector2 newDirection)
    {
        switch (newDirection)
        {
            case { X: > 0 }: // will catch all diagonal movement to right (normalized vector with have an x value less than 1)
            {
                playerAnimatedSprite.Animation = "move_right";
                playerAnimatedSprite.Play();
                break;
            }
            case { X: < 0 }: // will catch all diagonal movement to left
            {
                playerAnimatedSprite.Animation = "move_left";
                playerAnimatedSprite.Play();
                break;
            }
            case { X: 0, Y: 0 }:
            {
                playerAnimatedSprite.Animation = "idle_front";
                playerAnimatedSprite.Play();
                break;
            }
            case { X: 0, Y: -1 }:
            {
                playerAnimatedSprite.Animation = "move_back";
                playerAnimatedSprite.Play();
                break;
            }
            case { X: 0, Y: 1 }:
            {
                playerAnimatedSprite.Animation = "move_front";
                playerAnimatedSprite.Play();
                break;
            }
        }
    }
}
