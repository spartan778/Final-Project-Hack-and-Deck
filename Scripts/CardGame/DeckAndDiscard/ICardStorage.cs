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
    public int CardCount => StoredPokers.Count;
    private PokerGameManager pokerGameManagerRef;

    public override void _Ready()
    {
        if (PokerPreset != null)
        {
            StoredPokers = PokerPreset.SavedPokers;
            UpdateCardCountDisplay();
        }
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
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

    public void InsertAtBack(PokerInfo poker)
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
    
    public void UpdateCardCountDisplay()
    {
        cardCountLabel.Text = $"{StoredPokers.Count}";
    }
}
