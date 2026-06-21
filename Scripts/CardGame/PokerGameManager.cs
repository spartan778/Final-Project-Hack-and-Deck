using Godot;
using System;
using System.Linq;

public partial class PokerGameManager : Node2D
{
    [Export] private InputManager inputManager;
    [Export] public CardGameBase CardGameBase { get; private set; }
    [Export] public Node2D GameBase2D { get; private set; }
    
    public PokerBase HeldPoker {get; private set;}
    public PokerBase HoveredPoker {get; private set;}
    public bool IsDragging {get; private set;}
    private Vector2 originalScale;
    
    public Action<PokerBase> HoldingPoker, ReleasingPoker, HoveringPoker, UnHoveringPoker;


    public override void _EnterTree()
    {
        CardGameHelperSingleton.Instance.SetPokerGameManager(this);
    }

    public override void _Ready()
    {
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        HoldingPoker += OnHoldingPoker;
        ReleasingPoker += OnReleasingPoker;
        HoveringPoker += OnHoveringPoker;
        UnHoveringPoker += OnUnHoveringPoker;
    }

    private void OnHoldingPoker(PokerBase poker)
    {
        HeldPoker = poker;
        HeldPoker.GetParent()?.MoveChild(HeldPoker, -1); // Moving the picked card to top
        IsDragging = true;
        originalScale = poker.Scale;
        poker.Scale *= 1.1f;
        GD.Print($"Holding: {HeldPoker.Name}");
    }

    private void OnReleasingPoker(PokerBase poker)
    {
        GD.Print($"Releasing: {poker.Name}");
        IsDragging = false;
        poker.Scale = originalScale;
        HeldPoker = null;
    }

    private void OnHoveringPoker(PokerBase poker)
    {
        HoveredPoker = poker;
    }

    private void OnUnHoveringPoker(PokerBase poker)
    {
        HoveredPoker = null;
    }

    public PokerBase DetectPokerRayCast() // referenced from Godot official document 4.4 - Ray casting (not in use)
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var parameters = new PhysicsPointQueryParameters2D(); // setup detection parameters
        parameters.Position = GetGlobalMousePosition(); // using mouse position as raycast source
        parameters.CollideWithAreas = true;
        parameters.CollisionMask = 1;
        var results = spaceState.IntersectPoint(parameters);
        var firstResult = results.FirstOrDefault();
        if (firstResult == null)
        {
            GD.Print("No card found");
            return null;
        };
        var collider = firstResult["collider"];
        var node = collider.As<Area2D>().GetParent();
        GD.Print(node.Name);
        return node as PokerBase; // will return null if not a poker
    }
    
}
