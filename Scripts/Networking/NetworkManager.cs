using Godot;
using System;
using System.Collections;
using HCoroutines;
using Godot.Collections; 
using Array = Godot.Collections.Array;

public partial class NetworkManager : Node
{
	private WebRtcPeerConnection peerConnection;
	private WebRtcMultiplayerPeer rtcMultiplayerPeer;
	private WebSocketPeer webSocket = new WebSocketPeer();
	
	
	[Export] private CheckButton isHostButton;
	private bool isHost;
	[Export] private Button connectButton;
	[Export] private Label infoLabel;
	
	private bool offerHandled;
	private string roomName = "PlayRoom";
	private int deviceId;
	private int peerId;

	private Json jsonHelper;

	public override void _Ready()
	{
		ConnectSignals();
		PrepareConnection();
	}

	private void ConnectSignals()
	{
		connectButton.Pressed += StartSignalingConnection;
	}

	private void PrepareConnection()
	{
		jsonHelper = new Json();
		// webSocket.ConnectToUrl("wss://echo.websocket.org"); //free relay service
		// webSocket.ConnectToUrl("ws://localhost:8080");
		var config = new Dictionary
		{
			{ "iceServers", new Array
				{
					new Dictionary { { "urls", "stun:stun.l.google.com:19302" } }, // fallback free google stun service
					new Dictionary {
						{ "urls", "turn:global.relay.metered.ca:80" }, //registered (free) TURN service from Metered
						{ "username", "1268560a6c410cc2b0dee1f3" },
						{ "credential", "CF7DHme8j4F+IYpn" }
					},
					new Dictionary { 
						{ "urls", "turn:global.relay.metered.ca:443" },
						{ "username", "1268560a6c410cc2b0dee1f3" },
						{ "credential", "CF7DHme8j4F+IYpn" }
					},
				}
			}
		};
		peerConnection = new WebRtcPeerConnection();
		peerConnection.Initialize(config);
		peerConnection.SessionDescriptionCreated += (type, sdp) => //SDP process
		{
			GD.Print("SessionDescriptionCreated");
			peerConnection.SetLocalDescription(type, sdp);
			SendSignal(new Dictionary
			{
				{ "room", roomName },
				{ "type", type },
				{ "sdp", sdp }
			});
		};

		peerConnection.IceCandidateCreated += (media, index, name) => //ICE process
		{
			GD.Print("IceCandidateCreated");
			SendSignal(new Dictionary
			{
				{ "room", roomName },

				{ "type", "ice" },
				{ "media", media },
				{ "index", index },
				{ "name", name }
			});
		};
		
	}

	public override void _Process(double delta)
	{
		webSocket.Poll();
		peerConnection.Poll();
		
		// GD.Print(webSocket.GetReadyState());
		if (webSocket.GetReadyState() == WebSocketPeer.State.Open)
		{
			GD.Print("webSocket connected");
			while (webSocket.GetAvailablePacketCount() > 0)
			{
				GD.Print("WS: message received");
				var msg = webSocket.GetPacket().GetStringFromUtf8();
				HandleSignal(msg);
			}
		}
	}
	
	private void SendSignal(Dictionary data)
	{
		webSocket.SendText(Json.Stringify(data));
	}

	private void HandleSignal(string msg)
	{
		var err = jsonHelper.Parse(msg);
		if (err != Error.Ok)
		{
			GD.PrintErr("JSON parse error: ", jsonHelper.GetErrorMessage());
			return;
		}
		var data = Json.ParseString(msg).AsGodotDictionary(); //convert JSON to Dictionary
		
		if(!data.TryGetValue("room", out var roomValue)) return; // Block all message with a "Room"
		if(roomValue.ToString() !=  roomName) return;
		var type = data["type"].ToString();
		GD.Print($"HandlingSignal type: {type}");
		if (type is "offer" or "answer") // handle first stage
		{
			peerConnection.SetRemoteDescription(type, data["sdp"].ToString());
			if (type is "offer" or "answer")
			{
				var sdp = data["sdp"].ToString();
				GD.Print("Received: ", type);
				peerConnection.SetRemoteDescription(type, sdp);
			}else if (type == "ice")
			{
				peerConnection.AddIceCandidate( // media, index , name
					data["media"].ToString(),
					(int)data["index"],
					data["name"].ToString()
				);
			}else if (type == "playerCountUpdate")
			{
				var playerCount = (int)data["count"];
				GD.Print($"PlayerCount: {playerCount}");
			}
		}
	}

	private async void StartSignalingConnection()
	{
		webSocket.ConnectToUrl("ws://localhost:8080");
		GD.Print("webSocket connecting");
		isHost = isHostButton.ButtonPressed;
		GD.Print("Is Host: " + isHost);
		
		
		while (webSocket.GetReadyState() != WebSocketPeer.State.Open) //wait until Web Socket is Open
		{
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		SendJoinMsg();
		if (isHost) //switch ID to match on two peers
		{
			deviceId = 1;
			peerId = 2;
		}
		else
		{
			deviceId = 2;
			peerId = 1;
		}
		
		rtcMultiplayerPeer = new WebRtcMultiplayerPeer();
		rtcMultiplayerPeer.CreateMesh(deviceId);
		rtcMultiplayerPeer.AddPeer(peerConnection,peerId);
		Multiplayer.MultiplayerPeer = rtcMultiplayerPeer; //attach peer to Godot high level API (RPC)
		if (isHost)
		{
			Co.Delay(2f); //CRITICAL delay to make sure offer is sent when server is ready
			peerConnection.CreateOffer();
		}
		Co.Run(OnDurationRepeat(2f));
	}

	private IEnumerator OnDurationRepeat(float duration)
	{
		while (true)
		{
			float timer = 0;
			while (timer<duration)
			{
				timer += (float)GetProcessDeltaTime();
				yield return null;
			}
			// GD.Print($"running every: {duration} seconds");
			// GD.Print("WebSocket State: " +webSocket.GetReadyState());
		}
	}
	
	private void SendJoinMsg() //sending dummy message to make sure both player are in the room
	{
		SendSignal(new Dictionary
		{
			{ "room", roomName },
			{ "type", "join" }
		});
	}
}
