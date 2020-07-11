using ReliableNetcode.Utils;
using System;

namespace ReliableNetcode
{
	internal class ReliablePacketController
	{
		public ReliableConfig config;

		private double time;

		private float rtt;

		private float packetLoss;

		private float sentBandwidthKBPS;

		private float receivedBandwidthKBPS;

		private float ackedBandwidthKBPS;

		private ushort sequence;

		private SequenceBuffer<SentPacketData> sentPackets;

		private SequenceBuffer<ReceivedPacketData> receivedPackets;

		private SequenceBuffer<FragmentReassemblyData> fragmentReassembly;

		public float RTT => rtt;

		public float PacketLoss => packetLoss;

		public float SentBandwidthKBPS => sentBandwidthKBPS;

		public float ReceivedBandwidthKBPS => receivedBandwidthKBPS;

		public float AckedBandwidthKBPS => ackedBandwidthKBPS;

		public ReliablePacketController(ReliableConfig config, double time)
		{
			this.config = config;
			this.time = time;
			sentPackets = new SequenceBuffer<SentPacketData>(config.SentPacketBufferSize);
			receivedPackets = new SequenceBuffer<ReceivedPacketData>(config.ReceivedPacketBufferSize);
			fragmentReassembly = new SequenceBuffer<FragmentReassemblyData>(config.FragmentReassemblyBufferSize);
		}

		public ushort NextPacketSequence()
		{
			return sequence;
		}

		public void Reset()
		{
			sequence = 0;
			for (int i = 0; i < config.FragmentReassemblyBufferSize; i++)
			{
				fragmentReassembly.AtIndex(i)?.PacketDataBuffer.SetSize(0);
			}
			sentPackets.Reset();
			receivedPackets.Reset();
			fragmentReassembly.Reset();
		}

		public void Update(double newTime)
		{
			time = newTime;
			uint num = (uint)(sentPackets.sequence - config.SentPacketBufferSize + 1 + 65535);
			int num2 = 0;
			int num3 = config.SentPacketBufferSize / 2;
			for (int i = 0; i < num3; i++)
			{
				ushort num4 = (ushort)(num + i);
				SentPacketData sentPacketData = sentPackets.Find(num4);
				if (sentPacketData != null && !sentPacketData.acked)
				{
					num2++;
				}
			}
			float num5 = (float)num2 / (float)num3;
			if (Math.Abs(packetLoss - num5) > 1E-05f)
			{
				packetLoss += (num5 - packetLoss) * config.PacketLossSmoothingFactor;
			}
			else
			{
				packetLoss = num5;
			}
			uint num6 = (uint)(sentPackets.sequence - config.SentPacketBufferSize + 1 + 65535);
			int num7 = 0;
			double num8 = double.MaxValue;
			double num9 = 0.0;
			int num10 = config.SentPacketBufferSize / 2;
			for (int j = 0; j < num10; j++)
			{
				ushort num11 = (ushort)(num6 + j);
				SentPacketData sentPacketData2 = sentPackets.Find(num11);
				if (sentPacketData2 != null)
				{
					num7 += (int)sentPacketData2.packetBytes;
					num8 = Math.Min(num8, sentPacketData2.time);
					num9 = Math.Max(num9, sentPacketData2.time);
				}
			}
			if (num8 != double.MaxValue && num9 != 0.0)
			{
				float num12 = (float)num7 / (float)(num9 - num8) * 8f / 1000f;
				if (Math.Abs(sentBandwidthKBPS - num12) > 1E-05f)
				{
					sentBandwidthKBPS += (num12 - sentBandwidthKBPS) * config.BandwidthSmoothingFactor;
				}
				else
				{
					sentBandwidthKBPS = num12;
				}
			}
			lock (receivedPackets)
			{
				uint num13 = (uint)(receivedPackets.sequence - config.ReceivedPacketBufferSize + 1 + 65535);
				int num14 = 0;
				double num15 = double.MaxValue;
				double num16 = 0.0;
				int num17 = config.ReceivedPacketBufferSize / 2;
				for (int k = 0; k < num17; k++)
				{
					ushort num18 = (ushort)(num13 + k);
					ReceivedPacketData receivedPacketData = receivedPackets.Find(num18);
					if (receivedPacketData != null)
					{
						num14 += (int)receivedPacketData.packetBytes;
						num15 = Math.Min(num15, receivedPacketData.time);
						num16 = Math.Max(num16, receivedPacketData.time);
					}
				}
				if (num15 != double.MaxValue && num16 != 0.0)
				{
					float num19 = (float)num14 / (float)(num16 - num15) * 8f / 1000f;
					if (Math.Abs(receivedBandwidthKBPS - num19) > 1E-05f)
					{
						receivedBandwidthKBPS += (num19 - receivedBandwidthKBPS) * config.BandwidthSmoothingFactor;
					}
					else
					{
						receivedBandwidthKBPS = num19;
					}
				}
			}
			uint num20 = (uint)(sentPackets.sequence - config.SentPacketBufferSize + 1 + 65535);
			int num21 = 0;
			double num22 = double.MaxValue;
			double num23 = 0.0;
			int num24 = config.SentPacketBufferSize / 2;
			for (int l = 0; l < num24; l++)
			{
				ushort num25 = (ushort)(num20 + l);
				SentPacketData sentPacketData3 = sentPackets.Find(num25);
				if (sentPacketData3 != null && sentPacketData3.acked)
				{
					num21 += (int)sentPacketData3.packetBytes;
					num22 = Math.Min(num22, sentPacketData3.time);
					num23 = Math.Max(num23, sentPacketData3.time);
				}
			}
			if (num22 != double.MaxValue && num23 != 0.0)
			{
				float num26 = (float)num21 / (float)(num23 - num22) * 8f / 1000f;
				if (Math.Abs(ackedBandwidthKBPS - num26) > 1E-05f)
				{
					ackedBandwidthKBPS += (num26 - ackedBandwidthKBPS) * config.BandwidthSmoothingFactor;
				}
				else
				{
					ackedBandwidthKBPS = num26;
				}
			}
		}

