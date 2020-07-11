using ReliableNetcode.Utils;
using System;

namespace ReliableNetcode
{
	internal class UnreliableMessageChannel : MessageChannel
	{
		private ReliableConfig config;

		private ReliablePacketController packetController;

		private SequenceBuffer<ReceivedPacketData> receiveBuffer;

		public override int ChannelID => 1;

		public UnreliableMessageChannel()
		{
			receiveBuffer = new SequenceBuffer<ReceivedPacketData>(256);
			config = ReliableConfig.DefaultConfig();
			config.TransmitPacketCallback = delegate(byte[] buffer, int size)
			{
				TransmitCallback(buffer, size);
			};
			config.ProcessPacketCallback = delegate(ushort seq, byte[] buffer, int size)
			{
				if (!receiveBuffer.Exists(seq))
				{
					receiveBuffer.Insert(seq);
					ReceiveCallback(ChannelID, buffer, size);
				}
			};
			packetController = new ReliablePacketController(config, DateTime.Now.GetTotalSeconds());
		}

		public override void Reset()
		{
			packetController.Reset();
		}

		public override void Update(double newTime)
		{
			packetController.Update(newTime);
		}

		public override void ReceivePacket(byte[] buffer, int bufferLength)
		{
			packetController.ReceivePacket(buffer, bufferLength);
		}

		public override void SendMessage(byte[] buffer, int bufferLength)
		{
			packetController.SendPacket(buffer, bufferLength, (byte)ChannelID);
		}
	}
}
