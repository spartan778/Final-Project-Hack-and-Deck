using Godot;
using System;

public interface IBumpAttacker
{
    BumpAttackModule BumpAttackModule { get; set; }
}
public partial class BumpAttackModule : Node
{
    [Export] private EnemyBase enemy;
    [Export] private Area2D bumpArea;
    [Export] private Timer bumpCoolDownTimer;
    
    [Export] public float Damage;
    private bool onCoolDown;

    public override void _Ready()
    {
        if (bumpArea is null || bumpCoolDownTimer is null)
        {
            GD.PrintErr("Basic export assignment failed");
            return;
        }
        onCoolDown = false;
        bumpCoolDownTimer.Timeout += CoolDownEnded;
        bumpArea.AreaEntered += OnAreaEntered;
        Damage = enemy.CurrentDamage;
    }

    private void OnAreaEntered(Area2D area)
    {
        if(onCoolDown) return;
        if (area is IDamageable target)
        {
            target.TakeDamage(Damage);
            GD.Print($"{target.HealthSystem.GetParent().Name} has been bumped");
            onCoolDown = true;
            bumpCoolDownTimer.Start();
        }
    }
    private void CoolDownEnded()
    {
        onCoolDown = false;
        GD.Print($"{enemy.Name}: Cool Down Ended");
    }
}
