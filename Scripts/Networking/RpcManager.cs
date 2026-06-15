using Godot;
using System;

public partial class RpcManager : Node
{
    private NetworkManager_Singleton networkManagerSingleton;
    private static RpcManager _instance;
    private MultiplayerPeer multiplayerPeer;

    public int AddCount { get; private set; } = 0;
    
    public Action<int> TestNumberChanged;

    public override void _EnterTree()
    {
        _instance = this; //set a global (static) reference
    }
    public override void _Ready()
    {
        networkManagerSingleton = NetworkManager_Singleton.GetInstance();
        multiplayerPeer = Multiplayer.GetMultiplayerPeer();
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void TestRpc_Add()
    {
        AddCount++;
        GD.Print($"RTC running: addCount: {AddCount}");
        TestNumberChanged?.Invoke(AddCount);
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
        if (_instance == null)
        {
            GD.PrintErr("No RpcManager found");
        }
        return _instance;
    }
}
