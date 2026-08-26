using Godot;
using System;

public partial class ActionGamePoker : Node2D
{
    [Export] public PokerContent PokerContent { get; private set; }
    [Export] public Area2D PokerArea{ get; private set; }
    [Export] public PokerModifiersManager PokerModifiersManager { get; private set; }
    [Export] public PokerSummonManager PokerSummonManager { get; private set; }
    
    private CardSummonSystem cardSummonSystemRef;
    
    public PokerState PokerState => PokerModifiersManager.PokerState;
    public PokerType PokerType => PokerModifiersManager.PokerType;
    public int PokerChargeCount {get; private set;}
    

    public override void _Ready()
    {
        PokerArea.AreaEntered += OnPokerAreaEntered;
    }
    
    public void InitActionPoker(PokerInfo pokerInfo, PokerState pokerState)
    {
        PokerContent.ChangePokerInfo(pokerInfo);
        PokerModifiersManager.SetPokerState(pokerState);
        PokerSummonManager.SetShowSummonEffect(true);
        PokerChargeCount = pokerInfo.Rank;
    }

    public void SetCardSummonSystem(CardSummonSystem cardSummonSystem)
    {
        cardSummonSystemRef = cardSummonSystem;
    }

    private void OnPokerAreaEntered(Area2D area)
    {
        HandleBullet(area);
    }

    private void HandleBullet(Area2D area) // handle when a bullet enters the area
    {
        if(area is not BulletBase bullet) return; // ignore non-bullets
        
        var cardSuit = PokerContent.PokerInfo.Suit;
        switch (cardSuit)
        {
            case CardSuit.Diamonds or CardSuit.Hearts:
            {
                if (bullet.IsFromEnemy && PokerChargeCount>=0)
                {
                    bullet.Blocked(); // a summoned red card will block enemy bullets
                    GD.Print("Bullet blocked");
                    PokerChargeCount -= 1;
                    if (PokerChargeCount <= 0)
                    {
                        cardSummonSystemRef.SummonPokerUsedUp?.Invoke();
                        QueueFree();
                    }
                }
                break;
            }
            case CardSuit.Clubs or CardSuit.Spades: // a summoned black card will boost player attack speed and damage
            {
                if (bullet.IsFromPlayer && PokerChargeCount>=0)
                {
                    bullet.SetBulletSpeed(bullet.BulletSpeed * 2f);
                    bullet.SetDamage(bullet.Damage * 2f);
                    PokerChargeCount -= 1;
                    GD.Print("Attack boosted");
                    if (PokerChargeCount <= 0)
                    {
                        cardSummonSystemRef.SummonPokerUsedUp?.Invoke();
                        QueueFree();
                    }
                }
                break;
            }
        }
    }
}
