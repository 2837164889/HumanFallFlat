using ReliableNetcode.Utils;
using System;

namespace ReliableNetcode
{
	public class ReliableEndpoint
	{
		public Action<byte[], int> TransmitCallback;

		public Action<int, byte[], int> ReceiveCallback;

		public Action<uint, byte[], int> TransmitExtendedCallback;

		public Action<uint, byte[], int> ReceiveExtendedCallback;

		public uint Index = uint.MaxValue;

		private MessageChannel[] messageChannels;

		private double time;

		private ReliableMessageChannel _reliableChannel;

		public float RTT => _reliableChannel.RTT;

		public float PacketLoss => _reliableChannel.PacketLoss;

		public float SentBandwidthKBPS => _reliableChannel.SentBandwidthKBPS;

		public float ReceivedBandwidthKBPS => _reliableChannel.ReceivedBandwidthKBPS;

		public ReliableEndpoint()
		{
			time = DateTime.Now.GetTotalSeconds();
			_reliableChannel = new ReliableMessageChannel
			{
				TransmitCallback = transmitMessage,
				ReceiveCallback = receiveMessage
			};
			messageChannels = new MessageChannel[3]
			{
				_reliableChannel,
				new UnreliableMessageChannel
				{
					TransmitCallback = transmitMessage,
					ReceiveCallback = receiveMessage
				},
				new UnreliableOrderedMessageChannel
				{
					TransmitCallback = transmitMessage,
					ReceiveCallback = receiveMessage
				}
			};
		}

		public ReliableEndpoint(uint index)
			: this()
		{
			Index = index;
		}

		public void Reset()
		{
			for (int i = 0; i < messageChannels.Length; i++)
			{
				messageChannels[i].Reset();
			}
		}

		public void Update()
		{
			Update(DateTime.Now.GetTotalSeconds());
		}

		public void UpdateFastForward(double increment)
		{
			time += increment;
			Update(time);
		}

		public void Update(double time)
		{
			this.time = time;
			for (int i = 0; i < messageChannels.Length; i++)
			{
				messageChannels[i].Update(this.time);
			}
		}

		public void ReceivePacket(byte[] buffer, int bufferLength)
		{
			int num = buffer[1];
			messageChannels[num].ReceivePacket(buffer, bufferLength);
		}

		public void SendMessage(byte[] buffer, int bufferLength, QosType qos)
		{
			messageChannels[(uint)qos].SendMessage(buffer, bufferLength);
		}

		protected void receiveMessage(int channel, byte[] buffer, int length)
		{
			if (ReceiveCallback != null)
			{
				ReceiveCallback(channel, buffer, length);
			}
			if (ReceiveExtendedCallback != null)
			{
				ReceiveExtendedCallback(Index, buffer, length);
			}
		}

		protected void transmitMessage(byte[] buffer, int length)
		{
			if (TransmitCallback != null)
			{
				TransmitCallback(buffer, length);
			}
			if (TransmitExtendedCallback != null)
			{
				TransmitExtendedCallback(Index, buffer, length);
			}
		}
	}
}
