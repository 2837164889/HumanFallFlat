using ReliableNetcode.Utils;
using System;
using System.Collections.Generic;

namespace ReliableNetcode
{
	internal class ReliableMessageChannel : MessageChannel
	{
		internal class BufferedPacket
		{
			public bool writeLock = true;

			public double time;

			public ByteBuffer buffer = new ByteBuffer();
		}

		internal class OutgoingPacketSet
		{
			public List<ushort> MessageIds = new List<ushort>();
		}

		private ReliableConfig config;

		private ReliablePacketController packetController;

		private bool congestionControl;

		private double congestionDisableTimer;

		private double congestionDisableInterval;

		private double lastCongestionSwitchTime;

		private ByteBuffer messagePacker = new ByteBuffer();

		private SequenceBuffer<BufferedPacket> sendBuffer;

		private SequenceBuffer<BufferedPacket> receiveBuffer;

		private SequenceBuffer<OutgoingPacketSet> ackBuffer;

		private Queue<ByteBuffer> messageQueue = new Queue<ByteBuffer>();

		private double lastBufferFlush;

		private double lastMessageSend;

		private double time;

		private ushort oldestUnacked;

		private ushort sequence;

		private ushort nextReceive;

		protected List<ushort> tempList = new List<ushort>();

		public override int ChannelID => 0;

		public float RTT => packetController.RTT;

		public float PacketLoss => packetController.PacketLoss;

		public float SentBandwidthKBPS => packetController.SentBandwidthKBPS;

		public float ReceivedBandwidthKBPS => packetController.ReceivedBandwidthKBPS;

		public ReliableMessageChannel()
		{
			config = ReliableConfig.DefaultConfig();
			config.TransmitPacketCallback = delegate(byte[] buffer, int size)
			{
				TransmitCallback(buffer, size);
			};
			config.ProcessPacketCallback = processPacket;
			config.AckPacketCallback = ackPacket;
			sendBuffer = new SequenceBuffer<BufferedPacket>(256);
			receiveBuffer = new SequenceBuffer<BufferedPacket>(256);
			ackBuffer = new SequenceBuffer<OutgoingPacketSet>(256);
			time = DateTime.Now.GetTotalSeconds();
			lastBufferFlush = -1.0;
			lastMessageSend = 0.0;
			packetController = new ReliablePacketController(config, time);
			congestionDisableInterval = 5.0;
			sequence = 0;
			nextReceive = 0;
			oldestUnacked = 0;
		}

		public override void Reset()
		{
			packetController.Reset();
			sendBuffer.Reset();
			ackBuffer.Reset();
			lastBufferFlush = -1.0;
			lastMessageSend = 0.0;
			congestionControl = false;
			lastCongestionSwitchTime = 0.0;
			congestionDisableTimer = 0.0;
			congestionDisableInterval = 5.0;
			sequence = 0;
			nextReceive = 0;
			oldestUnacked = 0;
		}

		public override void Update(double newTime)
		{
			double num = newTime - time;
			time = newTime;
			packetController.Update(time);
			if (messageQueue.Count > 0)
			{
				int num2 = 0;
				ushort num3 = oldestUnacked;
				while (PacketIO.SequenceLessThan(num3, sequence))
				{
					if (sendBuffer.Exists(num3))
					{
						num2++;
					}
					num3 = (ushort)(num3 + 1);
				}
				if (num2 < sendBuffer.Size)
				{
					ByteBuffer byteBuffer = messageQueue.Dequeue();
					SendMessage(byteBuffer.InternalBuffer, byteBuffer.Length);
					ObjPool<ByteBuffer>.Return(byteBuffer);
				}
			}
			bool flag = packetController.RTT >= 250f;
			if (flag)
			{
				if (!congestionControl)
				{
					if (time - lastCongestionSwitchTime < 10.0)
					{
						congestionDisableInterval = Math.Min(congestionDisableInterval * 2.0, 60.0);
					}
					lastCongestionSwitchTime = time;
				}
				congestionControl = true;
				congestionDisableTimer = 0.0;
			}
			if (congestionControl && !flag)
			{
				congestionDisableTimer += num;
				if (congestionDisableTimer >= congestionDisableInterval)
				{
					congestionControl = false;
					lastCongestionSwitchTime = time;
					congestionDisableTimer = 0.0;
				}
			}
			if (!congestionControl)
			{
				congestionDisableTimer += num;
				if (congestionDisableTimer >= 10.0)
				{
					congestionDisableInterval = Math.Max(congestionDisableInterval * 0.5, 5.0);
				}
			}
			double num4 = (!congestionControl) ? 0.033 : 0.1;
			if (time - lastBufferFlush >= num4)
			{
				lastBufferFlush = time;
				processSendBuffer();
			}
		}

