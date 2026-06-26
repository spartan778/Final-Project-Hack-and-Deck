using Godot;
using System;

public partial class ConnectionMenu : Control
{
    private NetworkManager_Singleton networkManagerSingleton;
    [Export] private CheckButton isHostButton;
    [Export] private Button connectButton, makeOfferButton, testOfflineButton;
    [Export] private Label infoLabel;

    public override void _EnterTree()
    {
        // networkManagerSingleton = GetNode<NetworkManager_Singleton>("/root/NetworkManagerSingleton");
        // GD.Print($"Node Ref: {networkManagerSingleton.Name}");
    }

    public override void _Ready()
    {
        networkManagerSingleton = NetworkManager_Singleton.GetInstance();
        makeOfferButton.Disabled = true;
        isHostButton.SetPressedNoSignal(false);
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        isHostButton.Pressed += OnIsHostButtonPressed;
        connectButton.Pressed += OnConnectButtonPressed;
        makeOfferButton.Pressed += OnMakeOfferButtonPressed;
        networkManagerSingleton.SignalServerConnected += OnSignalServerConnected;
        networkManagerSingleton.PlayerMatched += OnPlayerMatched;
        networkManagerSingleton.PlayerCountChanged += OnPlayerCountChanged;
        networkManagerSingleton.RtcConnected += OnRtcConnected;
        testOfflineButton.Pressed += OnTestOfflinePressed;
    }
    
    private void OnTestOfflinePressed()
    {
        SceneManager.Instance.PrepareMainGameScene();
        SceneManager.Instance.ChangeToMainGameScene();
    }
    private void OnMakeOfferButtonPressed()
    {
        networkManagerSingleton.StartRtcProcess();
    }

    private void OnConnectButtonPressed()
    {
        networkManagerSingleton.StartSignalingConnection();
        connectButton.Disabled = true;
        infoLabel.Text = "Connecting to server...";
    }

    private void OnIsHostButtonPressed()
    {
        var state= isHostButton.ButtonPressed;
        var text = state? "Host" : "Client";
        infoLabel.Text = $"Joining as: {text}";
        networkManagerSingleton.SetJoinAsHost(state);
    }
    
    private void OnSignalServerConnected()
    {
        connectButton.Disabled = true;
        infoLabel.Text = "Signal Server Connected," +
                         "\n waiting for the other player";
    }

    private void OnPlayerMatched()
    {
        if (networkManagerSingleton.IsHost)
        {
            infoLabel.Text = "Player Matched, Click [Make Offer] to create P2P connection as Host";
            makeOfferButton.Disabled = false;
        }
        else
        {
            infoLabel.Text = "Player Matched, wait for Host to start P2P connection";
        }
        SceneManager.Instance.PrepareMainGameScene();
    }

    private void OnRtcConnected()
    {
        var state= isHostButton.ButtonPressed;
        var text = state? "Host" : "Client";
        infoLabel.Text = $"P2P connection established, you are playing as: {text}";
        makeOfferButton.Disabled = true;
        SceneManager.Instance.ChangeToMainGameScene();
    }

    private void OnPlayerCountChanged(int playerCount)
    {
        GD.Print($"Current Player Count: {playerCount}");
    }
}
