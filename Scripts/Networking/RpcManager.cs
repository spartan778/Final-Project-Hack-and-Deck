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

    #region SlotPoker
    public void SlotPokerRpc(PokerInfo pokerInfo, Dictionary modifiers = null)
    {
        var pokerVector = new Vector2((int)pokerInfo.Suit, pokerInfo.Rank); //convert to Vector2 for safe data transfer
        if (modifiers != null)
        {
            SlotPokerMod_Send(pokerVector, modifiers);
        }
        SlotPoker_Send(pokerVector, modifiers);
        GD.Print($"Sending Poker: {pokerVector}");
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

    [Rpc(RpcMode.AnyPeer)]
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

    public void TriggerPokerRpc(PokerInfo pokerInfo, Array modifiers = null)
    {
        var pokerVector = new Vector2((int)pokerInfo.Suit, pokerInfo.Rank);
        TriggerPoker_Send(pokerVector, modifiers);
    }
    [Rpc(RpcMode.AnyPeer)]
    private void TriggerPoker_Send(Vector2 pokerInfo, Array modifiers = null){
        if (!CardGameHelperSingleton.IsPokerValid(pokerInfo)) return;
        Rpc(nameof(TriggerPoker_Receive), pokerInfo);
    }
    [Rpc(RpcMode.AnyPeer)]
    private void TriggerPoker_Receive(Vector2 pokerInfo)
    {
        GD.Print($"Normal Poker Triggered: {pokerInfo}");
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
