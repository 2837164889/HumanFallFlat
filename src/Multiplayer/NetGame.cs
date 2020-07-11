using HumanAPI;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Multiplayer
{
	public class NetGame : MonoBehaviour, IDependency
	{
		public static string lobbyLevel = "Lobby";

		public static NetGame instance;

		private static object stateLock = new object();

		public static bool isNetStarting;

		public static bool isNetStarted;

		public static bool isServer;

		public static bool isClient;

		public static bool isLocal;

		[NonSerialized]
		public NetHost server;

		[NonSerialized]
		public NetHost local;

		[NonSerialized]
		public List<NetHost> allclients = new List<NetHost>();

		[NonSerialized]
		public List<NetHost> readyclients = new List<NetHost>();

		[NonSerialized]
		public List<NetPlayer> players = new List<NetPlayer>();

		[NonSerialized]
		public NetTransport transport;

		[NonSerialized]
		public string lobbyTitle;

		public static bool netlog;

		private object sendLock = new object();

		public static uint readLevelInstance = uint.MaxValue;

		public ulong currentLevel;

		[NonSerialized]
		public WorkshopItemSource currentLevelType;

		[NonSerialized]
		public uint currentLevelHash;

		[NonSerialized]
		public bool currentLevelStarted;

		public static int serverFrameId;

		private WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

		private static ulong oldLevelID = 3735928559uL;

		public static byte nextLevelInstanceID = 1;

		public static byte currentLevelInstanceID = 0;

		private static byte levelInstanceState = 0;

		private static NetFloat encoder = new NetFloat(1000f, 24, 1, 10);

		[NonSerialized]
		public bool multithreading;

		[NonSerialized]
		public uint ignoreDeltasMask;

		public const int Container_BaseOverhead = 35;

		public const int Container_PerScopeOverhead = 38;

		public const int Container_FrameIdBits = 22;

		public BitRateCounter sendBps = new BitRateCounter();

		public BitRateCounter recvBps = new BitRateCounter();

		public LatencyCounter clientLatency = new LatencyCounter();

		public LatencyCounter clientBuffer = new LatencyCounter();

		private NetNotificationQueue[] notifyQueues;

		public static List<object> kickedUsers = new List<object>();

		internal static bool friendly = false;

		internal static bool wireless = false;

		public bool isNetActive
		{
			get
			{
				lock (stateLock)
				{
					return isNetStarted || isNetStarting;
				}
			}
		}

		public static uint loadedLevel => (uint)((!instance.currentLevelStarted) ? 15u : instance.currentLevel);

		public int clientThreads
		{
			get
			{
				return PlayerPrefs.GetInt("clientThreads", 1);
			}
			set
			{
				PlayerPrefs.SetInt("clientThreads", value);
			}
		}

		public int serverThreads
		{
			get
			{
				return PlayerPrefs.GetInt("serverThreads", 2);
			}
			set
			{
				PlayerPrefs.SetInt("serverThreads", value);
			}
		}

		public event Action preUpdate;

		public static string FindName(string id)
		{
			foreach (NetPlayer player in instance.players)
			{
				if (player.skinUserId.Equals(id) && !string.IsNullOrEmpty(player.host.name))
				{
					return player.host.name;
				}
			}
			return null;
		}

		private NetHost FindAnyClient(object connection)
		{
			int i = 0;
			for (int count = allclients.Count; i < count; i++)
			{
				if (connection != null && transport.ConnectionEquals(connection, allclients[i]))
				{
					return allclients[i];
				}
			}
			return null;
		}

		private NetHost FindReadyClient(object connection)
		{
			int i = 0;
			for (int count = readyclients.Count; i < count; i++)
			{
				if (connection != null && transport.ConnectionEquals(connection, readyclients[i]))
				{
					return readyclients[i];
				}
			}
			return null;
		}

		public NetHost FindAnyHost(uint hostId)
		{
			if (hostId == 0)
			{
				return server;
			}
			int i = 0;
			for (int count = allclients.Count; i < count; i++)
			{
				if (allclients[i].hostId == hostId)
				{
					return allclients[i];
				}
			}
			return null;
		}

		public NetHost FindReadyHost(uint hostId)
		{
			if (hostId == 0)
			{
				return server;
			}
			int i = 0;
			for (int count = readyclients.Count; i < count; i++)
			{
				if (readyclients[i].hostId == hostId)
				{
					return readyclients[i];
				}
			}
			return null;
		}

		private void Awake()
		{
			instance = this;
			transport = base.gameObject.AddComponent<NetTransportSteam>();
			transport.Init();
			Shell.RegisterCommand("tclient", OnClientThreading, "tclient <on|off>\r\nToggle netcode processing in a thread on a client");
			Shell.RegisterCommand("tserver", OnServerThreading, "tserver <threadcount|off>\r\nToggle multithreaded netcode processing on a server\r\n\t<threadcount> - number of threads to use");
			Shell.RegisterCommand("netlog", OnNetLog, "netlog <on|off>\r\nToggle network logging (debugging)");
			netlog = (PlayerPrefs.GetInt("netlog", 0) > 0);
		}

		private void OnNetLog(string param)
		{
			if (!string.IsNullOrEmpty(param))
			{
				param = param.ToLowerInvariant();
				if ("off".Equals(param))
				{
					netlog = false;
				}
				else if ("on".Equals(param))
				{
					netlog = true;
				}
			}
			if (netlog)
			{
				Shell.Print("netlog on");
			}
			else
			{
				Shell.Print("netlog off");
			}
			PlayerPrefs.SetInt("netlog", netlog ? 1 : 0);
		}

		private void OnClientThreading(string param)
		{
			if (!string.IsNullOrEmpty(param))
			{
				param = param.ToLowerInvariant();
				int result = 0;
				if (int.TryParse(param, out result))
				{
					clientThreads = result;
				}
				if ("off".Equals(param))
				{
					clientThreads = 0;
				}
				else if ("on".Equals(param))
				{
					clientThreads = 1;
				}
			}
			Shell.Print("client worker threads " + clientThreads);
		}

		private void OnServerThreading(string param)
		{
			if (!string.IsNullOrEmpty(param))
			{
				param = param.ToLowerInvariant();
				int result = 0;
				if (int.TryParse(param, out result))
				{
					serverThreads = result;
				}
				if ("off".Equals(param))
				{
					serverThreads = 0;
				}
				else if ("on".Equals(param))
				{
					serverThreads = 2;
				}
			}
			Shell.Print("server worker threads " + serverThreads);
		}

		public static NetStream BeginMessage(NetMsgId id)
		{
			NetStream netStream = NetStream.AllocStream();
			netStream?.WriteMsgId(id);
			return netStream;
		}

		public void SendReliableToServer(NetStream stream)
		{
			SendReliable(server, stream);
		}

		public void SendUnreliableToServer(NetStream stream, int expectedContainerId)
		{
			SendUnreliable(server, stream, expectedContainerId);
		}

		public void SendReliable(NetHost host, NetStream stream)
		{
			lock (sendLock)
			{
				transport.SendReliable(host, stream.GetOriginalBuffer(), stream.UseBuffedSize());
				sendBps.ReportBits(stream.position);
			}
		}

		public void SendUnreliable(NetHost host, NetStream stream, int expectedContainerId)
		{
			lock (sendLock)
			{
				host.Buffer(stream, transport, expectedContainerId);
			}
		}

		public void StartLocalGame()
		{
			serverFrameId = 0;
			allclients.Clear();
			readyclients.Clear();
			local = (server = new NetHost(null, "local")
			{
				isLocal = true,
				isReady = true,
				hostId = 0u
			});
			isLocal = true;
			AddLocalPlayer();
			PlayerManager.SetSingle();
		}

		public void StopLocalGame()
		{
			for (int num = players.Count - 1; num >= 0; num--)
			{
				RemoveLocalPlayer(players[num]);
			}
			isLocal = false;
		}

		public void OnConnect(object connection)
		{
			if (!isServer)
			{
				return;
			}
			NetHost netHost = new NetHost(connection, NetMsgId.AddPlayer.ToString());
			netHost.isLocal = false;
			NetHost netHost2 = netHost;
			uint num = 1u;
			bool flag = false;
			while (!flag)
			{
				flag = true;
				for (int i = 0; i < allclients.Count; i++)
				{
					if (allclients[i].hostId == num)
					{
						num++;
						flag = false;
						break;
					}
				}
			}
			if (num >= 256)
			{
				throw new InvalidOperationException("Too many hosts?");
			}
			netHost2.hostId = num;
			allclients.Add(netHost2);
			NetNotificationQueue netNotificationQueue = null;
			int num2 = int.MaxValue;
			for (int j = 0; j < notifyQueues.Length; j++)
			{
				if (notifyQueues[j].clients.Count < num2)
				{
					netNotificationQueue = notifyQueues[j];
					num2 = netNotificationQueue.clients.Count;
				}
			}
			netNotificationQueue.clients.Add(netHost2);
			netHost2.notificationQueue = netNotificationQueue;
			NetScope.RemoveAllRemoteState(netHost2, setDying: false);
			OnServerConnect(netHost2);
		}

		public void OnDisconnect(object connection, bool suppressMessage = false)
		{
			if (isServer)
			{
				NetHost netHost = FindAnyClient(connection);
				if (netHost != null)
				{
					DestroyHostObjects(netHost);
					OnServerDisconnect(netHost);
				}
			}
			else
			{
				OnLostConnection(suppressMessage);
			}
		}

		public void OnLostConnection(bool suppressMessage = false)
		{
			App.instance.OnLostConnection(suppressMessage);
		}

		private void FixedUpdate()
		{
			sendBps.FrameComplete();
			recvBps.FrameComplete();
			clientLatency.FrameComplete();
			clientBuffer.FrameComplete();
			serverFrameId++;
			if (!isServer)
			{
			}
		}

		private void Update()
		{
		}

		private void LateUpdate()
		{
			if (isClient)
			{
			}
			if (isServer && !isNetStarted)
			{
			}
		}

		public IEnumerator PostSimulate()
		{
			while (this != null)
			{
				yield return fixedUpdate;
				ReplayRecorder.instance.PostSimulate();
				for (int i = 0; i < NetScope.all.Count; i++)
				{
					NetScope.all[i].Collect();
				}
				for (int j = 0; j < NetScope.all.Count; j++)
				{
					NetScope.all[j].PostSimulate();
				}
				for (int k = 0; k < local.players.Count; k++)
				{
					local.players[k].cameraController.PostSimulate();
				}
			}
		}

		public void PostFixedUpdate()
		{
			for (int i = 0; i < NetScope.all.Count; i++)
			{
				NetScope.all[i].PostFixedUpdate();
			}
			transport.FlushTxBufers(fixedUpdate: true);
		}

		public void PostUpdate()
		{
			if (isNetStarted)
			{
				for (int i = 0; i < NetScope.all.Count; i++)
				{
					NetScope.all[i].PostUpdate();
				}
				NetStream.TickPools();
			}
		}

		public void Flush()
		{
			if (isServer)
			{
				for (int i = 0; i < allclients.Count; i++)
				{
					allclients[i].FlushBuffer(transport);
				}
			}
			else if (server != null)
			{
				server.FlushBuffer(transport);
			}
			transport.FlushTxBufers(fixedUpdate: false);
		}

		public void PostLateUpdate()
		{
		}

		public void PreFixedUpdate()
		{
			for (int i = 0; i < NetScope.all.Count; i++)
			{
				NetScope.all[i].PreFixedUpdate();
			}
			if (this.preUpdate != null)
			{
				this.preUpdate();
			}
		}

		public void PreUpdate()
		{
			transport.OnUpdate();
		}

		public void PreLateUpdate()
		{
			ReplayRecorder.instance.PreLateUpdate();
			for (int i = 0; i < NetScope.all.Count; i++)
			{
				NetScope.all[i].Apply();
			}
		}

		public void Initialize()
		{
			instance = this;
			Dependencies.OnInitialized(this);
			StartCoroutine(PostSimulate());
		}

		public void DestroyEverything()
		{
			currentLevelInstanceID = 0;
			currentLevel = 0uL;
			currentLevelHash = 0u;
			currentLevelStarted = false;
			currentLevelType = WorkshopItemSource.BuiltIn;
			for (int i = 0; i < players.Count; i++)
			{
				players[i].DespawnPlayer();
			}
			lock (stateLock)
			{
				isNetStarting = (isServer = (isClient = (isNetStarted = false)));
			}
			server = (local = null);
			allclients.Clear();
			readyclients.Clear();
			players.Clear();
			StartLocalGame();
			NetStream.DiscardPools();
			Physics.autoSimulation = true;
		}

		public static void UpdateLevelInstanceID(ulong number, bool start, uint levelHash)
		{
			bool flag = false;
			if (start)
			{
				flag = (oldLevelID != number || levelInstanceState < 2 || levelHash == 0);
				levelInstanceState = (byte)((levelHash != 0) ? 3 : 2);
				oldLevelID = number;
			}
			else
			{
				flag = (levelInstanceState != 1);
				levelInstanceState = 1;
				oldLevelID = 3735928559uL;
			}
			if (flag)
			{
				if (++nextLevelInstanceID > 15)
				{
					nextLevelInstanceID = 1;
				}
				currentLevelInstanceID = 0;
			}
		}

		public void ServerLoadLobby(ulong number, WorkshopItemSource levelType, bool start, uint levelHash)
		{
			NetStream netStream = BeginMessage(NetMsgId.LoadLevel);
			try
			{
				uint x = (uint)(number >> 32);
				uint x2 = (uint)(number & uint.MaxValue);
				netStream.Write(v: true);
				netStream.Write(x, 4, 32);
				netStream.Write(x2, 4, 32);
				netStream.Write(levelHash, 32);
				for (int i = 0; i < readyclients.Count; i++)
				{
					SendReliable(readyclients[i], netStream);
				}
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		public void ServerLoadLevel(ulong number, WorkshopItemSource levelType, bool start, uint levelHash)
		{
			UpdateLevelInstanceID(number, start, levelHash);
			Options.lobbyLevel = (int)number;
			currentLevelStarted = start;
			currentLevelHash = levelHash;
			currentLevel = number;
			currentLevelType = levelType;
			instance.transport.UpdateServerLevel(number, levelType);
			NetStream netStream = BeginMessage(NetMsgId.LoadLevel);
			try
			{
				netStream.Write(v: false);
				netStream.Write((uint)currentLevel, 4, 32);
				netStream.Write(start);
				netStream.Write(levelHash, 32);
				netStream.Write((int)currentLevelType, 8);
				netStream.Write(nextLevelInstanceID, 4);
				for (int i = 0; i < readyclients.Count; i++)
				{
					SendReliable(readyclients[i], netStream);
				}
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		private void OnReceiveLevelAck(NetHost client, NetStream stream)
		{
			uint num = stream.ReadUInt32(4, 32);
			client.activeLevel = num;
		}

		private void OnLoadLevel(NetStream stream)
		{
			if (!stream.ReadBool())
			{
				currentLevel = stream.ReadUInt32(4, 32);
				currentLevelStarted = stream.ReadBool();
				currentLevelHash = stream.ReadUInt32(32);
				currentLevelType = (WorkshopItemSource)stream.ReadByte();
				currentLevelInstanceID = (byte)stream.ReadUInt32(4);
				Physics.autoSimulation = false;
				App.instance.OnRequestLoadLevel(currentLevel, currentLevelType, currentLevelStarted, currentLevelHash);
				if (isClient)
				{
					NetStream netStream = BeginMessage(NetMsgId.LoadLevel);
					try
					{
						netStream.Write((uint)currentLevel, 4, 32);
						SendReliableToServer(netStream);
					}
					finally
					{
						if (netStream != null)
						{
							netStream = netStream.Release();
						}
					}
				}
			}
			else
			{
				ulong num = (ulong)stream.ReadInt32(4, 32);
				ulong num2 = (ulong)stream.ReadInt32(4, 32);
				uint num3 = (uint)stream.ReadInt32(32);
				Game.multiplayerLobbyLevel = ((num << 32) | num2);
				App.instance.LobbyLoadLevel(Game.multiplayerLobbyLevel);
			}
		}

		[Conditional("NEVER_SET_THIS_DEFINE_1234")]
		public static void Assert(bool state, string msg = "")
		{
		}

		[Conditional("NEVER_SET_THIS_DEFINE_1234")]
		public static void Assert(bool state, string msg, int arg1)
		{
		}

		[Conditional("NEVER_SET_THIS_DEFINE_1234")]
		public static void Assert(bool state, string msg, object arg1)
		{
		}

		public void SendChatMessage(string msg)
		{
			string friendPersonaName = SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID());
			NetChat.OnReceive(local.hostId, friendPersonaName, msg);
			NetStream netStream = BeginMessage(NetMsgId.Chat);
			try
			{
				netStream.WriteNetId(instance.local.hostId);
				netStream.Write(friendPersonaName);
				netStream.Write(msg);
				if (isServer)
				{
					for (int i = 0; i < readyclients.Count; i++)
					{
						SendReliable(readyclients[i], netStream);
					}
				}
				else if (server != null && server.isReady)
				{
					SendReliableToServer(netStream);
				}
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		private void OnReceiveChatServer(NetHost client, NetStream stream)
		{
			uint clientId = stream.ReadNetId();
			string nick = stream.ReadString();
			string msg = stream.ReadString();
			NetChat.OnReceive(clientId, nick, msg);
			for (int i = 0; i < readyclients.Count; i++)
			{
				if (readyclients[i] != client)
				{
					SendReliable(readyclients[i], stream);
				}
			}
		}

		private void OnReceiveChatClient(NetStream stream)
		{
			uint clientId = stream.ReadNetId();
			string nick = stream.ReadString();
			string msg = stream.ReadString();
			NetChat.OnReceive(clientId, nick, msg);
		}

		public void NotifyUnlockAchievementServer(NetPlayer player, Achievement achievement, float amount = 0f)
		{
			if (isServer)
			{
				NetStream netStream = BeginMessage(NetMsgId.Achievement);
				try
				{
					netStream.WriteNetId((uint)achievement);
					encoder.CollectState(netStream, amount);
					if (player != null)
					{
						if (player.host.isReady)
						{
							SendReliable(player.host, netStream);
						}
					}
					else
					{
						for (int i = 0; i < readyclients.Count; i++)
						{
							SendReliable(readyclients[i], netStream);
						}
					}
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
			}
		}

		public void OnUnlockAchievementClient(NetStream stream)
		{
			if (isClient)
			{
				Achievement achievement = (Achievement)stream.ReadNetId();
				float amount = encoder.ApplyState(stream);
				StatsAndAchievements.OnNotifyUnlockAchievement(achievement, amount);
			}
		}

		public void JoinGame(object address)
		{
			lock (stateLock)
			{
				if (isNetActive)
				{
					return;
				}
				isNetStarting = true;
			}
			multithreading = (clientThreads > 0);
			transport.StartThread();
			transport.JoinGame(address, JoinGameCallback);
		}

		private void JoinGameCallback(object serverConnection, string error)
		{
			if (!string.IsNullOrEmpty(error))
			{
				lock (stateLock)
				{
					isNetStarting = false;
				}
				App.instance.OnConnectFail(error);
			}
		}

		private void SetMultiplayerLobbyID(string lobbyIDString)
		{
			ulong multiplayerLobbyLevel = 0uL;
			try
			{
				multiplayerLobbyLevel = ulong.Parse(lobbyIDString);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
			Game.multiplayerLobbyLevel = multiplayerLobbyLevel;
		}

		public void OnHelo(object serverConnection, NetStream stream)
		{
			uint num = stream.ReadNetId();
			string serverVersion = stream.ReadString();
			if (stream.ReadBool())
			{
				OnServerFull();
			}
			else if (num == VersionDisplay.netCode)
			{
				lock (stateLock)
				{
					isClient = true;
					isNetStarted = true;
					isNetStarting = false;
				}
				local = new NetHost(null, transport.GetMyName())
				{
					isLocal = true,
					isReady = false
				};
				allclients.Add(local);
				server = new NetHost(serverConnection, NetMsgId.Helo.ToString())
				{
					isLocal = false,
					hostId = 0u,
					isReady = false
				};
				local.hostId = stream.ReadNetId();
				server.name = stream.ReadString();
				lobbyLevel = stream.ReadString();
				string multiplayerLobbyID = stream.ReadString();
				SetMultiplayerLobbyID(multiplayerLobbyID);
				transport.LobbyConnectedFixup();
				NetStream netStream = BeginMessage(NetMsgId.Helo);
				try
				{
					netStream.WriteNetId(VersionDisplay.netCode);
					netStream.Write(VersionDisplay.fullVersion);
					netStream.Write(local.name);
					SendReliableToServer(netStream);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
				HumanAnalytics.instance.BeginMultiplayer(host: false);
				StopLocalGame();
				AddLocalPlayer();
				if (transport.IsRelayed(server))
				{
					App.instance.OnRelayConnection(server);
				}
			}
			else
			{
				OnIncompatibleServer(serverVersion);
			}
		}

		public void LeaveGame()
		{
			transport.LeaveGame();
			DestroyEverything();
		}

		public void OnIncompatibleServer(string serverVersion)
		{
			LeaveGame();
			App.instance.OnConnectFailVersion(serverVersion);
		}

		public void OnServerFull()
		{
			LeaveGame();
			App.instance.OnConnectFail("k_EChatRoomEnterResponseFull");
		}

		public void SendRequestRespawn()
		{
			NetStream netStream = BeginMessage(NetMsgId.Kick);
			try
			{
				if (server != null && server.isReady)
				{
					SendReliableToServer(netStream);
				}
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		public void OnData(object connection, byte[] buffer, int tier, int dataSize)
		{
			recvBps.ReportBits(dataSize * 8);
			NetStream netStream = NetStream.AllocStream(buffer, tier, 0, useRefCountOnBuffer: true);
			try
			{
				NetHost netHost;
				if (isServer)
				{
					netHost = FindAnyClient(connection);
					if (netHost != null)
					{
						goto IL_005c;
					}
					NetMsgId netMsgId = netStream.ReadMsgId();
					if (netMsgId != NetMsgId.Helo)
					{
						goto IL_005c;
					}
					OnConnect(connection);
					netHost = FindAnyClient(connection);
					OnClientHelo(netHost, netStream);
				}
				else
				{
					try
					{
						OnClientReceive(connection, netStream);
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogError("** OnClientReceive threw exception, 99.9% this is the buffer overflow due to applying data to stale objects.");
						UnityEngine.Debug.LogException(exception);
					}
				}
				goto end_IL_001a;
				IL_005c:
				if (netHost != null)
				{
					OnServerReceive(netHost, netStream);
				}
				end_IL_001a:;
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		public void OnServerReceive(NetHost client, NetStream stream)
		{
			if (!isServer)
			{
				return;
			}
			NetMsgId netMsgId = stream.ReadMsgId();
			if (netlog && netMsgId != NetMsgId.Container && netMsgId != NetMsgId.Delta && netMsgId != NetMsgId.Event && netMsgId != NetMsgId.Move)
			{
				UnityEngine.Debug.LogFormat("msg {0}", netMsgId);
			}
			switch (netMsgId)
			{
			case NetMsgId.AddHost:
			case NetMsgId.RemoveHost:
				break;
			case NetMsgId.Container:
			{
				uint num = ignoreDeltasMask;
				ignoreDeltasMask &= 4294967294u;
				uint num2 = stream.ReadUInt32(4);
				if (num2 != nextLevelInstanceID)
				{
					ignoreDeltasMask |= 1u;
				}
				client.readFrameId = (int)stream.ReadUInt32(22);
				NetStream netStream = stream.ReadStream();
				try
				{
					while (netStream != null)
					{
						OnServerReceive(client, netStream);
						netStream = netStream.Release();
						netStream = stream.ReadStream();
					}
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
				ignoreDeltasMask = (uint)(((int)ignoreDeltasMask & -2) | (int)(num & 1));
				break;
			}
			case NetMsgId.Helo:
				OnClientHelo(client, stream);
				break;
			case NetMsgId.AddPlayer:
				OnRequestAddPlayerServer(client, stream);
				break;
			case NetMsgId.RemovePlayer:
				OnRequestRemovePlayerServer(client, stream);
				break;
			case NetMsgId.Move:
			{
				uint id2 = stream.ReadNetId();
				NetPlayer netPlayer = client.FindPlayer(id2);
				if (netPlayer != null)
				{
					netPlayer.ReceiveMove(stream);
				}
				break;
			}
			case NetMsgId.Kick:
				if (App.state == AppSate.ServerPlayLevel)
				{
					Game.instance.RespawnAllPlayers(client);
				}
				break;
			case NetMsgId.Delta:
			{
				uint id3 = stream.ReadNetId();
				NetScope netScope2 = NetScope.Find(id3);
				if ((ignoreDeltasMask == 0 || netScope2 is NetPlayer) && netScope2 != null)
				{
					netScope2.OnReceiveAck(client, stream, client.GetReadFrameId());
				}
				break;
			}
			case NetMsgId.Event:
			{
				uint id = stream.ReadNetId();
				NetScope netScope = NetScope.Find(id);
				if ((ignoreDeltasMask == 0 || netScope is NetPlayer) && netScope != null)
				{
					netScope.OnReceiveEventAck(client, stream, client.GetReadFrameId());
				}
				break;
			}
			case NetMsgId.LoadLevel:
				OnReceiveLevelAck(client, stream);
				break;
			case NetMsgId.RequestSkin:
				OnRequestSkinServer(client, stream);
				break;
			case NetMsgId.SendSkin:
				OnReceiveSkin(stream);
				break;
			case NetMsgId.Chat:
				OnReceiveChatServer(client, stream);
				break;
			}
		}

		public void OnClientReceive(object connection, NetStream stream)
		{
			NetMsgId netMsgId = stream.ReadMsgId();
			if (netlog && netMsgId != NetMsgId.Container && netMsgId != NetMsgId.Delta && netMsgId != NetMsgId.Event && netMsgId != NetMsgId.Move && netMsgId != NetMsgId.Achievement)
			{
				UnityEngine.Debug.LogFormat("msg {0}", netMsgId);
			}
			if (!isClient)
			{
				if (netMsgId != NetMsgId.Helo && netMsgId != NetMsgId.Container && netMsgId != NetMsgId.Kick)
				{
					return;
				}
				lock (stateLock)
				{
					if (!isNetStarting && !isClient)
					{
						return;
					}
				}
			}
			switch (netMsgId)
			{
			case NetMsgId.Container:
			{
				uint num = ignoreDeltasMask;
				ignoreDeltasMask &= 4294967294u;
				uint num2 = stream.ReadUInt32(4);
				if (num2 != currentLevelInstanceID)
				{
					ignoreDeltasMask |= 1u;
				}
				server.readFrameId = (int)stream.ReadUInt32(22);
				NetStream netStream = stream.ReadStream();
				try
				{
					while (netStream != null)
					{
						OnClientReceive(connection, netStream);
						netStream = netStream.Release();
						netStream = stream.ReadStream();
					}
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
				ignoreDeltasMask = (uint)(((int)ignoreDeltasMask & -2) | (int)(num & 1));
				break;
			}
			case NetMsgId.Helo:
				OnHelo(connection, stream);
				break;
			case NetMsgId.Kick:
				App.instance.ServerKicked();
				break;
			case NetMsgId.AddHost:
			{
				uint num3 = stream.ReadNetId();
				if (server != null)
				{
					server.isReady = true;
				}
				while (num3 != 0)
				{
					string name = stream.ReadString();
					if (local != null && num3 == local.hostId)
					{
						if (!local.isReady)
						{
							local.isReady = true;
							readyclients.Add(local);
						}
					}
					else
					{
						NetHost netHost2 = new NetHost(null, name);
						netHost2.isLocal = false;
						netHost2.hostId = num3;
						netHost2.isReady = true;
						NetHost item = netHost2;
						allclients.Add(item);
						readyclients.Add(item);
					}
					num3 = stream.ReadNetId();
				}
				break;
			}
			case NetMsgId.RemoveHost:
			{
				uint hostId = stream.ReadNetId();
				NetHost netHost = FindAnyHost(hostId);
				if (netHost != null)
				{
					DestroyHostObjects(netHost);
				}
				break;
			}
			case NetMsgId.AddPlayer:
				OnAddPlayerClient(stream);
				break;
			case NetMsgId.RemovePlayer:
				OnRemovePlayerClient(stream);
				break;
			case NetMsgId.RequestSkin:
			{
				uint localCoopIndex = stream.ReadNetId();
				OnRequestSkinClient(localCoopIndex);
				break;
			}
			case NetMsgId.SendSkin:
				OnReceiveSkin(stream);
				break;
			case NetMsgId.Delta:
			{
				uint id3 = stream.ReadNetId();
				NetScope netScope2 = NetScope.Find(id3);
				if ((ignoreDeltasMask == 0 || netScope2 is NetPlayer) && netScope2 != null)
				{
					netScope2.OnReceiveDelta(stream, instance.server.GetReadFrameId());
				}
				break;
			}
			case NetMsgId.Event:
			{
				uint id2 = stream.ReadNetId();
				NetScope netScope = NetScope.Find(id2);
				if ((ignoreDeltasMask == 0 || netScope is NetPlayer) && netScope != null)
				{
					netScope.OnReceiveEvents(stream, instance.server.GetReadFrameId());
				}
				break;
			}
			case NetMsgId.Move:
			{
				uint id = stream.ReadNetId();
				NetPlayer netPlayer = local.FindPlayer(id);
				if (netPlayer != null)
				{
					netPlayer.ReceiveMoveAck(stream);
				}
				break;
			}
			case NetMsgId.LoadLevel:
				OnLoadLevel(stream);
				break;
			case NetMsgId.Chat:
				OnReceiveChatClient(stream);
				break;
			case NetMsgId.Achievement:
				OnUnlockAchievementClient(stream);
				break;
			}
		}

		private uint GetNextPlayerId()
		{
			return NetStream.GetDynamicScopeId();
		}

		public void AddLocalPlayer()
		{
			uint localCoopIndex = 0u;
			if (local.players.Count > 0 && local.players[0].localCoopIndex == 0)
			{
				localCoopIndex = 1u;
			}
			AddLocalPlayer(localCoopIndex);
		}

		public void AddLocalPlayer(uint localCoopIndex)
		{
			byte[] cRC = NetPlayer.GetLocalSkin(localCoopIndex).GetCRC();
			if (isClient)
			{
				NetStream netStream = BeginMessage(NetMsgId.AddPlayer);
				try
				{
					netStream.WriteNetId(localCoopIndex);
					netStream.Write(transport.getUserId((int)localCoopIndex));
					netStream.WriteArray(cRC, 8);
					SendReliableToServer(netStream);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
			}
			else
			{
				AddServerPlayer(local, isLocal: true, transport.getUserId((int)localCoopIndex), localCoopIndex, cRC);
			}
		}

		public void RemoveLocalPlayer(NetPlayer player)
		{
			if (isClient && !isLocal)
			{
				NetStream netStream = BeginMessage(NetMsgId.RemovePlayer);
				try
				{
					netStream.WriteNetId(player.netId);
					SendReliableToServer(netStream);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
			}
			else
			{
				RemoveServerPlayer(player);
			}
		}

		private void OnRequestAddPlayerServer(NetHost client, NetStream stream)
		{
			uint localCoopIndex = stream.ReadNetId();
			string userId = stream.ReadString();
			byte[] crc = stream.ReadArray(8);
			AddServerPlayer(client, isLocal: false, userId, localCoopIndex, crc);
		}

		private void OnRequestRemovePlayerServer(NetHost client, NetStream stream)
		{
			uint id = stream.ReadNetId();
			NetPlayer netPlayer = client.FindPlayer(id);
			if (netPlayer != null)
			{
				RemoveServerPlayer(netPlayer);
			}
		}

		private NetPlayer AddServerPlayer(NetHost owner, bool isLocal, string userId, uint localCoopIndex, byte[] crc)
		{
			if (instance.players.Count >= Options.lobbyMaxPlayers)
			{
				return null;
			}
			if (owner == null || !owner.isReady)
			{
				if (owner == null)
				{
					UnityEngine.Debug.LogError("Request to spawn a player for a NetHost that doesn't exist");
				}
				else
				{
					UnityEngine.Debug.LogError("Request to spawn a player for a NetHost that isn't ready : hostid=" + owner.hostId.ToString());
				}
				return null;
			}
			ReplayRecorder.Abort();
			NetPlayer netPlayer = NetPlayer.SpawnPlayer(GetNextPlayerId(), owner, isLocal, userId, localCoopIndex, crc);
			players.Add(netPlayer);
			netPlayer.StartNetwork();
			App.instance.OnClientCountChanged();
			if (Game.currentLevel != null)
			{
				Game.instance.Respawn(netPlayer.human, Vector3.zero);
			}
			else
			{
				netPlayer.human.SpawnAt(Vector3.zero);
				netPlayer.human.state = HumanState.Idle;
			}
			if (netPlayer.skin == null)
			{
				NetStream netStream = BeginMessage(NetMsgId.RequestSkin);
				try
				{
					netStream.WriteNetId(localCoopIndex);
					netStream.Write(userId);
					SendReliable(owner, netStream);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
			}
			if (isServer)
			{
				NetStream netStream2 = BeginMessage(NetMsgId.AddPlayer);
				try
				{
					netStream2.WriteNetId(netPlayer.netId);
					netStream2.WriteNetId(netPlayer.host.hostId);
					netStream2.WriteNetId(netPlayer.localCoopIndex);
					netStream2.Write(netPlayer.skinUserId);
					netStream2.WriteArray(netPlayer.skinCRC, 8);
					netStream2.WriteNetId(0u);
					for (int i = 0; i < readyclients.Count; i++)
					{
						SendReliable(readyclients[i], netStream2);
					}
					return netPlayer;
				}
				finally
				{
					if (netStream2 != null)
					{
						netStream2 = netStream2.Release();
					}
				}
			}
			return netPlayer;
		}

		private void RemoveServerPlayer(NetPlayer player)
		{
			ReplayRecorder.Abort();
			if (isServer)
			{
				NetStream netStream = BeginMessage(NetMsgId.RemovePlayer);
				try
				{
					netStream.WriteNetId(player.netId);
					for (int i = 0; i < readyclients.Count; i++)
					{
						SendReliable(readyclients[i], netStream);
					}
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
			}
			players.Remove(player);
			player.DespawnPlayer();
			App.instance.OnClientCountChanged();
		}

		private void DestroyHostObjects(NetHost client)
		{
			NetPlayer[] array = client.players.ToArray();
			foreach (NetPlayer netPlayer in array)
			{
				players.Remove(netPlayer);
				netPlayer.DespawnPlayer();
			}
			allclients.Remove(client);
			readyclients.Remove(client);
			if (client.notificationQueue != null)
			{
				client.notificationQueue.clients.Remove(client);
			}
			NetScope.RemoveAllRemoteState(client, setDying: true);
		}

		private void OnAddPlayerClient(NetStream stream)
		{
			for (uint num = stream.ReadNetId(); num != 0; num = stream.ReadNetId())
			{
				uint num2 = stream.ReadNetId();
				uint num3 = stream.ReadNetId();
				string text = stream.ReadString();
				byte[] skinCRC = stream.ReadArray(8);
				NetHost netHost = FindAnyHost(num2);
				if (netHost != null && netHost.isReady)
				{
					bool flag = num2 == local.hostId;
					NetPlayer netPlayer = NetPlayer.SpawnPlayer(num, netHost, flag, text, num3, skinCRC);
					players.Add(netPlayer);
					netPlayer.StartNetwork();
					App.instance.OnClientCountChanged();
					if (netPlayer.skin == null)
					{
						NetStream netStream = BeginMessage(NetMsgId.RequestSkin);
						try
						{
							netStream.WriteNetId(num3);
							netStream.Write(text);
							SendReliableToServer(netStream);
						}
						finally
						{
							if (netStream != null)
							{
								netStream = netStream.Release();
							}
						}
					}
				}
			}
		}

		private void OnRemovePlayerClient(NetStream stream)
		{
			uint id = stream.ReadNetId();
			NetPlayer netPlayer = NetScope.Find(id) as NetPlayer;
			if (netPlayer != null)
			{
				players.Remove(netPlayer);
				netPlayer.DespawnPlayer();
				App.instance.OnClientCountChanged();
			}
		}

		public void HostGame(object sessionArgs)
		{
			lock (stateLock)
			{
				if (isNetActive)
				{
					return;
				}
				isNetStarting = true;
			}
			multithreading = (serverThreads > 0);
			currentLevel = (ulong)Options.lobbyLevel;
			serverFrameId = 0;
			UpdateLevelInstanceID(currentLevel, start: false, 0u);
			transport.StartThread();
			transport.StartServer(HostGameCallback, sessionArgs);
		}

		private void HostGameCallback(string error)
		{
			DialogOverlay.DisableCancel();
			if (string.IsNullOrEmpty(error))
			{
				StopLocalGame();
				allclients.Clear();
				readyclients.Clear();
				local = (server = new NetHost(SteamUser.GetSteamID(), transport.GetMyName())
				{
					isLocal = true,
					isReady = true,
					hostId = 0u
				});
				lock (stateLock)
				{
					isServer = true;
					isNetStarted = true;
					isNetStarting = false;
				}
				TerminateNotifyQueues();
				notifyQueues = new NetNotificationQueue[Mathf.Max(1, instance.serverThreads)];
				for (int i = 0; i < notifyQueues.Length; i++)
				{
					notifyQueues[i] = new NetNotificationQueue();
					notifyQueues[i].StartNotifyThread();
				}
				AddLocalPlayer();
				App.instance.OnHostGameSuccess();
				HumanAnalytics.instance.BeginMultiplayer(host: true);
			}
			else
			{
				lock (stateLock)
				{
					isNetStarting = false;
				}
				App.instance.OnHostGameFail(string.Empty);
			}
		}

		private void TerminateNotifyQueues()
		{
			if (notifyQueues != null)
			{
				for (int i = 0; i < notifyQueues.Length; i++)
				{
					notifyQueues[i].StopNotifyThreadPhase1();
				}
				for (int j = 0; j < notifyQueues.Length; j++)
				{
				}
				notifyQueues = null;
			}
		}

		public void StopServer()
		{
			TerminateNotifyQueues();
			transport.StopServer();
			DestroyEverything();
		}

		public void NotifyClients(NetScope scope, int frameId, int timeId, NetStream full)
		{
			for (int i = 0; i < notifyQueues.Length; i++)
			{
				notifyQueues[i].NotifyClients(scope, frameId, timeId, full);
			}
		}

		public void OnServerConnect(NetHost client)
		{
			for (int i = 0; i < kickedUsers.Count; i++)
			{
				if (transport.ConnectionEquals(kickedUsers[i], client))
				{
					Kick(client);
					return;
				}
			}
			NetStream netStream = BeginMessage(NetMsgId.Helo);
			try
			{
				netStream.WriteNetId(VersionDisplay.netCode);
				netStream.Write(VersionDisplay.fullVersion);
				netStream.Write(instance.players.Count > Options.lobbyMaxPlayers);
				netStream.WriteNetId(client.hostId);
				netStream.Write(server.name);
				netStream.Write(lobbyLevel);
				netStream.Write(string.Empty + Game.multiplayerLobbyLevel);
				SendReliable(client, netStream);
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		public void OnClientHelo(NetHost client, NetStream msg)
		{
			if (client == null)
			{
				return;
			}
			uint num = msg.ReadNetId();
			string text = msg.ReadString();
			if (num == VersionDisplay.netCode)
			{
				client.name = msg.ReadString();
				if (transport.IsRelayed(client))
				{
					App.instance.OnRelayConnection(client);
				}
				NetStream netStream = BeginMessage(NetMsgId.LoadLevel);
				try
				{
					netStream.Write(v: false);
					netStream.Write((uint)currentLevel, 4, 32);
					netStream.Write(currentLevelStarted);
					netStream.Write(currentLevelHash, 32);
					netStream.Write((int)currentLevelType, 8);
					netStream.Write(nextLevelInstanceID, 4);
					SendReliable(client, netStream);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
				netStream = null;
				netStream = BeginMessage(NetMsgId.AddHost);
				try
				{
					for (int i = 0; i < readyclients.Count; i++)
					{
						if (readyclients[i] != client)
						{
							netStream.WriteNetId(readyclients[i].hostId);
							netStream.Write(readyclients[i].name);
						}
					}
					netStream.WriteNetId(0u);
					SendReliable(client, netStream);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
				netStream = null;
				netStream = BeginMessage(NetMsgId.AddPlayer);
				try
				{
					for (int j = 0; j < players.Count; j++)
					{
						netStream.WriteNetId(players[j].netId);
						netStream.WriteNetId(players[j].host.hostId);
						netStream.WriteNetId(players[j].localCoopIndex);
						netStream.Write(players[j].skinUserId);
						netStream.WriteArray(players[j].skinCRC, 8);
					}
					netStream.WriteNetId(0u);
					SendReliable(client, netStream);
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
				if (!client.isReady)
				{
					client.isReady = true;
					readyclients.Add(client);
				}
				netStream = null;
				netStream = BeginMessage(NetMsgId.AddHost);
				try
				{
					netStream.WriteNetId(client.hostId);
					netStream.Write(client.name);
					netStream.WriteNetId(0u);
					for (int k = 0; k < readyclients.Count; k++)
					{
						SendReliable(readyclients[k], netStream);
					}
				}
				finally
				{
					if (netStream != null)
					{
						netStream = netStream.Release();
					}
				}
			}
			App.instance.OnClientCountChanged();
		}

		public void Kick(NetHost client)
		{
			bool flag = false;
			for (int i = 0; i < kickedUsers.Count; i++)
			{
				if (transport.ConnectionEquals(kickedUsers[i], client))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				kickedUsers.Add(client.connection);
			}
			NetStream netStream = BeginMessage(NetMsgId.Kick);
			try
			{
				instance.SendReliable(client, netStream);
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		public void OnServerDisconnect(NetHost client)
		{
			NetStream netStream = BeginMessage(NetMsgId.RemoveHost);
			try
			{
				netStream.WriteNetId(client.hostId);
				for (int i = 0; i < readyclients.Count; i++)
				{
					if (readyclients[i] != client)
					{
						SendReliable(readyclients[i], netStream);
					}
				}
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
			App.instance.OnClientCountChanged();
		}

		private void OnRequestSkinClient(uint localCoopIndex)
		{
			NetStream netStream = BeginMessage(NetMsgId.SendSkin);
			try
			{
				RagdollPresetMetadata localSkin = NetPlayer.GetLocalSkin(localCoopIndex);
				netStream.WriteNetId(localCoopIndex);
				netStream.Write(transport.getUserId((int)localCoopIndex));
				netStream.WriteArray(localSkin.GetSerialized(), 32);
				SendReliableToServer(netStream);
				if (netlog)
				{
					UnityEngine.Debug.LogFormat("OnRequestSkinClient>SendSkin {0} {1} {2}", transport.getUserId((int)localCoopIndex), localCoopIndex, RagdollPresetMetadata.FormatCRC(localSkin.GetCRC()));
				}
			}
			finally
			{
				if (netStream != null)
				{
					netStream = netStream.Release();
				}
			}
		}

		private void OnRequestSkinServer(NetHost client, NetStream stream)
		{
			uint num = stream.ReadNetId();
			string text = stream.ReadString();
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].skinUserId.Equals(text))
				{
					NetPlayer netPlayer = players[i];
					if (netPlayer.skin != null)
					{
						NetStream netStream = BeginMessage(NetMsgId.SendSkin);
						try
						{
							RagdollPresetMetadata skin = players[i].skin;
							netStream.WriteNetId(num);
							netStream.Write(text);
							netStream.WriteArray(skin.GetSerialized(), 32);
							if (netlog)
							{
								UnityEngine.Debug.LogFormat("OnRequestSkinClient>SendSkin {0} {1} {2}", text, num, RagdollPresetMetadata.FormatCRC(skin.GetCRC()));
							}
							SendReliable(client, netStream);
						}
						finally
						{
							if (netStream != null)
							{
								netStream = netStream.Release();
							}
						}
					}
					else
					{
						netPlayer.skinQueue.Enqueue(client);
					}
				}
			}
		}

		private void OnReceiveSkin(NetStream stream)
		{
			uint num = stream.ReadNetId();
			string text = stream.ReadString();
			int num2 = 0;
			while (true)
			{
				if (num2 < players.Count)
				{
					if (players[num2].skinUserId.Equals(text) && players[num2].localCoopIndex == num)
					{
						break;
					}
					num2++;
					continue;
				}
				return;
			}
			NetPlayer netPlayer = players[num2];
			byte[] bytes = stream.ReadArray(32);
			netPlayer.ApplySkin(bytes);
			if (netlog)
			{
				UnityEngine.Debug.LogFormat("OnReceiveSkin {0} {1} {2}", text, num, RagdollPresetMetadata.FormatCRC(netPlayer.skin.GetCRC()));
			}
			if (isServer)
			{
				while (netPlayer.skinQueue.Count > 0)
				{
					NetHost host = netPlayer.skinQueue.Dequeue();
					SendReliable(host, stream);
				}
			}
		}
	}
}
