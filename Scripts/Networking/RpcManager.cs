using Godot;
using System;
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

    public void SlotPokerRpc(PokerInfo pokerInfo, Array modifiers = null)
    {
        var pokerVector = new Vector2((int)pokerInfo.CardSuit, pokerInfo.CardValue);
        SlotPoker_Send(pokerVector, modifiers);
        GD.Print($"Sending Poker: {pokerVector}");
    }
    
    [Rpc(RpcMode.AnyPeer)]
    private void SlotPoker_Send(Vector2 pokerInfo, Array modifiers = null)
    {
        if (pokerInfo.X < 0 || pokerInfo.X > 3 || pokerInfo.Y < 0 || pokerInfo.Y > 12)
        {
            GD.PrintErr("Invalid poker info");
            return;
        }
        Rpc(nameof(SlotPoker_Receive), pokerInfo);
    }

    [Rpc(RpcMode.AnyPeer)]
    private void SlotPoker_Receive(Vector2 pokerInfo)
    {
        GD.Print($"Normal Poker received: {pokerInfo}");
    }
	
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
