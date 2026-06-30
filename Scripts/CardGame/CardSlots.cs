using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Godot.Collections;
using HCoroutines;
using Godot.NativeInterop;

public partial class CardSlots : Node2D
{
    [Export] private HBoxContainer cardSlotContainer;
    public Array<CardSlotControl> CardSlotControls{get; private set;}
    [Export] public Timer SlotCheckTimer { get; private set;}
    
    [Export] public float SlotCheckInterval { get; private set;}
    [Export] public float PokerTriggerInterval { get; private set;}
    
    


    public override void _Ready()
    {
        CardSlotControls = new Array<CardSlotControl>();
        var children = cardSlotContainer.GetChildren();
        foreach (var child in children) 
        {
            if (child is CardSlotControl cardSlotControl) // add all CardSlotControl node to array at runtime
            {
                CardSlotControls.Add(cardSlotControl);
            }
        }
        GD.Print("Slot Count: "+CardSlotControls.Count);
        SlotCheckTimer.WaitTime = SlotCheckInterval;
        SlotCheckTimer.Start();
        SlotCheckTimer.Timeout += OnSlotCheckTimer_Timeout;
        
    }
    public void SetSlotCheckInterval(float interval)
    {
        SlotCheckInterval = interval;
    }

    private void OnSlotCheckTimer_Timeout()
    {
        TriggerAllSlots();
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

public enum PokerHandType
{
    HighCard,
    OnePair,
    TwoPairs,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush,
}

public enum ReleaseMode
{
    Charged,
    Burst
}
