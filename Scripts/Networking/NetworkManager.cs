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
	[Export] private Button connectButton, makeOfferButton;
	[Export] private Label infoLabel;
	
	private bool offerHandled;
	private string roomName = "PlayRoom";
	private int deviceId;
	private int peerId;

	private Json jsonHelper;

	public override void _Ready()
	{
		makeOfferButton.Disabled = true;
		
		ConnectSignals();
		PrepareConnection();
	}
	
	private void ConnectSignals()
	{
		connectButton.Pressed += StartSignalingConnection;
		makeOfferButton.Pressed += StartRtcProcess;
		
		Multiplayer.PeerConnected += (id) => // inform player about connection status
		{
			GD.Print($"Peer connected ID: {id}");
			if (isHost)
			{
				infoLabel.Text = $"Playing as host (ID: {id})";
			}
			else
			{
				infoLabel.Text = $"Playing as client (ID: {id})";
			}
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
					// new Dictionary { { "urls", "stun:stun.l.google.com:19302" } }, // fallback free google stun service
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
		
		if(!data.TryGetValue("room", out var roomValue)) return; // Block all message with a "Room"
		if(roomValue.ToString() !=  roomName) return;
		var type = data["type"].ToString();
		GD.Print($"HandlingSignal type: {type}");
		switch (type)
		{
			case "playerCountUpdate":
			{
				var playerCount = (int)data["count"];
				GD.Print($"PlayerCount: {playerCount}");
				break;
			}
			case "matchReady":
			{
				if (isHost)
				{
					infoLabel.Text = $"Player Matched, Press Make Offer to start game";
					connectButton.Disabled = true;
					makeOfferButton.Disabled = false;
				}
				else
				{
					infoLabel.Text = $"Player Matched, wait for host to start game";
					connectButton.Disabled = true;
					makeOfferButton.Disabled = true;
				}
				break;
			}
			// handle first stage
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

	private async void StartSignalingConnection()
	{
		webSocket.ConnectToUrl("wss://final-project-hack-and-deck.onrender.com/");
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
		if (!isHost)
		{
			infoLabel.Text = $"Waiting for offer as client";
			connectButton.Disabled = true;
		}
		else
		{
			infoLabel.Text = $"Connecting as host";
			connectButton.Disabled = true;
			makeOfferButton.Disabled = false;
		}
		Co.Run(OnDurationRepeat(2f));
	}

	private void StartRtcProcess()
	{
		peerConnection.CreateOffer();
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
}
