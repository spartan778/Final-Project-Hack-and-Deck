using Godot;
using System;
using System.Threading;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class NetworkManager_Demo : Node
{
	// (UNUSED) referenced official Godot WebRTC tutorial (converted GDScript demo into c# and adding TURN for testing)
	private WebRtcPeerConnection p1;
	private WebRtcPeerConnection p2;
	private WebRtcDataChannel ch1;
	private WebRtcDataChannel ch2;
	
	public override async void _Ready()
	{
		
		var config = new Dictionary
		{
			{ "iceServers", new Array
				{
					new Dictionary { { "urls", "stun:stun.l.google.com:19302" } },
					new Dictionary { 
						{ "urls", "turn:openrelay.metered.ca:80" },
						{ "username", "1268560a6c410cc2b0dee1f3" },
						{ "credential", "CF7DHme8j4F+IYpn" }
					},
					new Dictionary { 
						{ "urls", "turn:global.relay.metered.ca:443" },
						{ "username", "1268560a6c410cc2b0dee1f3" },
						{ "credential", "CF7DHme8j4F+IYpn" }
					},
					new Dictionary { 
						{ "urls", "turns:global.relay.metered.ca:443" },
						{ "username", "1268560a6c410cc2b0dee1f3" },
						{ "credential", "CF7DHme8j4F+IYpn" }
					},
				}
			
			
			}
		};

		p1 = new WebRtcPeerConnection();
		p2 = new WebRtcPeerConnection();

		p1.Initialize(config);
		p2.Initialize(config);


		// p1.Initialize();
		// p2.Initialize();
		ch1 = p1.CreateDataChannel("chat", new Dictionary
		{
			{"id", 1}, {"negotiated", true}
		});
		ch2 =p2.CreateDataChannel("chat", new Dictionary
		{
			{"id", 1}, {"negotiated", true}
		});
		
		p1.SessionDescriptionCreated += (type, sdp) =>
		{
			p1.SetLocalDescription(type, sdp);
			p2.SetRemoteDescription(type, sdp);
		};
		p1.IceCandidateCreated += (media, index, name) =>
		{
			p2.AddIceCandidate(media, (int)index, name);
		};
		
		p2.SessionDescriptionCreated += (type, sdp) =>
		{
			p2.SetLocalDescription(type, sdp);
			p1.SetRemoteDescription(type, sdp);
		};
		p2.IceCandidateCreated += (media, index, name) =>
		{
			p1.AddIceCandidate(media, (int)index, name);
			
		};

		p1.CreateOffer();
		
		await ToSignal(GetTree().CreateTimer(3.0), SceneTreeTimer.SignalName.Timeout);
		ch1.PutPacket("hello from P1 (ABC)".ToUtf8Buffer());
		
		await ToSignal(GetTree().CreateTimer(3.0), SceneTreeTimer.SignalName.Timeout);
		ch2.PutPacket("hello from P2 (DEF)".ToUtf8Buffer());
	}

	public override void _Process(double delta)
	{
		p1.Poll();
		p2.Poll();

		if(ch1.GetReadyState() == WebRtcDataChannel.ChannelState.Open && ch1.GetAvailablePacketCount() > 0)
		{
			GD.Print("P1 received:" + ch1.GetPacket().GetStringFromUtf8());
		}

		if (ch2.GetReadyState() == WebRtcDataChannel.ChannelState.Open && ch2.GetAvailablePacketCount() > 0)
		{
			GD.Print("P2 received:" + ch2.GetPacket().GetStringFromUtf8());
		}
	}
}
