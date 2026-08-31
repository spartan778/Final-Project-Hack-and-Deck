using Godot;
using System;
using System.Collections;
using HCoroutines;

public partial class BlackFlushHand : Node2D
{
    [Export] private PlayerManager playerManagerRef;
    [Export] private QiBlastAttack qiBlastAttackRef;
    [Export] private MagicBulletAttack magicBulletAttackRef;
    [Export] public float DefaultBuffDuration { get; private set; }
    [Export] public float DefaultMagicAttackInterval { get; private set; }
    
    [Export] public int TotalBlastAmount, MagicBulletAmount;
    private ActionRpcHandler actionRpcHandlerRef;
    private float qiBlastVortexCompleteTIme = 2f;
    private bool isBonusActive;

    public override void _Ready()
    {
        actionRpcHandlerRef = ActionRpcHandler.Instance;
        actionRpcHandlerRef.ReleasedPokerHandAction += OnReleasedPokerHandAction;
    }

    private void OnReleasedPokerHandAction(PokerHandBase pokerHandBase, ReleaseMode releaseMode)
    {
        if (pokerHandBase is PokerHandBase.BlackFlush)
        {
            StartBuffStatus();
            MagicAttackProcess();
            MakeQiVortex();
        }
    }

    private async void StartBuffStatus()
    {
        isBonusActive = true;
        await ToSignal(GetTree().CreateTimer(DefaultBuffDuration), SceneTreeTimer.SignalName.Timeout);
        isBonusActive = false;
    }

    private async void MagicAttackProcess()
    {
        while (isBonusActive)
        {
            MakeMagicAttack();
            await ToSignal(GetTree().CreateTimer(DefaultMagicAttackInterval), SceneTreeTimer.SignalName.Timeout);
        }
    }
    private void MakeMagicAttack()
    {
        magicBulletAttackRef.MakeMagicAttack(MagicBulletAmount);
    }

    private void MakeQiVortex()
    {
        Co.Run(QiVortexCoroutine);
    }

    private IEnumerator QiVortexCoroutine()
    {
        var startingVector = playerManagerRef.GetMouseToPlayerVector();
        var interval = qiBlastVortexCompleteTIme / TotalBlastAmount;
        for (var i = 0; i < TotalBlastAmount; i++)
        {
            float timer = 0;
            while (timer < interval)
            {
                timer += (float)GetProcessDeltaTime();
                yield return null;
            }
            var targetVector = startingVector.Rotated(Mathf.Tau/TotalBlastAmount * i);
            qiBlastAttackRef.MakeQiBlast(targetVector);
            
        }
    }
    /*public override void _Input(InputEvent @event) 
    {
        if (@event.IsActionPressed("test_make_handAttack"))
        {
            GD.Print("BlackFlushHand: Debug trigger");
            OnReleasedPokerHandAction(PokerHandBase.BlackFlush, ReleaseMode.Charged);
        };
    }*/
}
