using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Multiplayer
{
	public class NetTransportUNet : NetTransport
	{
		private int serverConnectionId;

		private int hostId;

		private int reliableChannelId;

		private int unreliableChannelId;

		public override bool SupportsLobbyListings()
		{
			return false;
		}

		private int GetConnectionID(NetHost host)
		{
			return (int)host.connection;
		}

		public override bool ConnectionEquals(object connection, NetHost host)
		{
			return (int)connection == GetConnectionID(host);
		}

		public override string GetMyName()
		{
			return string.Empty;
		}

		public override void Init()
		{
			NetworkTransport.Init();
		}

		public override void StartServer(OnStartHostDelegate callback, object sessionArgs = null)
		{
			Application.runInBackground = true;
			NetworkTransport.Init();
			ConnectionConfig connectionConfig = new ConnectionConfig();
			reliableChannelId = connectionConfig.AddChannel(QosType.Reliable);
			unreliableChannelId = connectionConfig.AddChannel(QosType.Unreliable);
			HostTopology topology = new HostTopology(connectionConfig, 4);
			hostId = NetworkTransport.AddHost(topology, 8888);
			if (hostId < 0)
			{
				callback("Server socket creation failed!");
			}
			else
			{
				callback(null);
			}
		}

		public override void JoinGame(object serverAddress, OnJoinGameDelegate callback)
		{
			Application.runInBackground = true;
			NetworkTransport.Init();
			ConnectionConfig connectionConfig = new ConnectionConfig();
			reliableChannelId = connectionConfig.AddChannel(QosType.Reliable);
			unreliableChannelId = connectionConfig.AddChannel(QosType.Unreliable);
			HostTopology topology = new HostTopology(connectionConfig, 4);
			hostId = NetworkTransport.AddHost(topology);
			if (hostId < 0)
			{
				callback(null, "Server socket creation failed!");
				return;
			}
			serverConnectionId = NetworkTransport.Connect(hostId, (string)serverAddress, 8888, 0, out byte error);
			if (error != 0)
			{
				NetworkError networkError = (NetworkError)error;
				callback(null, "Error: " + networkError.ToString());
			}
			else
			{
				callback(serverConnectionId, null);
			}
		}

		public override void SendReliable(NetHost host, byte[] data, int len)
		{
			NetworkTransport.Send(hostId, GetConnectionID(host), reliableChannelId, data, len, out byte error);
			if (error != 0)
			{
				NetworkError networkError = (NetworkError)error;
				Debug.LogError("Error: " + networkError.ToString());
			}
		}

		public override void SendUnreliable(NetHost host, byte[] data, int len)
		{
			NetworkTransport.Send(hostId, GetConnectionID(host), unreliableChannelId, data, len, out byte error);
			if (error != 0)
			{
				NetworkError networkError = (NetworkError)error;
				Debug.LogError("Error: " + networkError.ToString());
			}
		}

		public override void OnUpdate()
		{
			NetworkEventType networkEventType = NetworkEventType.DataEvent;
			do
			{
				byte[] array = new byte[1024];
				networkEventType = NetworkTransport.Receive(out int _, out int connectionId, out int _, array, array.Length, out int receivedSize, out byte _);
				switch (networkEventType)
				{
				case NetworkEventType.DataEvent:
					NetGame.instance.OnData(connectionId, array, -1, receivedSize);
					break;
				case NetworkEventType.ConnectEvent:
					NetGame.instance.OnConnect(connectionId);
					break;
				case NetworkEventType.DisconnectEvent:
					NetGame.instance.OnDisconnect(connectionId);
					break;
				}
			}
			while (networkEventType != NetworkEventType.Nothing);
		}

		public override void StopServer()
		{
			throw new NotImplementedException();
		}

		public override void LeaveGame()
		{
			throw new NotImplementedException();
		}

		public override void StartThread()
		{
			throw new NotImplementedException();
		}

		public override void StopThread()
		{
			throw new NotImplementedException();
		}
	}
}
