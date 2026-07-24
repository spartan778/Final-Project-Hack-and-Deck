using Godot;
using System;

public partial class CardGameUIManager : Control
{
    [Export] private Label testLabel;

    public override void _Ready()
    {
        var rpcManager = RpcManager.GetInstance();
        rpcManager.TestNumberChanged += OnTestNumberChanged;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
    }

    private void OnTestNumberChanged(int newValue)
    {
        testLabel.Text = $"Current count: {newValue}";
    }

    private void OnPeerDisconnected(long id)
    {
        testLabel.Text = "Player Disconnected";
    }
}
