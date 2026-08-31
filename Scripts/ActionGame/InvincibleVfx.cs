using Godot;
using System;

public partial class InvincibleVfx : Node2D
{
    [Export] private AnimatedSprite2D playerSprite;
    [Export] private PlayerHitbox playerHitboxRef;

    public override void _Ready()
    {
        playerHitboxRef.InvincibilityStateChanged += OnInvincibilityStateChanged;

        void OnInvincibilityStateChanged(bool isInvincible) // change player transparency depending on Invincibility
        {
            if (isInvincible)
            {
                var modulate = playerSprite.Modulate;
                modulate.A = .6f;
                playerSprite.Modulate = modulate;
            }
            else
            {
                var modulate = playerSprite.Modulate;
                modulate.A = 1f;
                playerSprite.Modulate = modulate;
            }
        }
    }
}
