using Godot;
using System;

public partial class DirectionalAnchor : Node2D
{
    [Export] private CardSummonSystem cardSummonSystem;
    [Export] private PlayerManager playerManagerRef;
    private ActionGamePoker actionGamePokerRef;
    private ActionRpcHandler actionRpcHandlerRef;
    
    private PokerInfo summonedPokerInfo;
    private PokerState summonedPokerState;
    
    private bool isPokerSummoned = false;
    

    public override void _Ready()
    {
        actionRpcHandlerRef = ActionRpcHandler.Instance;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        actionRpcHandlerRef.SummonPokerAction += OnSummonPoker;
    }

    private void OnSummonPoker(PokerInfo pokerInfo, PokerState pokerState)
    {
        summonedPokerInfo = pokerInfo;
        summonedPokerState = pokerState;
        isPokerSummoned = true;
    }

    public override void _Process(double delta)
    {
        AlignArrowProcess();
    }

    private void AlignArrowProcess()
    {
        if(!isPokerSummoned) return;
        // GD.Print("Rotating");
        var target = cardSummonSystem.SummonedActionPoker;
        var targetVector = playerManagerRef.GetTargetToPlayerVector(target);
        var radian = targetVector.Angle();
        Rotation = radian;
        // GD.Print($"{targetVector}");
        // var degrees = Mathf.RadToDeg(radian);
        // GD.Print($"{degrees}");
        // Rotation = degrees;
    }
}
