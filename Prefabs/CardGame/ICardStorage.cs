using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class ICardStorage : Node
{
    [Export] public Array<PokerInfo> StoredPokers;
    [Export] public PokerArray PokerPreset;

    public override void _Ready()
    {
        if (PokerPreset != null)
        {
            StoredPokers = PokerPreset.SavedPokers;
        }
    }
    public void ReshufflePokers()
    {
        for (var i = 0; i < StoredPokers.Count; i++)
        {
            var j = GD.RandRange(0, StoredPokers.Count);
            (StoredPokers[i], StoredPokers[j]) = (StoredPokers[j], StoredPokers[i]); //swap the two pokers
        }
    }
    public PokerInfo DrawPoker()
    {
        var poker = StoredPokers[0];
        StoredPokers.RemoveAt(0);
        GD.Print($"Remaining cards: {StoredPokers.Count}");
        return poker;
    }

    public void InsertPoker(PokerInfo poker, int index = -1)
    {
        var count = StoredPokers.Count;
        var i = (index < 0 || index > count) ? GD.RandRange(0, count) : index;
        StoredPokers.Insert(i, poker);
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
}