		public override void ReceivePacket(byte[] buffer, int bufferLength)
		{
			packetController.ReceivePacket(buffer, bufferLength);
		}

		public override void SendMessage(byte[] buffer, int bufferLength)
		{
			int num = 0;
			ushort num2 = oldestUnacked;
			while (PacketIO.SequenceLessThan(num2, sequence))
			{
				if (sendBuffer.Exists(num2))
				{
					num++;
				}
				num2 = (ushort)(num2 + 1);
			}
			if (num == sendBuffer.Size)
			{
				ByteBuffer byteBuffer = ObjPool<ByteBuffer>.Get();
				byteBuffer.SetSize(bufferLength);
				byteBuffer.BufferCopy(buffer, 0, 0, bufferLength);
				messageQueue.Enqueue(byteBuffer);
			}
			else
			{
				ushort val = sequence++;
				BufferedPacket bufferedPacket = sendBuffer.Insert(val);
				bufferedPacket.time = -1.0;
				int variableLengthBytes = getVariableLengthBytes((ushort)bufferLength);
				bufferedPacket.buffer.SetSize(bufferLength + 2 + variableLengthBytes);
				using (ByteArrayReaderWriter byteArrayReaderWriter = ByteArrayReaderWriter.Get(bufferedPacket.buffer.InternalBuffer))
				{
					byteArrayReaderWriter.Write(val);
					writeVariableLengthUShort((ushort)bufferLength, byteArrayReaderWriter);
					byteArrayReaderWriter.WriteBuffer(buffer, bufferLength);
				}
				bufferedPacket.writeLock = false;
			}
		}

		private void sendAckPacket()
		{
			packetController.SendAck((byte)ChannelID);
		}

		private int getVariableLengthBytes(ushort val)
		{
			if (val > 32767)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ((byte)(val >> 7) == 0) ? 1 : 2;
		}

		private void writeVariableLengthUShort(ushort val, ByteArrayReaderWriter writer)
		{
			if (val > 32767)
			{
				throw new ArgumentOutOfRangeException();
			}
			byte b = (byte)(val & 0x7F);
			byte b2 = (byte)(val >> 7);
			if (b2 != 0)
			{
				b = (byte)(b | 0x80);
			}
			writer.Write(b);
			if (b2 != 0)
			{
				writer.Write(b2);
			}
		}

		private ushort readVariableLengthUShort(ByteArrayReaderWriter reader)
		{
			ushort num = 0;
			byte b = reader.ReadByte();
			num = (ushort)(num | (ushort)(b & 0x7F));
			if ((b & 0x80) != 0)
			{
				num = (ushort)(num | (ushort)(reader.ReadByte() << 7));
			}
			return num;
		}

