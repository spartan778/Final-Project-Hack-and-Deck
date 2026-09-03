using Godot;
using System;
using System.Threading.Tasks;
using Godot.Collections;

public partial class ChainLightening : Node2D
{
    public static Vector2 SpriteSize = new(64, 160);
    private float defaultRotationOffset = -90;
    [Export] private AnimatedSprite2D lightningSprite;
    [Export] private Area2D chainRange;
    [Export] public float DefaultDamage { get; private set; }
    public float CurrentDamage { get; private set; }
    [Export] public int DefaultChainAmount { get; private set; }
    [Export] private Node2D nextTarget;
    public int ChainAmount;

    
    private Array<EnemyBase> pastTarget;


    public override void _Ready()
    {
        CurrentDamage = DefaultDamage;
        ChainAmount = DefaultChainAmount;
        pastTarget = new Array<EnemyBase>();
    }
    
    private void AttackTarget(EnemyBase target)
    {
        if(target is null) return; // avoid case where target is deleted during attack
        target.HealthSystem.TakeDamage(DefaultDamage);
        pastTarget.Add(target);
        GlobalPosition = target.GetGlobalPosition();
        
    }
    private void ShowLighteningEffect(Node2D source ,Node2D target)
    {
        var tempEffect = (AnimatedSprite2D)lightningSprite.Duplicate();
        tempEffect.AnimationFinished += tempEffect.QueueFree;
        GetTree().Root.AddChild(tempEffect);
        tempEffect.GlobalPosition = source.GetGlobalPosition();
        var lightningLengthScale = (source.GlobalPosition.DistanceTo(target.GlobalPosition) *.8f) / SpriteSize.Y;
        Vector2 direction = target.GlobalPosition - source.GlobalPosition;
        tempEffect.Rotation = direction.Angle() + Mathf.DegToRad(defaultRotationOffset);
        tempEffect.Scale = new Vector2(.5f, lightningLengthScale);
        tempEffect.Visible = true;
        tempEffect.Frame = 0; 
        tempEffect.Play("lightning");
    }

    private bool ChainToNextTarget(out EnemyBase target)
    {
        var area2Ds = chainRange.GetOverlappingAreas();
        target = null;
        if (area2Ds.Count <= 0) return false;
        var enemiesInRange = new Array<EnemyBase>();
        GD.Print(area2Ds);
        foreach (var area2D in area2Ds)
        {
            if (area2D is not EnemyHitbox enemyHitbox) continue;
            if(pastTarget.Contains(enemyHitbox.Enemy)) continue; // ignore already attacked targets
            enemiesInRange.Add(enemyHitbox.Enemy);
        }
        if (enemiesInRange.Count == 0) return false;
        var randomEnemy = enemiesInRange[0]; // pick random target in range (method above does not arrange result by distance)
        target = randomEnemy;
        return true;
    }

    private void ChainAttackProcess(Node2D source, EnemyBase target)
    {
        pastTarget.Clear();
        AttackTarget(target);
        ShowLighteningEffect(source, target);
        var currentChainAmount = (ChainAmount-1);
        GD.Print(currentChainAmount);
        if (currentChainAmount <= 0) return;// attack finished
        var newAnchor = target;
        for (var i = currentChainAmount; i > 0; i--)
        {
            if (!ChainToNextTarget(out var targetEnemy)) return;
            if(targetEnemy is null) return;
            AttackTarget(targetEnemy);
            ShowLighteningEffect(newAnchor, targetEnemy);
            newAnchor = targetEnemy;
        }
        
        
    }

    public void MakeChainAttack(Node2D source, EnemyBase target)
    {
        ChainAttackProcess(source, target);
    }
    
}
