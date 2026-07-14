using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using HCoroutines;
using Godot.NativeInterop;

public partial class CardSlots : Node2D
{
    [Export] private HBoxContainer cardSlotContainer;
    public Array<CardSlotControl> CardSlotControls { get; private set; }
    [Export] public Timer SlotCheckTimer { get; private set; }
    [Export] public Timer ReleaseSlotsCoolDownTimer { get; private set; }
    [Export] public float SlotCheckInterval { get; private set; }
    [Export] public float PokerTriggerInterval { get; private set; }
    [Export] public float SwipeCoolDownTime { get; private set; }
    [Export] private TextureProgressBar coolDownProgressBar, swipeProgressBar;
    public Action SwipeSuccess;
    
    public bool IsInCoolDown { get; private set; }
    private RpcManager rpcManagerRef;
    public Array<PokerBase> SlottedPokerArray {get; private set;}
    public Array<PokerHandBase> PokerHandArray {get; private set;}


    public override void _Ready()
    {
        CardSlotControls = new Array<CardSlotControl>();
        SlottedPokerArray = new Array<PokerBase>();
        PokerHandArray = new Array<PokerHandBase>();
        rpcManagerRef = RpcManager.Instance;
        var children = cardSlotContainer.GetChildren();
        foreach (var child in children) 
        {
            if (child is CardSlotControl cardSlotControl) // add all CardSlotControl node to array at runtime
            {
                CardSlotControls.Add(cardSlotControl);
            }
        }
        // GD.Print("Slot Count: "+CardSlotControls.Count);
        IsInCoolDown = false;
        SlotCheckTimer.WaitTime = SlotCheckInterval;
        SlotCheckTimer.Start();
        ReleaseSlotsCoolDownTimer.WaitTime = SwipeCoolDownTime;
        ConnectSignals();
        
    }

    private void ConnectSignals()
    {
        SlotCheckTimer.Timeout += OnSlotCheckTimer_Timeout;
        SwipeSuccess += OnSwipeSuccess;
        ReleaseSlotsCoolDownTimer.Timeout += OnCoolDownTimer_Timeout;
    }
    
    public void ScanPokerSlots()
    {
        SlottedPokerArray.Clear();
        foreach (var cardSlot in CardSlotControls)
        {
            if (cardSlot.CardSlotBase.SlottedPoker != null)
            {
                SlottedPokerArray.Add(cardSlot.CardSlotBase.SlottedPoker);
            }
        }
        if(SlottedPokerArray.Count == 0)
        {
            GD.Print("No Poker in slots");
            swipeProgressBar.Value = 0;
            return;
        }
        var playedPokerHands= ScanPlayedPokerHands();
        ReleaseSlotsCoolDownTimer.Start();
        GD.Print("Swipe cooldown started");
        swipeProgressBar.Value = 0;
        coolDownProgressBar.Visible = true;
        IsInCoolDown = true;
        GD.Print(playedPokerHands);
    }

    private Array<PokerHandBase> ScanPlayedPokerHands()
    {
        PokerHandArray.Clear();
        ScanSameRank();
        ScanSameSuit();
        ScanContinuousRank();
        return PokerHandArray;
    }

    private void ScanSameRank()
    {
        var pokerValues = new List<int>();
        foreach (var card in SlottedPokerArray) // turn all card's rank into a list (ignoring suits)
        {
            pokerValues.Add(card.PokerContent.PokerInfo.Rank);
        }
        var pokerHandGroups = pokerValues.GroupBy(ranks => ranks).ToList(); // group up all cards with the same Rank
        int maxSameValueCount = pokerHandGroups.Max(group => group.Count()); // find the largest number of cards with same Rank
        if (maxSameValueCount >= 4) // this is a Four/Five of a count hand
        { // separated in case slot size change
            if(maxSameValueCount == 5)
            {
                PokerHandArray.Add(PokerHandBase.FiveOfAKind);
                GD.Print("FourOfAKind: " + maxSameValueCount);
                return;
            }
            PokerHandArray.Add(PokerHandBase.FourOfAKind);
            return; // in a 5 card hand there should be no other possible hands
        }
        if (maxSameValueCount == 3)
        {
            PokerHandArray.Add(PokerHandBase.ThreeOfAKind);
            GD.Print("ThreeOfAKind: " + maxSameValueCount);
        }
        int pairCount = pokerHandGroups.Count(group => group.Count() == 2); // count the number of groups with exactly 2 cards (a pair)

        if (pairCount <= 0) return;
        for (var i = 0; i < pairCount; i++) // add a pair to list for each pair that exist
        {
            PokerHandArray.Add(PokerHandBase.OnePair);
            GD.Print("Pairs: " + pairCount);
        }
    }
    
