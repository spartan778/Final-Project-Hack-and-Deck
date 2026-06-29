using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using HCoroutines;

public partial class PokerGameManager : Node2D
{
    [Export] private InputManager inputManager;
    [Export] public CardGameBase CardGameBase { get; private set; }
    [Export] public Node2D GameBase2D { get; private set; }
    
    public RpcManager RpcManager { get; private set; }
    
    
    public PokerBase HeldPoker {get; private set;}
    public PokerBase HoveredPoker {get; private set;}
    public bool IsDragging {get; private set;}
    private Vector2 originalScale;

    private Vector2 phyMousePos, visualMousePos;
    
    public Action<PokerBase> HoldingPoker, ReleasingPoker, DrawingPoker, HoveringPoker, UnHoveringPoker;


    public override void _EnterTree()
    {
        CardGameHelperSingleton.Instance.SetPokerGameManager(this);
    }

    public override void _Ready()
    {
        ConnectSignals();
        GameStartSetup();
    }

    private void ConnectSignals()
    {
        HoldingPoker += OnHoldingPoker;
        ReleasingPoker += OnReleasingPoker;
        HoveringPoker += OnHoveringPoker;
        UnHoveringPoker += OnUnHoveringPoker;
        // CardGameBase.NetworkTickTimer.Timeout += MouseTrackingProcess;
    }
    
    private void GameStartSetup()
    {
        
    }
    private void OnHoldingPoker(PokerBase poker)
    {
        HeldPoker = poker;
        HeldPoker.GetParent()?.MoveChild(HeldPoker, -1); // Moving the picked card to top (in hierarchy)
        IsDragging = true;
        originalScale = poker.Scale;
        poker.Scale *= 1.1f;
        CardGameBase.NetworkTickTimer.Start();
    }

    private void OnReleasingPoker(PokerBase poker)
    {
        IsDragging = false;
        poker.Scale = originalScale;
        HeldPoker = null;
        CardGameBase.NetworkTickTimer.Stop();
    }

    private void OnHoveringPoker(PokerBase poker)
    {
        HoveredPoker = poker;
    }

    private void OnUnHoveringPoker(PokerBase poker)
    {
        HoveredPoker = null;
    }

    private void MouseTrackingProcess() // Testing RPC network strength with continuous calls
    {
        phyMousePos = GetGlobalMousePosition();
        RpcManager.Instance.MouseSyncTestRpc(phyMousePos);
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton {ButtonIndex: MouseButton.Left}:
            {
                if(@event.IsPressed())
                {
                    var draggedPoker = DetectPokerRayCast();
                    if(draggedPoker == null) return;
                    if(draggedPoker.IsLocked)
                    {
                        GD.Print($"Poker:{draggedPoker.PokerContent.PokerInfo} is locked");
                        return;
                    }
                    draggedPoker.PokerDraggingRef.IsDragging = true;
                    draggedPoker.PokerDraggingRef.PickUpOffset = draggedPoker.GlobalPosition - GetGlobalMousePosition();
                    HoldingPoker?.Invoke(draggedPoker);
                }
                break;
            }
        }
    }

    public PokerBase DetectPokerRayCast() // referenced from Godot official document 4.4 - Ray casting
    {
        GD.Print("Detecting poker ray cast");
        var spaceState = GetWorld2D().DirectSpaceState;
        var parameters = new PhysicsPointQueryParameters2D(); // setup detection parameters
        parameters.Position = GetGlobalMousePosition(); // using mouse position as raycast source
        parameters.CollideWithAreas = true;
        parameters.CollisionMask = 1;
        var results = spaceState.IntersectPoint(parameters);
        /*GD.Print(results);
        if (results != null)
        {
            foreach (var result in results)
            {
                var temp = result["collider"];
                var tempNode = temp.As<Node2D>().GetParent();
                GD.Print($"Debugging: {tempNode.Name}");
            }
        }*/
        // var firstResult = results.FirstOrDefault();
        if (results == null)
        {
            GD.Print("No card found");
            return null;
        };
        // GD.Print(results.ToString());
        List<PokerBase> detectedPokers = new List<PokerBase>(); //temp list to filter out all pokers
        foreach (var result in results)
        {
            var collider = (Node)result["collider"];
            var tempNode = collider.GetParent();
            if (tempNode is PokerBase)
            {
                detectedPokers.Add(tempNode as PokerBase);
            }
        }
        
        if (detectedPokers.Count > 0)
        {
            var topCard = detectedPokers.OrderByDescending(card => card.GetIndex()).First(); // get the card with the lowest index
            // GD.Print($"{detectedPokers.Count} cards detected");
            // GD.Print($"picking with index: {topCard.GetIndex()}");
            return topCard;
        }
        return null;
    }
    
}
