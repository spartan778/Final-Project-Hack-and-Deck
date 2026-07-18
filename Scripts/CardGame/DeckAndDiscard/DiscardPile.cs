using Godot;
using System;

public partial class DiscardPile : Node2D
{
    [Export] public ICardStorage DiscardStorage;
    [Export] private DrawPile drawPileRef;
    [Export] private AnimatedSprite2D discardPileSprite;
    
    private PokerGameManager pokerGameManagerRef;

    public override void _Ready()
    {
        pokerGameManagerRef = CardGameHelperSingleton.Instance.PokerGameManager;
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

    public void UpdateDiscardDisplay()
    {
        discardPileSprite.Visible = DiscardStorage.CardCount > 0;
    }

    public void RefillRandomToDrawPile()
    {
        if (DiscardStorage.TryDrawRandomPoker(out var poker))
        {
            GD.Print($"Discard Pile: {DiscardStorage}");
            drawPileRef.CardStorage.InsertPoker(poker,drawPileRef.CardStorage.CardCount-1);
        }
    }
}