    private void ScanSameSuit()
    {
        if(SlottedPokerArray.Count < 5) return; // currently only 5 cards can form a flush
        var pokerSuits = new List<CardSuit>();
        foreach (var card in SlottedPokerArray) // turn all card's suit into a list (ignoring rank)
        {
            pokerSuits.Add(card.PokerContent.PokerInfo.Suit);
        }
        var pokerHandGroups = pokerSuits.GroupBy(suit => suit).ToList(); // group up all cards with the same Suit
        int maxSameSuitCount = pokerHandGroups.Max(group => group.Count());
        GD.Print("Max Poker suit count: " + maxSameSuitCount);
        if (maxSameSuitCount >= 5)
        {
            var suitWithFiveCards = pokerHandGroups
                .OrderByDescending(group => group.Count()) // order the groups by amount of cards
                .First(); // get the group with biggest count
            if (suitWithFiveCards.Key is CardSuit.Clubs or CardSuit.Spades)
            {
                PokerHandArray.Add(PokerHandBase.BlackFlush);
                GD.Print("Black Flush: " + PokerHandArray.Count);
            }
            else
            {
                PokerHandArray.Add(PokerHandBase.RedFlush);
                GD.Print("Red Flush: " + PokerHandArray.Count);
            }
        }
    }

    private void ScanContinuousRank()
    {
        if(SlottedPokerArray.Count < 5) return; // currently only 5 cards can form a Straight
        var pokerRanks = new List<int>();
        foreach (var card in SlottedPokerArray)
        {
            pokerRanks.Add(card.PokerContent.PokerInfo.Rank);
        }
        var pokersInOrder = pokerRanks.OrderByDescending(rank => rank).ToList();
        bool isContinuous = true;
        for (var i = 0; i < pokersInOrder.Count-1; i++)
        {
            isContinuous = pokersInOrder[i] == pokersInOrder[i + 1] + 1; //check if the different between two card is 1
            if (!isContinuous) break;
        }
        if (isContinuous)
        {
            PokerHandArray.Add(PokerHandBase.Straight);
            GD.Print("Straight: " + PokerHandArray.Count);
        }
    }
    
    public void SetSlotCheckInterval(float interval)
    {
        SlotCheckInterval = interval;
    }

    private void OnSlotCheckTimer_Timeout()
    {
        TriggerAllSlots();
    }
    private void OnSwipeSuccess()
    {
        ScanPokerSlots();
    }

    private void OnCoolDownTimer_Timeout()
    {
        // coolDownProgressBar.Visible = false;
        // coolDownProgressBar.Value = 0;
        swipeProgressBar.Visible = true;
        IsInCoolDown = false;
    }
    
    private void TriggerAllSlots()
    {
        Co.Run(TriggerAllSlotsCoroutine);
    }
    private IEnumerator TriggerAllSlotsCoroutine() // same pattern as Unity Coroutine using HCoroutine plugin
    {
        foreach (var cardSlotControl in CardSlotControls)
        {
            cardSlotControl.CardSlotBase.TriggerPoker();
            yield return Co.Wait(PokerTriggerInterval);
        }
        yield return null;
    }
}

public enum PokerHandBase
{
    HighCard,
    OnePair,
    ThreeOfAKind,
    Straight,
    BlackFlush,
    RedFlush,
    FourOfAKind,
    FiveOfAKind,
}

public enum ReleaseMode
{
    Charged,
    Burst
}
