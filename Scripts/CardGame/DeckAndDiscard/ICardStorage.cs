using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class ICardStorage : Node
{
    [Export] public Array<PokerInfo> StoredPokers;
    [Export] public PokerArray PokerPreset;
    [Export] private Label cardCountLabel;
    [Export] private Area2D interactionArea;
    [Export] public Timer HoldActionCoolDownTimer { get; private set; }
    [Export] private TextureProgressBar holdProgressBar;
    [Export] private Node2D progressTextureAnchor;
    [Export] public float HoldTimeRequired {get; private set;}
    [Export] public float HoldCoolDownTime {get; private set;}
    private PokerGameManager pokerGameManagerRef;

    private bool isHoldOnCoolDown = false;
    public Action HoldActionCompleted;
    public bool IsMouseOverArea { get; private set; }
    public int CardCount
    {
        get
        {
            UpdateCardCountDisplay();
            return StoredPokers.Count;
        }
    }

    private double interactionProgress;
    

    public override void _Ready()
    {
        if (PokerPreset != null)
        {
            StoredPokers = PokerPreset.SavedPokers;
            UpdateCardCountDisplay();
        }
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
        HoldActionCoolDownTimer.WaitTime = HoldCoolDownTime;
        // holdProgressBar.Value = 100;
        holdProgressBar.Visible = false;
        ConnectSignals();
    }
    private void ConnectSignals()
    {
        interactionArea.MouseEntered += OnMouseEntered;
        interactionArea.MouseExited += OnMouseExited;
        HoldActionCoolDownTimer.Timeout += OnHoldCoolDownTimer_Timeout;
    }
    
    private void OnMouseEntered()
    {
        IsMouseOverArea = true;
        interactionProgress = 0; // making sure progress is reset
    }
    private void OnMouseExited()
    {
        IsMouseOverArea = false;
        interactionProgress = 0;
    }

    private void OnHoldCoolDownTimer_Timeout()
    {
        isHoldOnCoolDown = false;
        GD.Print("Cooldown finished");
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (IsJustStartedHolding() && !isHoldOnCoolDown)
        {
            holdProgressBar.Visible = true;
        }
        
        HoldInteractionProcess(delta);
    }

    private bool IsJustStartedHolding()
    {
        if (Input.IsActionJustPressed("card_poker_hold"))
        {
            GD.Print("just started holding");
            return true;
        }
        return false;
    }

    private bool IsJustStoppedHolding()
    {
        if (Input.IsActionJustReleased("card_poker_hold"))
        {
            GD.Print("just released");
            return true;
        }
        return false;
    }
    private void HoldInteractionProcess(double delta)
    {
        if(!IsMouseOverArea || isHoldOnCoolDown)
        { 
            holdProgressBar.Visible = false;
            holdProgressBar.Value = 0;
            return;
        }
        if (Input.IsActionPressed("card_poker_hold"))
        {
            interactionProgress += delta;
            holdProgressBar.Value = interactionProgress/HoldTimeRequired * 100;
            progressTextureAnchor.Position = progressTextureAnchor.GetGlobalMousePosition();
            GD.Print($"{holdProgressBar.GlobalPosition}");
            GD.Print($"{holdProgressBar.Value}");
            if (!(interactionProgress >= HoldTimeRequired)) return;
            HoldActionCompleted?.Invoke();
            GD.Print($"{GetParent().Name}: Hold action completed");
            holdProgressBar.Visible = false;
            isHoldOnCoolDown = true;
            HoldActionCoolDownTimer.Start();
        }
        if (Input.IsActionJustReleased("card_poker_hold"))
        {
            holdProgressBar.Visible = false;
            holdProgressBar.Value = 0;
            return;
        }
    }
    

    public void ReshufflePokers()
    {
        for (var i = 0; i < StoredPokers.Count; i++)
        {
            var j = GD.RandRange(0, StoredPokers.Count-1);
            (StoredPokers[i], StoredPokers[j]) = (StoredPokers[j], StoredPokers[i]); //swap the two pokers
        }
    }
    public PokerInfo DrawPoker()
    {
        if (StoredPokers.Count == 0)
        {
            GD.Print("DrawDeck is empty");
            return null;
        }
        var poker = StoredPokers[0];
        StoredPokers.RemoveAt(0);
        GD.Print($"Remaining cards: {StoredPokers.Count}");
        UpdateCardCountDisplay();
        return poker;
    }

    public bool TryDrawRandomPoker(out PokerInfo pokerInfo)
    {
        if (StoredPokers.Count == 0)
        {
            pokerInfo = null;
            return false;
        }
        var index = GD.RandRange(0, StoredPokers.Count-1);
        var poker = StoredPokers[index];
        StoredPokers.RemoveAt(index);
        pokerInfo = poker;
        UpdateCardCountDisplay();
        return true;
    }

    public void InsertPoker(PokerInfo poker, int index = -1)
    {
        var count = StoredPokers.Count;
        var i = (index < 0 || index >= count) ? GD.RandRange(0, count-1) : index; // if not given an index param, will insert card at random
        // GD.Print($"Inserting at: {i}");
        if (count == 0) i = 0; // avoid edge case of (count-1) = -1 when stored poker is 0
        StoredPokers.Insert(i, poker);
        UpdateCardCountDisplay();
        GD.Print($"{GetParent().Name}: Inserted, current pokers: {StoredPokers}");
    }

    public void InsertAtBack(PokerInfo poker) // insert the poker at the back of the list
    {
        var targetIndex = StoredPokers.Count - 1;
        StoredPokers.Insert(targetIndex, poker);
        UpdateCardCountDisplay();
    }

    public void RefreshPokers(Array<PokerInfo> pokers)
    {
        StoredPokers = pokers;
        ReshufflePokers();
    }

    public void RefreshPokers(PokerArray pokerArray)
    {
        StoredPokers = pokerArray.SavedPokers;
    }

    public Array<PokerInfo> DrawAllPokers(bool isRandom = true)
    {
        var tempArray = new Array<PokerInfo>();
        if (isRandom)
        {
            ReshufflePokers();
        }
        foreach (var poker in StoredPokers)
        {
            tempArray.Add(poker);
        }
        StoredPokers.Clear();
        return tempArray;
    }
    
    public void UpdateCardCountDisplay()
    {
        cardCountLabel.Text = $"{StoredPokers.Count}";
    }
}