		protected void processSendBuffer()
		{
			int num = 0;
			ushort num2 = oldestUnacked;
			while (PacketIO.SequenceLessThan(num2, sequence))
			{
				num++;
				num2 = (ushort)(num2 + 1);
			}
			ushort num3 = oldestUnacked;
			while (PacketIO.SequenceLessThan(num3, sequence) && num3 < oldestUnacked + 256)
			{
				BufferedPacket bufferedPacket = sendBuffer.Find(num3);
				if (bufferedPacket != null && !bufferedPacket.writeLock && !(time - bufferedPacket.time < 0.1))
				{
					bool flag = false;
					if (!((bufferedPacket.buffer.Length >= config.FragmentThreshold) ? (messagePacker.Length + bufferedPacket.buffer.Length <= config.MaxPacketSize - 6 - 10) : (messagePacker.Length + bufferedPacket.buffer.Length <= config.FragmentThreshold - 10)))
					{
						flushMessagePacker();
					}
					bufferedPacket.time = time;
					int length = messagePacker.Length;
					messagePacker.SetSize(messagePacker.Length + bufferedPacket.buffer.Length);
					messagePacker.BufferCopy(bufferedPacket.buffer, 0, length, bufferedPacket.buffer.Length);
					tempList.Add(num3);
					lastMessageSend = time;
				}
				num3 = (ushort)(num3 + 1);
			}
			if (time - lastMessageSend >= 0.1)
			{
				sendAckPacket();
				lastMessageSend = time;
			}
			flushMessagePacker();
		}

		protected void flushMessagePacker(bool bufferAck = true)
		{
			if (messagePacker.Length > 0)
			{
				ushort num = packetController.SendPacket(messagePacker.InternalBuffer, messagePacker.Length, (byte)ChannelID);
				OutgoingPacketSet outgoingPacketSet = ackBuffer.Insert(num);
				outgoingPacketSet.MessageIds.Clear();
				outgoingPacketSet.MessageIds.AddRange(tempList);
				messagePacker.SetSize(0);
				tempList.Clear();
			}
		}

		protected void ackPacket(ushort seq)
		{
			OutgoingPacketSet outgoingPacketSet = ackBuffer.Find(seq);
			if (outgoingPacketSet == null)
			{
				return;
			}
			for (int i = 0; i < outgoingPacketSet.MessageIds.Count; i++)
			{
				ushort num = outgoingPacketSet.MessageIds[i];
				if (sendBuffer.Exists(num))
				{
					sendBuffer.Find(num).writeLock = true;
					sendBuffer.Remove(num);
				}
			}
			bool flag = true;
			ushort num2 = oldestUnacked;
			while (num2 == sequence || PacketIO.SequenceLessThan(num2, sequence))
			{
				if (sendBuffer.Exists(num2))
				{
					oldestUnacked = num2;
					flag = false;
					break;
				}
				num2 = (ushort)(num2 + 1);
			}
			if (flag)
			{
				oldestUnacked = sequence;
			}
		}

		protected void processPacket(ushort seq, byte[] packetData, int packetLen)
		{
			using (ByteArrayReaderWriter byteArrayReaderWriter = ByteArrayReaderWriter.Get(packetData))
			{
				while (byteArrayReaderWriter.ReadPosition < packetLen)
				{
					ushort num = byteArrayReaderWriter.ReadUInt16();
					ushort num2 = readVariableLengthUShort(byteArrayReaderWriter);
					if (num2 != 0)
					{
						if (!receiveBuffer.Exists(num))
						{
							BufferedPacket bufferedPacket = receiveBuffer.Insert(num);
							bufferedPacket.buffer.SetSize(num2);
							byteArrayReaderWriter.ReadBytesIntoBuffer(bufferedPacket.buffer.InternalBuffer, num2);
						}
						else
						{
							byteArrayReaderWriter.SeekRead(byteArrayReaderWriter.ReadPosition + (int)num2);
						}
						while (receiveBuffer.Exists(nextReceive))
						{
							BufferedPacket bufferedPacket2 = receiveBuffer.Find(nextReceive);
							ReceiveCallback(ChannelID, bufferedPacket2.buffer.InternalBuffer, bufferedPacket2.buffer.Length);
							receiveBuffer.Remove(nextReceive);
							nextReceive++;
						}
					}
				}
			}
		}
	}
}
