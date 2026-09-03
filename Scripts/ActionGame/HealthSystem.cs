using Godot;
using System;

[GlobalClass]
public partial class HealthSystem : Node
{
    [Export] public float MaxHealth { get; private set; }
    public Action HealthChanged;
    [Signal] 
    public delegate void DamageEffectEventHandler(float oldHealth, float newHealth); // special event (signal) type for gdScript
    [Signal]
    public delegate void HealingEffectEventHandler(float healedValue);
    [Export] public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            
            if (Mathf.Abs(currentHealth - value) < 0.01f) { // avoid floating point precision bug
                return;
            }
            currentHealth = value;
            HealthChanged?.Invoke();
        } 
    }
    
    private float currentHealth;
    [Export] public ProgressBar HealthBar { get; private set; }

    public Action<float> TakingDamage, Healing;
    public Action Dying;
    

    public override void _Ready()
    {
        ConnectSignals();
        
        CurrentHealth = MaxHealth;
    }

    private void ConnectSignals()
    {
        HealthChanged += OnHealthChanged;
    }

    public void InitHealthSystem(float maxHealth, float newHealth = -1f)
    {
        MaxHealth = maxHealth;
        if (newHealth < 0)
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
        EmitSignal(SignalName.DamageEffect, (currentHealth + amount), currentHealth);
        // GD.Print($"{GetParent().Name}:Taking damage: {amount}");
        if (CurrentHealth <= 0)
        {
            Dying?.Invoke();
            // GD.Print($"Dying");
        }
    }
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        Healing?.Invoke(amount);
        // GD.Print($"{GetParent().Name}:Healing: {amount}");
        EmitSignalHealingEffect(amount); // godot generated (c#) helper
    }

    private void OnHealthChanged()
    {
        if (HealthBar is null) return;
        HealthBar.Value = currentHealth/MaxHealth * 100f;
    }
}
