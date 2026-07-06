using Godot;
using System;
using Godot.Collections;

public partial class TriggeredAttack : Node
{
    [Export] protected PlayerManager PlayerManagerRef;
    [Export] private BasicAttack basicAttackRef;
    [Export] private PackedScene triggeredBulletPrefab;
    
    private BulletManager bulletManagerRef;
    private ActionRpcHandler actionRpcHandler;
    
    private PlayerForm playerForm;
    public override void _Ready()
    {
        playerForm = PlayerManagerRef.PlayerForm;
        bulletManagerRef = ActionGameBase.Instance.BulletManagerRef;
        actionRpcHandler = ActionRpcHandler.Instance;
        
    }

    private void ConnectSignals()
    {
        PlayerManagerRef.PlayerFormChanged += OnPlayerFormChange;
    }

    private void OnPokerSlotTriggered(PokerInfo pokerInfo, Dictionary modifiers)
    {
        switch (pokerInfo.Suit)
        {
            case CardSuit.Hearts or CardSuit.Diamonds:
            {
                GD.Print($"Trigger support Poker, Strength {pokerInfo.Rank + 1}");
                break;
            }
            case CardSuit.Spades or CardSuit.Clubs:
            {
                var bulletCount = pokerInfo.Rank + 1;
                basicAttackRef.MakeBasicAttack(bulletCount);
                break;
            }
        }
    }
    private void OnPlayerFormChange(PlayerForm form)
    {
        playerForm = form;
    }
}
