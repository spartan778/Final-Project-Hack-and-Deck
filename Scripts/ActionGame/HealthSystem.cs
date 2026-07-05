using Godot;
using System;

public partial class HealthSystem : Node
{
    [Export] public float MaxHealth { get; private set; }
    [Export] public float CurrentHealth { get; private set; }


    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }
    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
    }
    
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
    }
    
    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }
}
