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
    [Export] private float holdBoundary = 50;
    private PokerGameManager pokerGameManagerRef;

    private bool isHoldOnCoolDown = false;
    public Action HoldActionCompleted;
    public bool IsMouseOverArea { get; private set; }
    public bool IsPressingTouchArea { get; private set; }
    private int holdTouchIndex;
    private Vector2 holdStartPosition;
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
        // interactionArea.MouseEntered += OnMouseEntered;
        // interactionArea.MouseExited += OnMouseExited;
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
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch)
        {
            if( touch.Pressed && IsPosInsideArea(touch.Position) )
            {
                IsPressingTouchArea = true;
                interactionProgress = 0;
                holdTouchIndex = touch.Index;
                holdStartPosition = touch.Position;
                GD.Print("Started holding");
            }
            if (touch.IsReleased())
            {
                IsPressingTouchArea = false;
                interactionProgress = 0;
                holdProgressBar.Visible = false;
            }
        }
        if(!IsPressingTouchArea) return; // only tracks dragging when area is pressed
        if (@event is InputEventScreenDrag screenDrag && screenDrag.Index == holdTouchIndex) // tracks the indexed touch input
        {
            if (!IsInBoundary(screenDrag.Position))
            {
                IsPressingTouchArea = false;
                interactionProgress = 0;
                holdProgressBar.Visible = false;
                GD.Print("drag out of bound");
            }
        }
    }


    private bool IsInBoundary(Vector2 currentPosition, float range = -1) // check if the touch is within the boundary (bubble)
    {
        if (range < 0)
        {
            range = holdBoundary;
        }
        // if(Mathf.Abs(currentPosition.X - holdStartPosition.X) > range) return false; (Mathf version)
        // if(Mathf.Abs(currentPosition.Y - holdStartPosition.Y) > range) return false;
        return currentPosition.DistanceTo(holdStartPosition) <= range;
    }
    
    private bool IsPosInsideArea(Vector2 pos) // raycast to detect if touch hits the interaction area
    {
        var spaceState = interactionArea.GetWorld2D().DirectSpaceState; // get reference to the physical state
        var query = new PhysicsPointQueryParameters2D // set raycast to detect Area2D ONLY
        {
            Position = pos,
            CollideWithAreas = true,
            CollideWithBodies = false
        };
        var results = spaceState.IntersectPoint(query); // the actual raycast query

        foreach (var result in results)
        {
            if (result["collider"].As<Node>() == interactionArea) // find node from collider
            {
                return true;
            }
        }
        return false;
    }

    private void OnHoldCoolDownTimer_Timeout()
    {
        isHoldOnCoolDown = false;
        GD.Print("Cooldown finished");
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if ( (IsJustStartedHolding() || IsPressingTouchArea) && !isHoldOnCoolDown)
        {
            holdProgressBar.Visible = true;
            // GD.Print(holdProgressBar.Value);
        }
        HoldInteractionProcess(delta);
    }

    private bool IsJustStartedHolding()
    {
        if (!IsMouseOverArea)
        {
            // GD.Print("mouse is Hovering");
            return false;
        }
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
        if(isHoldOnCoolDown || !holdProgressBar.Visible) return;
        if(!IsMouseOverArea && !IsPressingTouchArea)
        { 
            // GD.Print("Resetting");
            holdProgressBar.Visible = false;
            holdProgressBar.Value = 0;
            return;
        }
        if (Input.IsActionPressed("card_poker_hold") || IsPressingTouchArea)
        {
            interactionProgress += delta;
            holdProgressBar.Value = interactionProgress/HoldTimeRequired * 100;
            progressTextureAnchor.Position = holdStartPosition;
            // GD.Print($"{holdProgressBar.GlobalPosition}");
            // GD.Print($"{holdProgressBar.Value}");
            if (!(interactionProgress >= HoldTimeRequired)) return;
            HoldActionCompleted?.Invoke();
            GD.Print($"{GetParent().Name}: Hold action completed");
            holdProgressBar.Visible = false;
            isHoldOnCoolDown = true;
            HoldActionCoolDownTimer.Start();
        }
        else if (Input.IsActionJustReleased("card_poker_hold") || !IsPressingTouchArea)
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
        // GD.Print($"Remaining cards: {StoredPokers.Count}");
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
    
    /// <param name="index">
    /// if not given an index, will insert card at random
    /// </param>
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
