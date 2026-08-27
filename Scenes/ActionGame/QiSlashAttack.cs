using Godot;
using System;

public partial class QiSlashAttack : Node2D
{
    [Export] private PlayerManager playerManagerRef;
    [Export] private AnimatedSprite2D slashAnimation;
    [Export] private Area2D hitArea;
    [Export] private float detectDuration = 0.5f;
    [Export] private float defaultDamage = 20f;
    public float CurrentDamage;
    public bool IsAttackReady { get; private set; }
    public float DefaultAttackCooldown  { get; private set; } = 1f;
    public float CurrentAttackCooldown;
    
    

    public override void _Ready()
    {
        CurrentAttackCooldown = DefaultAttackCooldown;
        IsAttackReady = true;
        slashAnimation.Visible = false;
        hitArea.Monitorable = false;
        hitArea.Monitoring = false;
        CurrentDamage = defaultDamage;
        hitArea.AreaEntered += OnSlashHit;
        slashAnimation.AnimationFinished += slashAnimation.Hide; // hide sprites when animation is finished
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(playerManagerRef.PlayerForm != PlayerForm.Defensive) return; // ignore if not in Defensive Mode
        if (@event.IsActionPressed("action_activeAttack") && IsAttackReady)
        {
            MakeSlashAttack();
        }
    }

    public async void MakeSlashAttack()
    {
        GD.Print("Making Slash Attack");
        IsAttackReady = false;
        var direction = playerManagerRef.GetMouseToPlayerVector();
        hitArea.Rotation = direction.Angle();
        slashAnimation.Visible = true;
        hitArea.Monitoring = true;
        
        slashAnimation.Play();
        await ToSignal(GetTree().CreateTimer(CurrentAttackCooldown), SceneTreeTimer.SignalName.Timeout);
        IsAttackReady = true;
        slashAnimation.Visible = false;
        hitArea.Monitoring = false;
    }

    private void OnSlashHit(Area2D area)
    {
        if(area is not (IDamageable or IBlockable) ) return; // check if target is (either) bullet or enemies
        switch (area)
        {
            // collision is already set at layer 2 to detect enemies 
            case IDamageable damageable:
            {
                if(damageable is PlayerHitbox playerHitbox) return; // slash attack should not hit player
                if (damageable is EnemyHitbox enemyHitbox)
                {
                    enemyHitbox.TakeDamage(CurrentDamage);
                }
                break;
            }
            case IBlockable blockable:
            {
                if(blockable.IsFromPlayer) return; // slash attack should not hit player bullets
                if (blockable.IsFromEnemy && blockable.IsBlockable)
                {
                    blockable.Blocked(); // slash attack will "block" enemy attacks
                }
                break;
            }
        }
    }
}
