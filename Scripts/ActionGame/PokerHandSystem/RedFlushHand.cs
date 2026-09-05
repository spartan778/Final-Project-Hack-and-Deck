using Godot;
using System;

public partial class RedFlushHand : Node2D
{
    [Export] public Timer DurationTimer { get; private set; }
    [Export] public float DefaultDuration { get; private set; }
    [Export] public int InvincibleDuration { get; private set; }
    [Export] private QiSystem qiSystemRef;
    [Export] private PlayerHitbox playerHitboxRef;
    private ActionRpcHandler actionRpcHandlerRef;
    

    public override void _Ready()
    {
        actionRpcHandlerRef = ActionRpcHandler.Instance;
        actionRpcHandlerRef.ReleasedPokerHandAction += OnReleasedPokerHandAction;
        DurationTimer.Timeout += OnDurationTimerTimeout;
        DurationTimer.WaitTime = DefaultDuration;
    }

    private void OnDurationTimerTimeout()
    {
        qiSystemRef.QiRegenBonusModifier -= 2;
        playerHitboxRef.HealthSystem.Heal(playerHitboxRef.HealthSystem.MaxHealth/2); // heal half of full HP again 
    }

    private void OnReleasedPokerHandAction(PokerHandBase pokerHandBase, ReleaseMode releaseMode)
    {
        if (pokerHandBase is PokerHandBase.RedFlush)
        {
            playerHitboxRef.StartInvincibility(InvincibleDuration);
            playerHitboxRef.HealthSystem.Heal(playerHitboxRef.HealthSystem.MaxHealth/2); // heal half of full HP
            qiSystemRef.QiRegenBonusModifier += 2; // triple Qi Regen speed for the duration
            DurationTimer.Start();
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("test_make_handAttack2"))
        {
            GD.Print("RedFlushHand: Debug trigger");
            OnReleasedPokerHandAction(PokerHandBase.RedFlush, ReleaseMode.Charged);
        };
    }
}
