using Godot;
using System;
using System.Collections;
using Godot.Collections;
using HCoroutines;

public partial class ThreeOfAKindHand : Node2D
{
    [Export] private Area2D shockWaveArea;
    [Export] private AnimatedSprite2D shockWaveAnimation;
    [Export] public float ShockWaveStrength;
    [Export] private Timer durationTimer;
    [Export] public float DefaultDuration;
    [Export] private float shockWaveInterval;
    
    private ActionRpcHandler actionRpcHandlerRef;
    private bool isBonusActive;

    public override void _Ready()
    {
        actionRpcHandlerRef = ActionRpcHandler.Instance;
        shockWaveAnimation.Visible = false;
        durationTimer.WaitTime = DefaultDuration;
        durationTimer.Timeout += OnDurationTimerTimeout;
        actionRpcHandlerRef.ReleasedPokerHandAction += OnReleasedPokerHandAction;
        isBonusActive = false;
    }
    
    private void OnDurationTimerTimeout()
    {
        isBonusActive = false;
        shockWaveAnimation.Visible = false;
    }
    
    private void OnReleasedPokerHandAction(PokerHandBase pokerHandBase, ReleaseMode releaseMode)
    {
        if (pokerHandBase is PokerHandBase.ThreeOfAKind)
        {
            isBonusActive = true;
            durationTimer.Start();
            ShockWaveAttackProcess();
        }
    }

    private void MakeShockWave()
    {
        GD.Print("MakeShockWave");
        var area2Ds = shockWaveArea.GetOverlappingAreas();
        if (area2Ds.Count <= 0) return;
        Array<EnemyHitbox> enemyHitboxes = new Array<EnemyHitbox>();
        foreach (var area2D in area2Ds)
        {
            if (area2D is EnemyHitbox enemyHitbox)
            {
                enemyHitboxes.Add(enemyHitbox);
            }
        }
        if(enemyHitboxes.Count <= 0) return;
        GD.Print(enemyHitboxes);
        foreach (var enemyHitbox in enemyHitboxes)
        {
            enemyHitbox.KnockBack(ShockWaveStrength);
        }
    }
    private async void ShockWaveAttackProcess()
    {
        while (isBonusActive)
        {
            shockWaveAnimation.Visible = true;
            shockWaveAnimation.Play();
            MakeShockWave();
            await ToSignal(GetTree().CreateTimer(shockWaveInterval), SceneTreeTimer.SignalName.Timeout);
            shockWaveAnimation.Visible = false;
        }
    }
    
    /*public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("test_make_handAttack"))
        {
            GD.Print("ThreeOfAKindHand: Debug trigger");
            OnReleasedPokerHandAction(PokerHandBase.ThreeOfAKind, ReleaseMode.Charged);
        };
    }*/
}
