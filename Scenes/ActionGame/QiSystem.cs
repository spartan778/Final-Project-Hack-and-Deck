using Godot;
using System;

public partial class QiSystem : Node2D
{
    [Export] private PlayerManager playerManagerRef;
    [Export] public float MaxQi { get; private set; } = 300f;
    [Export] public float DefaultQiAttackCost { get; private set; } = 100f;
    public float CurrentQiAttackCost;
    [Export] public float DefaultQiRegen { get; private set; } = 5f;
    public float CurrentQiRegen;
    [Export] public float CurrentQi = 100f;
    
    public float QiBaseRegenModifier = 1f;
    public float QiRegenBonusModifier = 0f;
    
    [Export] private QiBlastAttack qiBlastAttack;
    [Export] private QiSlashAttack qiSlashAttack;

    public override void _Ready()
    {
        CurrentQiAttackCost = DefaultQiAttackCost;
        CurrentQiRegen = DefaultQiRegen;
        playerManagerRef.PlayerFormChanged += OnPlayerFormChanged;
    }

    private void OnPlayerFormChanged(PlayerForm playerForm)
    {
        QiBaseRegenModifier = (playerForm == PlayerForm.Aggressive) ? 1f : 2f;
    }

    public override void _Process(double delta)
    {
        QiRegenProcess(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("action_activeAttack")) return; 
        if(CurrentQi < CurrentQiAttackCost)
        {
            GD.Print("not enough Qi");
            return;
        }
        switch (playerManagerRef.PlayerForm)
        {
            case PlayerForm.Aggressive:
                qiBlastAttack.MakeQiBlast();
                CurrentQi -= CurrentQiAttackCost;
                break;
            case PlayerForm.Defensive:
            {
                if(qiSlashAttack.IsAttackReady)
                {
                    qiSlashAttack.MakeSlashAttack();
                    CurrentQi -= CurrentQiAttackCost;
                }
                break;
            }
        }
    }

    private void QiRegenProcess(double delta)
    {
        if(CurrentQi >= MaxQi) return;
        CurrentQi += CurrentQiRegen * (QiBaseRegenModifier + QiRegenBonusModifier) * (float)delta;
    }

    public bool HasEnoughQi(float value)
    {
        return CurrentQi >= value;
    }
    public void ConsumeQi(float value)
    {
        CurrentQi -= value;
    }
    
    public void SetMaxQi(float value)
    {
        MaxQi = value;
    }
}
