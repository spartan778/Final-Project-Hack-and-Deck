using Godot;
using System;
using System.Collections;
using HCoroutines;
using Godot.Collections; 
using Array = Godot.Collections.Array;

public partial class NetworkManager_Singleton : Node
{
	private static NetworkManager_Singleton _instance;
	private RpcManager rpcManager;
	
	private WebRtcPeerConnection peerConnection;
	private WebRtcMultiplayerPeer rtcMultiplayerPeer;
	public WebRtcMultiplayerPeer RtcMultiplayerPeer => rtcMultiplayerPeer;
	private WebSocketPeer webSocket = new WebSocketPeer();
	private int addCount = 0;
	private bool isHost;
	public bool IsHost => isHost;
	public Action SignalServerConnected, PlayerMatched, RtcConnected;
	public Action<int> PlayerCountChanged;

	[Export] private string localAddress = "ws://localhost:8080";
	[Export] private string onlineAddress = "wss://final-project-hack-and-deck.onrender.com/";
	[Export] private bool testLocal;
	private bool offerHandled;
	[Export] private string roomName = "PlayRoom";
	private int deviceId;
	private int peerId;

	private Json jsonHelper;

	public override void _EnterTree()
	{
		_instance = this; //set a global (static) reference
	}

	public override void _Ready()
	{
		ConnectSignals();
		PrepareConnection();
		rpcManager = RpcManager.GetInstance();
	}
	
	private void ConnectSignals()
	{
		Multiplayer.PeerConnected += (id) => // inform player about connection status
		{
			GD.Print($"Peer connected ID: {id}");
			if (isHost)
			{
				
				GD.Print($"Playing as host (ID: {deviceId})");
			}
			else
			{
				GD.Print($"Playing as client (ID: {deviceId})"); ;
			}
			RtcConnected?.Invoke();
		};
		Multiplayer.PeerDisconnected += (id) => // Debugging with peer id
		{
			GD.Print($"Peer disconnected ID: {id}");
		};
	}

	private void PrepareConnection()
	{
		jsonHelper = new Json();
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
			GD.Print($"ICE: {name}");
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
		if (webSocket.GetReadyState() == WebSocketPeer.State.Open)
		{
			while (webSocket.GetAvailablePacketCount() > 0)
			{
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
		
		if(!data.TryGetValue("room", out var roomValue)) return; // Block all message without a "Room"
		if(roomValue.ToString() !=  roomName) return;
		var type = data["type"].ToString();
		GD.Print($"HandlingSignal type: {type}");
		switch (type) // Handle message by type
		{
			case "playerCountUpdate":
			{
				var playerCount = (int)data["count"];
				GD.Print($"PlayerCount: {playerCount}");
				PlayerCountChanged.Invoke(playerCount);
				break;
			}
			case "matchReady":
			{
				if (isHost)
				{
					GD.Print($"Player Matched, Press Make Offer to start game");
				}
				else
				{
					GD.Print($"Player Matched, wait for host to start game");
				}
				PlayerMatched?.Invoke();
				break;
			}
			// handle first stage (depending on host or client)
			case "offer" or "answer":
			{
				peerConnection.SetRemoteDescription(type, data["sdp"].ToString());
				GD.Print($"RTC msg type: {type}");
				break;
			}
			case "ice":
			{
				peerConnection.AddIceCandidate( // media, index , name
					data["media"].ToString(),
					(int)data["index"],
					data["name"].ToString()
				);
				break;
			}
		}
	}
	
	private string GetServerAddress() //return connection address depending on if testing locally
	{
		return testLocal ? localAddress : onlineAddress;
	}

	public async void StartSignalingConnection()
	{
		webSocket.ConnectToUrl(GetServerAddress());
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
		GD.Print("Signal Server connected");
		SignalServerConnected?.Invoke();
		// Co.Run(OnDurationRepeat(2f));
	}

	public void StartRtcProcess()
	{
		peerConnection.CreateOffer();
	}
	
	public void SetJoinAsHost(bool value)
	{
		isHost = value;
		if (isHost)
		{
			GD.Print("Joining as Host");
		}
		else
		{
			GD.Print("Joining as Client");
		}
	}

	private IEnumerator OnDurationRepeat(float duration) //mostly used for debugging
	{
		while (true)
		{
			float timer = 0;
			while (timer<duration)
			{
				timer += (float)GetProcessDeltaTime();
				yield return null;
			}

			var connectionStatus= rtcMultiplayerPeer.GetConnectionStatus();
			var peerAmount = rtcMultiplayerPeer.GetPeers().Count;
			
			GD.Print($"RTC Connection: {connectionStatus}");
			GD.Print($"RTC Peer Count: {peerAmount}");
		}
	}
	
	private void SendJoinMsg() //send joining message to make sure both player are in the room
	{
		SendSignal(new Dictionary
		{
			{ "room", roomName },
			{ "type", "join" }
		});
	}

	public static NetworkManager_Singleton GetInstance()
	{
		if (_instance == null)
		{
			GD.PrintErr("No NetworkManager_Singleton found");
		}
		return _instance;
	}
}
