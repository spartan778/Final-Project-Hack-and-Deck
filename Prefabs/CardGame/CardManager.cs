using Godot;
using System;
using System.Linq;

public partial class CardManager : Node2D
{
    [Export] private InputManager inputManager;
    private PokerBase holdingPoker;
    
    public Action<PokerBase> HoldingPoker, ReleasingPoker;

    public override void _Ready()
    {
        
    }

    private void OnHoldingPoker(PokerBase poker)
    {
        holdingPoker = poker;
    }

    private void OnReleasingPoker(PokerBase poker)
    {
        holdingPoker = null;
    }
    

    public PokerBase DetectPokerRayCast() // referenced from Godot official document 4.4 - Ray casting
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
