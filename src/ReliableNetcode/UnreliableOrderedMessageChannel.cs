using ReliableNetcode.Utils;
using System;

namespace ReliableNetcode
{
	internal class UnreliableOrderedMessageChannel : MessageChannel
	{
		private ReliableConfig config;

		private ReliablePacketController packetController;

		private ushort nextSequence;

		public override int ChannelID => 2;

		public UnreliableOrderedMessageChannel()
		{
			config = ReliableConfig.DefaultConfig();
			config.TransmitPacketCallback = delegate(byte[] buffer, int size)
			{
				TransmitCallback(buffer, size);
			};
			config.ProcessPacketCallback = processPacket;
			packetController = new ReliablePacketController(config, DateTime.Now.GetTotalSeconds());
		}

		public override void Reset()
		{
			nextSequence = 0;
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

		protected void processPacket(ushort sequence, byte[] buffer, int length)
		{
			if (sequence == nextSequence || PacketIO.SequenceGreaterThan(sequence, nextSequence))
			{
				nextSequence = (ushort)(sequence + 1);
				ReceiveCallback(ChannelID, buffer, length);
			}
		}
	}
}
