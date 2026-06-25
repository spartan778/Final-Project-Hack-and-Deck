using Godot;
using System;

public partial class DrawPile : Node2D
{
    [Export] public ICardStorage CardStorage;
    [Export] public Node CardSystemRef { get;private set; }
    [Export] private DeckArea deckArea;
    private PackedScene pokerPrefab;
    

    public override void _Ready()
    {
        CardStorage.StoredPokers = CardGameHelperSingleton.Instance.StartingDeck.SavedPokers;
        pokerPrefab = CardGameHelperSingleton.Instance.PokerPrefab;
        deckArea.IsClicked += DrawPoker;
        CardStorage.ReshufflePokers();
    }

    public void DrawPoker()
    {
        var drawnPokerInfo = CardStorage.DrawPoker();
        var newPoker = pokerPrefab.Instantiate<PokerBase>();
        newPoker.InitPoker(drawnPokerInfo);
        CardSystemRef.AddChild(newPoker);
        newPoker.GlobalPosition = new Vector2(500,500);
        // GD.Print($"DrawPoker: {drawnPokerInfo}");
    }
}