		public void SendAck(byte channelID)
		{
			ushort ack;
			uint ackBits;
			lock (receivedPackets)
			{
				receivedPackets.GenerateAckBits(out ack, out ackBits);
			}
			byte[] buffer = BufferPool.GetBuffer(16);
			int arg = PacketIO.WriteAckPacket(buffer, channelID, ack, ackBits);
			config.TransmitPacketCallback(buffer, arg);
			BufferPool.ReturnBuffer(buffer);
		}

		public ushort SendPacket(byte[] packetData, int length, byte channelID)
		{
			if (length > config.MaxPacketSize)
			{
				throw new ArgumentOutOfRangeException("Packet is too large to send, max packet size is " + config.MaxPacketSize + " bytes");
			}
			ushort num = sequence++;
			ushort ack;
			uint ackBits;
			lock (receivedPackets)
			{
				receivedPackets.GenerateAckBits(out ack, out ackBits);
			}
			SentPacketData sentPacketData = sentPackets.Insert(num);
			sentPacketData.time = time;
			sentPacketData.packetBytes = (uint)(config.PacketHeaderSize + length);
			sentPacketData.acked = false;
			if (length <= config.FragmentThreshold)
			{
				byte[] buffer = BufferPool.GetBuffer(2048);
				int num2 = PacketIO.WritePacketHeader(buffer, channelID, num, ack, ackBits);
				int arg = length + num2;
				Buffer.BlockCopy(packetData, 0, buffer, num2, length);
				config.TransmitPacketCallback(buffer, arg);
				BufferPool.ReturnBuffer(buffer);
			}
			else
			{
				byte[] buffer2 = BufferPool.GetBuffer(10);
				int num3 = 0;
				try
				{
					num3 = PacketIO.WritePacketHeader(buffer2, channelID, num, ack, ackBits);
				}
				catch
				{
					throw;
				}
				int num4 = length / config.FragmentSize + ((length % config.FragmentSize != 0) ? 1 : 0);
				byte[] buffer3 = BufferPool.GetBuffer(2048);
				int num5 = 0;
				byte b = 1;
				b = (byte)(b | (byte)((channelID & 3) << 6));
				for (int i = 0; i < num4; i++)
				{
					using (ByteArrayReaderWriter byteArrayReaderWriter = ByteArrayReaderWriter.Get(buffer3))
					{
						byteArrayReaderWriter.Write(b);
						byteArrayReaderWriter.Write(channelID);
						byteArrayReaderWriter.Write(num);
						byteArrayReaderWriter.Write((byte)i);
						byteArrayReaderWriter.Write((byte)(num4 - 1));
						if (i == 0)
						{
							byteArrayReaderWriter.WriteBuffer(buffer2, num3);
						}
						int num6 = config.FragmentSize;
						if (num5 + num6 > length)
						{
							num6 = length - num5;
						}
						for (int j = 0; j < num6; j++)
						{
							byteArrayReaderWriter.Write(packetData[num5++]);
						}
						int arg2 = (int)byteArrayReaderWriter.WritePosition;
						config.TransmitPacketCallback(buffer3, arg2);
					}
				}
				BufferPool.ReturnBuffer(buffer2);
				BufferPool.ReturnBuffer(buffer3);
			}
			return num;
		}

