using Godot;
using System;

public partial class CardGameUIManager : Control
{
    [Export] private Label mainLabel;
    [Export] private Label subLabel;

    public override void _Ready()
    {
        var rpcManager = RpcManager.GetInstance();
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        
    }
    

    private void OnPeerDisconnected(long id)
    {
        subLabel.Text = "Player Disconnected";
    }
}
