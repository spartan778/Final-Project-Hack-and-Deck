using Godot;
using System;

public partial class HealthSystem : Node
{
    [Export] public float MaxHealth { get; private set; }
    [Export] public float CurrentHealth { get; private set; }

    public Action<float> TakingDamage, Healing;
    public Action Dying;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }

    public void InitHealthSystem(float maxHealth, float currentHealth = -1f)
    {
        MaxHealth = maxHealth;
        if (currentHealth < 0)
        {
            currentHealth = maxHealth;
        }
    }
    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
    }
    
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        TakingDamage?.Invoke(amount);
        GD.Print($"Taking damage: {amount}");
        if (CurrentHealth <= 0)
        {
            Dying?.Invoke();
            GD.Print($"Dying");
        }
    }
    
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        Healing?.Invoke(amount);
    }
}
