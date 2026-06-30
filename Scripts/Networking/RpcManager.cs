using Godot;
using System;
using Godot.Collections;
using static Godot.MultiplayerApi;
using Array = Godot.Collections.Array;

public partial class RpcManager : Node
{
    private NetworkManager_Singleton networkManagerSingleton;
    public static RpcManager Instance {get; private set;}
    private MultiplayerPeer multiplayerPeer;

    public int AddCount { get; private set; } = 0;
    
    public Action<int> TestNumberChanged;

    public override void _EnterTree()
    {
        Instance = this; //set a global (static) reference
    }
    public override void _Ready()
    {
        networkManagerSingleton = NetworkManager_Singleton.GetInstance();
        multiplayerPeer = Multiplayer.GetMultiplayerPeer();
    }
    
    [Rpc(RpcMode.AnyPeer)]
    private void TestRpc_Add()
    {
        AddCount++;
        GD.Print($"RTC running: addCount: {AddCount}");
        TestNumberChanged?.Invoke(AddCount);
    }
    public void MouseSyncTestRpc(Vector2 pokerPlacement)
    {
        Rpc(nameof(MouseSyncTest_Receive), pokerPlacement);
    }

    [Rpc(RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    private void MouseSyncTest_Receive(Vector2 pokerPlacement)
    {
        GD.Print($"Mouse Pos: {pokerPlacement}");
    }

    #region SlotPoker
    public void SlotPokerRpc(PokerInfo pokerInfo, Dictionary modifiers = null)
    {
        var pokerVector = new Vector2((int)pokerInfo.Suit, pokerInfo.Rank); //convert to Vector2 for safe data transfer
        if (modifiers != null)
        {
            SlotPokerMod_Send(pokerVector, modifiers);
        }
        else
        {
            SlotPoker_Send(pokerVector, modifiers);
            GD.Print($"Sending Poker without modifiers: {pokerVector}");
        }
    }
    
    [Rpc(RpcMode.AnyPeer)]
    private void SlotPoker_Send(Vector2 pokerInfo, Dictionary modifiers = null)
    {
        if (!CardGameHelperSingleton.IsPokerValid(pokerInfo))
        {
            GD.PrintErr("Invalid poker info");
            return;
        }
        Rpc(nameof(SlotPoker_Receive), pokerInfo);
    }

    // [Rpc(RpcMode.AnyPeer)]
    private void SlotPokerMod_Send(Vector2 pokerInfo, Dictionary modifiers)
    {
        if (!CardGameHelperSingleton.IsPokerValid(pokerInfo))
        {
            GD.PrintErr("Invalid poker info");
            return;
        }
        Rpc(nameof(SlotPokerMod_Receive), pokerInfo, modifiers);
    }

    [Rpc(RpcMode.AnyPeer)]
    private void SlotPokerMod_Receive(Vector2 pokerInfo, Dictionary modifiers)
    {
        GD.Print($"Receiving Poker: {pokerInfo}");
        GD.Print($"Modifiers: {modifiers}");
    }
    

    [Rpc(RpcMode.AnyPeer)]
    private void SlotPoker_Receive(Vector2 pokerInfo)
    {
        GD.Print($"Normal Poker received: {pokerInfo}");
    }
    #endregion

    #region TriggerPoker

    public void TriggerPokerRpc(PokerInfo pokerInfo, Dictionary modifiers = null)
    {
        var pokerVector = new Vector2((int)pokerInfo.Suit, pokerInfo.Rank);
        TriggerPoker_Send(pokerVector, modifiers);
    }
    private void TriggerPoker_Send(Vector2 pokerInfo, Dictionary modifiers = null){
        if (!CardGameHelperSingleton.IsPokerValid(pokerInfo)) return;
        Rpc(nameof(TriggerPoker_Receive), pokerInfo);
    }
    [Rpc(RpcMode.AnyPeer)]
    private void TriggerPoker_Receive(Vector2 pokerInfo, Dictionary modifiers)
    {
        GD.Print($"Normal Poker Triggered: {pokerInfo}");
        GD.Print($"Modifiers: {modifiers}");
    }
    #endregion
    
    #region SummonPoker

    public void TriggerPokerSummonRpc(PokerInfo pokerInfo, Dictionary modifiers, Vector2 pokerPlacement)
    {
        
    }
    // public void SyncPokerSummonRpc(PokerInfo pokerInfo, Dictionary modifiers)
    
    
    #endregion
    
    #region ReleaseHandSlots

    public void ReleaseHandSlotsRpc(PokerHandType pokerHandType, ReleaseMode releaseMode)
    {
        ReleaseHandSlots_Send(pokerHandType, releaseMode);
    }

    private void ReleaseHandSlots_Send(PokerHandType pokerHandType, ReleaseMode releaseMode)
    {
        var packedVector = new Vector2((int)pokerHandType, (int)releaseMode);
        Rpc(nameof(ReleaseHandSlots_Receive), packedVector);
    }

    [Rpc(RpcMode.AnyPeer)]
    private void ReleaseHandSlots_Receive(Vector2 packedVector)
    {
        GD.Print($"Receiving Poker hand type: {(PokerHandType)packedVector.X})");
        GD.Print($"Release Mode: {(ReleaseMode)packedVector.Y}");
    }
    #endregion
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("menu_confirm"))
        {
            var connectionStatus = multiplayerPeer.GetConnectionStatus();
            if(connectionStatus != MultiplayerPeer.ConnectionStatus.Connected) return;
            Rpc(nameof(TestRpc_Add));
        }
    }
    
    
    public static RpcManager GetInstance()
    {
        if (Instance == null)
        {
            GD.PrintErr("No RpcManager found");
        }
        return Instance;
    }
}
