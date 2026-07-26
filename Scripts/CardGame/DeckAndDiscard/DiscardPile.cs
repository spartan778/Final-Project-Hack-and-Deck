using Godot;
using System;
using System.Linq;

public partial class DiscardPile : Node2D
{
    [Export] public ICardStorage DiscardStorage;
    [Export] private DrawPile drawPileRef;
    [Export] private AnimatedSprite2D discardPileSprite;
    [Export] private DeckArea inputArea;
    private PokerGameManager pokerGameManagerRef;

    public override void _Ready()
    {
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
        inputArea.IsClicked += RefillRandomToDrawPile;
        DiscardStorage.HoldActionCompleted += RefillAllToDrawPile;
        UpdateDiscardDisplay();
    }

    public void AddToDiscardPile(PokerInfo poker, int index = -1)
    {
        DiscardStorage.InsertPoker(poker, index);
        UpdateDiscardDisplay();
    }

    public void AddToDiscardPile(PokerBase poker, int index = -1)
    {
        var pokerInfo = poker.PokerContent.PokerInfo;
        DiscardStorage.InsertPoker(pokerInfo, index);
        UpdateDiscardDisplay();
    }
    
    public void RefillToDrawPile(bool isRandom = false)
    {
        if (isRandom)
        {
            RefillRandomToDrawPile();
            return;
        }
        var drawnPoker = DiscardStorage.DrawPoker();
        if (drawnPoker != null)
        {
            drawPileRef.CardStorage.InsertAtBack(drawnPoker);
            UpdateDiscardDisplay();
        }
    }

    public void RefillAllToDrawPile()
    {
        var drawnPokers= DiscardStorage.DrawAllPokers();
        if (drawnPokers == null || drawnPokers.Count == 0) return;
        foreach (var drawnPoker in drawnPokers)
        {
            drawPileRef.CardStorage.InsertAtBack(drawnPoker);
        }
        UpdateDiscardDisplay();
    }
    
    public void UpdateDiscardDisplay()
    {
        discardPileSprite.Visible = DiscardStorage.CardCount > 0;
    }

    private void RefillRandomToDrawPile()
    {
        if (!DiscardStorage.TryDrawRandomPoker(out var poker)) return;
        UpdateDiscardDisplay();
        drawPileRef.CardStorage.InsertAtBack(poker);
    }
}
