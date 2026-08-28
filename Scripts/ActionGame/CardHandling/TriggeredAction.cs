using Godot;
using System;
using Godot.Collections;

public partial class TriggeredAction : Node
{
    [Export] protected PlayerManager PlayerManagerRef;
    [Export] private BasicAttack basicAttackRef;
    [Export] private PackedScene triggeredBulletPrefab;
    [Export] private HealthSystem healthSystemRef;
    [Export] private QiSystem qiSystemRef;
    
    private BulletManager bulletManagerRef;
    private ActionRpcHandler actionRpcHandler;
    
    private PlayerForm playerForm;
    public override void _Ready()
    {
        playerForm = PlayerManagerRef.PlayerForm;
        bulletManagerRef = ActionGameBase.Instance.BulletManagerRef;
        actionRpcHandler = ActionRpcHandler.Instance;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        PlayerManagerRef.PlayerFormChanged += OnPlayerFormChange;
        actionRpcHandler.TriggerPokerAction += OnPokerSlotTriggered;
    }

    private void OnPokerSlotTriggered(PokerInfo pokerInfo, Dictionary modifiers)
    {
        switch (pokerInfo.Suit)
        {
            case CardSuit.Hearts or CardSuit.Diamonds:
            {
                var regenValue = pokerInfo.Rank + 1;
                if (playerForm is PlayerForm.Defensive)
                {
                    regenValue *= 2;
                }
                qiSystemRef.RegenQi(regenValue);
                break;
            }
            case CardSuit.Spades or CardSuit.Clubs:
            {
                var bulletCount = pokerInfo.Rank + 1;
                if(playerForm is PlayerForm.Aggressive)
                {
                    basicAttackRef.MakeBasicAttack(bulletCount);
                }
                break;
            }
        }
    }
    private void OnPlayerFormChange(PlayerForm form)
    {
        playerForm = form;
    }
}
