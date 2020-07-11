using System.Collections.Generic;

namespace Multiplayer
{
	public class NetHost
	{
		public bool isLocal;

		public bool isReady;

		internal bool isDyingForScopes;

		public uint hostId;

		public ulong activeLevel;

		public string name = string.Empty;

		public NetHostConnectionType connectionType;

		public List<NetPlayer> players = new List<NetPlayer>();

		public object connection;

		public NetNotificationQueue notificationQueue;

		private object unreliableStreamLock = new object();

		public NetStream unreliableStream = NetStream.AllocStream();

		private uint currentBufferTag = 4294967294u;

		private static int cachedContainerLimitTier0;

		private int writeFrameId;

		public int readFrameId;

		internal bool mute;

		public NetHost(object connection, string name)
		{
			this.connection = connection;
			this.name = name;
		}

		public void AddPlayer(NetPlayer netPlayer)
		{
			players.Add(netPlayer);
			ChatManager.Instance.AddChatUser(netPlayer);
		}

		internal void RemovePlayer(NetPlayer netPlayer)
		{
			players.Remove(netPlayer);
			ChatManager.Instance.RemoveChatUser(netPlayer);
		}

		public bool CanBuffer(NetStream stream, int expectedContainerId)
		{
			if (unreliableStream.position == 0)
			{
				return true;
			}
			if (currentBufferTag != NetGame.currentLevelInstanceID)
			{
				return false;
			}
			if (expectedContainerId != -1 && expectedContainerId != writeFrameId)
			{
				return false;
			}
			return unreliableStream.UseBuffedSize() + stream.UseBuffedSize() + 2 < NetStream.CalculateSizeForTier(0);
		}

		public void Buffer(NetStream stream, NetTransport transport, int expectedContainerId)
		{
			lock (unreliableStreamLock)
			{
				if (!CanBuffer(stream, expectedContainerId))
				{
					FlushBuffer(transport);
				}
				if (unreliableStream.position == 0)
				{
					unreliableStream.WriteMsgId(NetMsgId.Container);
					unreliableStream.Write(NetGame.currentLevelInstanceID, 4);
					currentBufferTag = NetGame.currentLevelInstanceID;
					unreliableStream.Write((uint)writeFrameId, 22);
				}
				unreliableStream.WriteStream(stream);
			}
		}

		public static int CalcMaxPossibleSizeForContainerContents(int tier)
		{
			int num = NetStream.CalculateSizeForTier(tier);
			int num2 = num;
			int num3 = CalcMaxEstimate(num2);
			num2 -= num3 - num;
			for (num3 = CalcMaxEstimate(num2); num3 < num; num3 = CalcMaxEstimate(++num2))
			{
			}
			while (num3 > num)
			{
				num3 = CalcMaxEstimate(--num2);
			}
			if (num2 > 4194303)
			{
				num2 = 4194303;
			}
			return num2;
		}

		public static int CalcMaxPossibleSizeForContainerContentsTier0()
		{
			if (cachedContainerLimitTier0 == 0)
			{
				cachedContainerLimitTier0 = CalcMaxPossibleSizeForContainerContents(0);
			}
			return cachedContainerLimitTier0;
		}

		private static int CalcMaxEstimate(int streamSizeBytes)
		{
			uint num = 0u;
			num += 30;
			num = NetStream.PredictStreamAdvance((uint)streamSizeBytes, num);
			num = NetStream.PredictStreamAdvance(0u, num);
			return (int)(num + 7 >> 3);
		}

		public void FlushBuffer(NetTransport transport, bool skipLock = false)
		{
			lock (unreliableStreamLock)
			{
				if (unreliableStream.position != 0)
				{
					unreliableStream.WriteStream(null);
					transport.SendUnreliable(this, unreliableStream.GetOriginalBuffer(), unreliableStream.UseBuffedSize());
					NetGame.instance.sendBps.ReportBits(unreliableStream.position);
					unreliableStream.Seek(0);
				}
			}
		}

		public NetPlayer FindPlayer(uint id)
		{
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].netId == id)
				{
					return players[i];
				}
			}
			return null;
		}

		public int GetWriteFrameId(int serverFrameId)
		{
			lock (unreliableStreamLock)
			{
				if (unreliableStream.position == 0)
				{
					writeFrameId = serverFrameId;
				}
				return writeFrameId;
			}
		}

		public int GetReadFrameId()
		{
			return readFrameId;
		}
	}
}
