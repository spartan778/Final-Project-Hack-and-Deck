using Godot;
using System;
using Godot.Collections;

public partial class CardGameUIManager : Control
{
    
    private CardGameBase cardGameBaseRef;
    [Export] private CardSlots cardSlotsRef;
    [Export] private Label mainLabel;
    [Export] private Label subLabel;
    
    public override void _Ready()
    {
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        cardGameBaseRef = CardGameBase.Instance;
        ConnectSignals();
    }

    private void ConnectSignals()
    { 
        cardSlotsRef.ScanningPokerHand += OnScanningPokerHand;
        cardGameBaseRef.PokerGameManager.HoldingPoker +=  OnHoldingPoker;
    }

    private async void OnScanningPokerHand(PokerHandBase pokerHandBase, ReleaseMode releaseMode)
    {
        const string startingMainString = "Current Hand: ";
        const string startingSubString = "Effect: ";
        switch (pokerHandBase)
        {
            case PokerHandBase.BlackFlush:
            {
                mainLabel.Text = startingMainString + "Black Flush";
                subLabel.Text = startingSubString + "Powerful AOE attack + constant Kunai attack";
                break;
            }
            case PokerHandBase.RedFlush:
            {
                mainLabel.Text = startingMainString + "Red Flush";
                subLabel.Text = startingSubString + "Short Invincibility + Bonus Qi Regen + Healing";
                break;
            }
            case PokerHandBase.Straight:
            {
                mainLabel.Text = startingMainString + "Straight";
                subLabel.Text = startingSubString + "Chain Lightning Attack";
                break;
            }
            case PokerHandBase.ThreeOfAKind:
            {
                mainLabel.Text = startingMainString + "Three Of A Kind";
                subLabel.Text = startingSubString + "Water waves pushes enemies away";
                break;
            }
            case PokerHandBase.FourOfAKind:
            {
                mainLabel.Text = startingMainString + "Four of A Kind";
                subLabel.Text = "";
                break;
            }
            case PokerHandBase.OnePair:
            {
                mainLabel.Text = startingMainString + "One Pair";
                subLabel.Text = "";
                break;
            }
        }
        await ToSignal(GetTree().CreateTimer(10f), SceneTreeTimer.SignalName.Timeout);
        {
            mainLabel.Text = "";
            subLabel.Text = "";
        }
    }

    private async void OnHoldingPoker(PokerBase pokerBase)
    {
        
        const string startingMainString ="Holding Poker: ";
        const string subStringA = "On Slotted: ";
        const string subStringB = "\nRepeating: ";

        switch (pokerBase.PokerContent.PokerInfo.Suit)
        {
            case (CardSuit.Clubs or CardSuit.Spades):
            {
                mainLabel.Text = startingMainString + "Black";
                subLabel.Text = subStringA + "Fanning Kunai Attack"
                                           + subStringB + "Bonus Shuriken";
                break;
            }
            case (CardSuit.Diamonds or CardSuit.Hearts):
            {
                mainLabel.Text = startingMainString + "Red";
                subLabel.Text = subStringA + "Restore HP"
                                           + subStringB + "Bonus Qi Regen";
                break;
            } 
        }
        await ToSignal(GetTree().CreateTimer(10f), SceneTreeTimer.SignalName.Timeout);
        {
            mainLabel.Text = "";
            subLabel.Text = "";
        }
    }

    private void OnPeerDisconnected(long id)
    {
        subLabel.Text = "Player Disconnected";
    }
}
