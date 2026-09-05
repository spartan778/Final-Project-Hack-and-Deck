using Godot;
using System;
using Godot.Collections;

public partial class SlottedAction : Node
{
    [Export] protected PlayerManager PlayerManagerRef;
    [Export] private HealthSystem healthSystemRef;
    [Export] private MagicBulletAttack magicBulletAttackRef;
    
    private BulletManager bulletManagerRef;
    private ActionRpcHandler actionRpcHandler;
    
    private PlayerForm playerForm;

    public override void _Ready()
    {
        playerForm = PlayerManagerRef.PlayerForm;
        actionRpcHandler = ActionRpcHandler.Instance;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        PlayerManagerRef.PlayerFormChanged += OnPlayerFormChange;
        actionRpcHandler.SlotPokerAction += OnPokerSlotted;
    }
    private async void OnPokerSlotted(PokerInfo pokerInfo, Dictionary modifiers)
    {
        switch (pokerInfo.Suit)
        {
            case CardSuit.Hearts or CardSuit.Diamonds:
            {
                // GD.Print($"Slotted support Poker, Strength {pokerInfo.Rank + 1}");
                healthSystemRef.Heal(pokerInfo.Rank + 1);
                if (PlayerManagerRef.PlayerForm is PlayerForm.Defensive) // heals again after short delay if in Defensive mode
                {
                    await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
                    healthSystemRef.Heal(pokerInfo.Rank + 1);
                }
                break;
            }
            case CardSuit.Spades or CardSuit.Clubs:
            {
                var bulletCount = pokerInfo.Rank + 1;
                magicBulletAttackRef.MakeMagicAttack(bulletCount);
                if (PlayerManagerRef.PlayerForm is PlayerForm.Aggressive) // attack again if in Aggressive mode
                {
                    await ToSignal(GetTree().CreateTimer(.5f), SceneTreeTimer.SignalName.Timeout);
                    magicBulletAttackRef.MakeMagicAttack(bulletCount);
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