		public void ReceivePacket(byte[] packetData, int bufferLength)
		{
			if (bufferLength > config.MaxPacketSize)
			{
				throw new ArgumentOutOfRangeException("Packet is larger than max packet size");
			}
			if (packetData == null)
			{
				throw new InvalidOperationException("Tried to receive null packet!");
			}
			if (bufferLength > packetData.Length)
			{
				throw new InvalidOperationException("Buffer length exceeds actual packet length!");
			}
			byte b = packetData[0];
			if ((b & 1) == 0)
			{
				byte channelID;
				ushort arg;
				ushort ack;
				uint ackBits;
				int num = PacketIO.ReadPacketHeader(packetData, 0, bufferLength, out channelID, out arg, out ack, out ackBits);
				bool flag;
				lock (receivedPackets)
				{
					flag = !receivedPackets.TestInsert(arg);
				}
				if (!flag && (b & 0x80) == 0)
				{
					if (num >= bufferLength)
					{
						throw new FormatException("Buffer too small for packet data!");
					}
					ByteBuffer byteBuffer = ObjPool<ByteBuffer>.Get();
					byteBuffer.SetSize(bufferLength - num);
					byteBuffer.BufferCopy(packetData, num, 0, byteBuffer.Length);
					config.ProcessPacketCallback(arg, byteBuffer.InternalBuffer, byteBuffer.Length);
					lock (receivedPackets)
					{
						ReceivedPacketData receivedPacketData = receivedPackets.Insert(arg);
						if (receivedPacketData == null)
						{
							throw new InvalidOperationException("Failed to insert received packet!");
						}
						receivedPacketData.time = time;
						receivedPacketData.packetBytes = (uint)(config.PacketHeaderSize + bufferLength);
					}
					ObjPool<ByteBuffer>.Return(byteBuffer);
				}
				if (flag && (b & 0x80) == 0)
				{
					return;
				}
				for (int i = 0; i < 32; i++)
				{
					if ((ackBits & 1) != 0)
					{
						ushort obj = (ushort)(ack - i);
						SentPacketData sentPacketData = sentPackets.Find(obj);
						if (sentPacketData != null && !sentPacketData.acked)
						{
							sentPacketData.acked = true;
							if (config.AckPacketCallback != null)
							{
								config.AckPacketCallback(obj);
							}
							float num2 = (float)(time - sentPacketData.time) * 1000f;
							if ((rtt == 0f && num2 > 0f) || Math.Abs(rtt - num2) < 1E-05f)
							{
								rtt = num2;
							}
							else
							{
								rtt += (num2 - rtt) * config.RTTSmoothFactor;
							}
						}
					}
					ackBits >>= 1;
				}
				return;
			}
			int fragmentID;
			int numFragments;
			int fragmentBytes;
			ushort num3;
			ushort ack2;
			uint ackBits2;
			byte channelID2;
			int num4 = PacketIO.ReadFragmentHeader(packetData, 0, bufferLength, config.MaxFragments, config.FragmentSize, out fragmentID, out numFragments, out fragmentBytes, out num3, out ack2, out ackBits2, out channelID2);
			FragmentReassemblyData fragmentReassemblyData = fragmentReassembly.Find(num3);
			if (fragmentReassemblyData == null)
			{
				fragmentReassemblyData = fragmentReassembly.Insert(num3);
				if (fragmentReassemblyData == null)
				{
					return;
				}
				fragmentReassemblyData.Sequence = num3;
				fragmentReassemblyData.Ack = 0;
				fragmentReassemblyData.AckBits = 0u;
				fragmentReassemblyData.NumFragmentsReceived = 0;
				fragmentReassemblyData.NumFragmentsTotal = numFragments;
				fragmentReassemblyData.PacketBytes = 0;
				Array.Clear(fragmentReassemblyData.FragmentReceived, 0, fragmentReassemblyData.FragmentReceived.Length);
			}
			if (numFragments == fragmentReassemblyData.NumFragmentsTotal && !fragmentReassemblyData.FragmentReceived[fragmentID])
			{
				fragmentReassemblyData.NumFragmentsReceived++;
				fragmentReassemblyData.FragmentReceived[fragmentID] = true;
				byte[] buffer = BufferPool.GetBuffer(2048);
				Buffer.BlockCopy(packetData, num4, buffer, 0, bufferLength - num4);
				fragmentReassemblyData.StoreFragmentData(channelID2, num3, ack2, ackBits2, fragmentID, config.FragmentSize, buffer, bufferLength - num4);
				BufferPool.ReturnBuffer(buffer);
				if (fragmentReassemblyData.NumFragmentsReceived == fragmentReassemblyData.NumFragmentsTotal)
				{
					ByteBuffer byteBuffer2 = ObjPool<ByteBuffer>.Get();
					byteBuffer2.SetSize(fragmentReassemblyData.PacketDataBuffer.Length - fragmentReassemblyData.HeaderOffset);
					Buffer.BlockCopy(fragmentReassemblyData.PacketDataBuffer.InternalBuffer, fragmentReassemblyData.HeaderOffset, byteBuffer2.InternalBuffer, 0, byteBuffer2.Length);
					ReceivePacket(byteBuffer2.InternalBuffer, byteBuffer2.Length);
					ObjPool<ByteBuffer>.Return(byteBuffer2);
					fragmentReassemblyData.PacketDataBuffer.SetSize(0);
					fragmentReassembly.Remove(num3);
				}
			}
		}
	}
}
