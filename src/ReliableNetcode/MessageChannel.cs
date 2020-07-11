using System;

namespace ReliableNetcode
{
	internal abstract class MessageChannel
	{
		public Action<byte[], int> TransmitCallback;

		public Action<int, byte[], int> ReceiveCallback;

		public abstract int ChannelID
		{
			get;
		}

		public abstract void Reset();

		public abstract void Update(double newTime);

		public abstract void ReceivePacket(byte[] buffer, int bufferLength);

		public abstract void SendMessage(byte[] buffer, int bufferLength);
	}
}
