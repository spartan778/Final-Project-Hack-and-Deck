using Godot;
using System;

public partial class ConnectionMenu : Control
{
    private NetworkManager_Singleton networkManagerSingleton;
    [Export] private CheckButton isHostButton;
    [Export] private Button joinRoomButton, startGameButton, testOfflineButton;
    [Export] private Label infoLabel,titleLabel;
    [Export] private LineEdit roomIdField;
    [Export] private Control roomIdModule;

    public override void _EnterTree()
    {
        // networkManagerSingleton = GetNode<NetworkManager_Singleton>("/root/NetworkManagerSingleton");
        // GD.Print($"Node Ref: {networkManagerSingleton.Name}");
    }

    public override void _Ready()
    {
        networkManagerSingleton = NetworkManager_Singleton.GetInstance();
        startGameButton.Disabled = true;
        joinRoomButton.Disabled = true;
        isHostButton.SetPressedNoSignal(false);
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        isHostButton.Pressed += OnIsHostButtonPressed;
        joinRoomButton.Pressed += OnJoinGameButtonPressed;
        startGameButton.Pressed += OnStartGameButtonPressed;
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
    private void OnStartGameButtonPressed()
    {
        networkManagerSingleton.StartRtcProcess();
    }

    private void OnJoinGameButtonPressed()
    {
        networkManagerSingleton.JoinGameRoom(roomIdField.Text);
        joinRoomButton.Disabled = true;
        isHostButton.Disabled = true;
        infoLabel.Text = "Joining Room...";
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
        joinRoomButton.Disabled = false;
        titleLabel.Text = "Signal Server is online";
    }

    private void OnPlayerMatched()
    {
        if (networkManagerSingleton.IsHost)
        {
            infoLabel.Text = "Player Matched, Click [Start Game] to Start the game";
            startGameButton.Disabled = false;
            joinRoomButton.Disabled = true;
        }
        else
        {
            infoLabel.Text = "Player Matched, please wait for the Host to start the game";
            joinRoomButton.Disabled = true;
        }
        roomIdModule.Visible = false;
        SceneManager.Instance.PrepareMainGameScene();
    }

    private void OnRtcConnected()
    {
        var state= isHostButton.ButtonPressed;
        var text = state? "Host" : "Client";
        infoLabel.Text = $"P2P connection established, you are playing as: {text}";
        startGameButton.Disabled = true;
        SceneManager.Instance.ChangeToMainGameScene();
    }

    private void OnPlayerCountChanged(int playerCount)
    {
        GD.Print($"Current Player Count: {playerCount}");
        infoLabel.Text = $"Current Player Count: {playerCount}";
        if (playerCount == 2)
        {
            
        }
    }
}
