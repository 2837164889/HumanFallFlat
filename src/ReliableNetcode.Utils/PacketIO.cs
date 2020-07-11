using System;

namespace ReliableNetcode.Utils
{
	internal static class PacketIO
	{
		public static bool SequenceGreaterThan(ushort s1, ushort s2)
		{
			return (s1 > s2 && s1 - s2 <= 32768) || (s1 < s2 && s2 - s1 > 32768);
		}

		public static bool SequenceLessThan(ushort s1, ushort s2)
		{
			return SequenceGreaterThan(s2, s1);
		}

		public static int ReadPacketHeader(byte[] packetBuffer, int offset, int bufferLength, out byte channelID, out ushort sequence, out ushort ack, out uint ackBits)
		{
			if (bufferLength < 4)
			{
				throw new FormatException("Buffer too small for packet header");
			}
			using (ByteArrayReaderWriter byteArrayReaderWriter = ByteArrayReaderWriter.Get(packetBuffer))
			{
				byteArrayReaderWriter.SeekRead(offset);
				byte b = byteArrayReaderWriter.ReadByte();
				if ((b & 1) != 0)
				{
					throw new InvalidOperationException("Header does not indicate regular packet");
				}
				channelID = byteArrayReaderWriter.ReadByte();
				if ((b & 0x80) == 0)
				{
					sequence = byteArrayReaderWriter.ReadUInt16();
				}
				else
				{
					sequence = 0;
				}
				if ((b & 0x20) != 0)
				{
					if (bufferLength < 3)
					{
						throw new FormatException("Buffer too small for packet header");
					}
					byte b2 = byteArrayReaderWriter.ReadByte();
					ack = (ushort)(sequence - b2);
				}
				else
				{
					if (bufferLength < 4)
					{
						throw new FormatException("Buffer too small for packet header");
					}
					ack = byteArrayReaderWriter.ReadUInt16();
				}
				int num = 0;
				for (int i = 0; i <= 4; i++)
				{
					if ((b & (1 << i)) != 0)
					{
						num++;
					}
				}
				if (bufferLength < bufferLength - byteArrayReaderWriter.ReadPosition + num)
				{
					throw new FormatException("Buffer too small for packet header");
				}
				ackBits = uint.MaxValue;
				if ((b & 2) != 0)
				{
					ackBits &= 4294967040u;
					ackBits |= byteArrayReaderWriter.ReadByte();
				}
				if ((b & 4) != 0)
				{
					ackBits &= 4294902015u;
					ackBits |= (uint)(byteArrayReaderWriter.ReadByte() << 8);
				}
				if ((b & 8) != 0)
				{
					ackBits &= 4278255615u;
					ackBits |= (uint)(byteArrayReaderWriter.ReadByte() << 16);
				}
				if ((b & 0x10) != 0)
				{
					ackBits &= 16777215u;
					ackBits |= (uint)(byteArrayReaderWriter.ReadByte() << 24);
				}
				return (int)byteArrayReaderWriter.ReadPosition - offset;
			}
		}

		public static int ReadFragmentHeader(byte[] packetBuffer, int offset, int bufferLength, int maxFragments, int fragmentSize, out int fragmentID, out int numFragments, out int fragmentBytes, out ushort sequence, out ushort ack, out uint ackBits, out byte channelID)
		{
			if (bufferLength < 6)
			{
				throw new FormatException("Buffer too small for packet header");
			}
			using (ByteArrayReaderWriter byteArrayReaderWriter = ByteArrayReaderWriter.Get(packetBuffer))
			{
				byte b = byteArrayReaderWriter.ReadByte();
				if ((b & 1) != 1)
				{
					throw new FormatException("Packet header indicates non-fragment packet");
				}
				channelID = byteArrayReaderWriter.ReadByte();
				sequence = byteArrayReaderWriter.ReadUInt16();
				fragmentID = byteArrayReaderWriter.ReadByte();
				numFragments = byteArrayReaderWriter.ReadByte() + 1;
				if (numFragments > maxFragments)
				{
					throw new FormatException("Packet header indicates fragments outside of max range");
				}
				if (fragmentID >= numFragments)
				{
					throw new FormatException("Packet header indicates fragment ID outside of fragment count");
				}
				fragmentBytes = bufferLength - 6;
				ushort sequence2 = 0;
				ushort ack2 = 0;
				uint ackBits2 = 0u;
				byte channelID2 = 0;
				if (fragmentID == 0)
				{
					int num = ReadPacketHeader(packetBuffer, 6, bufferLength, out channelID2, out sequence2, out ack2, out ackBits2);
					if (sequence2 != sequence)
					{
						throw new FormatException("Bad packet sequence in fragment");
					}
					fragmentBytes = bufferLength - num - 6;
				}
				ack = ack2;
				ackBits = ackBits2;
				if (fragmentBytes > fragmentSize)
				{
					throw new FormatException("Fragment bytes remaining > indicated fragment size");
				}
				if (fragmentID != numFragments - 1 && fragmentBytes != fragmentSize)
				{
					throw new FormatException("Fragment size invalid");
				}
				return (int)byteArrayReaderWriter.ReadPosition - offset;
			}
		}

		public static int WriteAckPacket(byte[] packetBuffer, byte channelID, ushort ack, uint ackBits)
		{
			using (ByteArrayReaderWriter byteArrayReaderWriter = ByteArrayReaderWriter.Get(packetBuffer))
			{
				byte b = 128;
				if ((ackBits & 0xFF) != 255)
				{
					b = (byte)(b | 2);
				}
				if ((ackBits & 0xFF00) != 65280)
				{
					b = (byte)(b | 4);
				}
				if ((ackBits & 0xFF0000) != 16711680)
				{
					b = (byte)(b | 8);
				}
				if (((int)ackBits & -16777216) != -16777216)
				{
					b = (byte)(b | 0x10);
				}
				byteArrayReaderWriter.Write(b);
				byteArrayReaderWriter.Write(channelID);
				byteArrayReaderWriter.Write(ack);
				if ((ackBits & 0xFF) != 255)
				{
					byteArrayReaderWriter.Write((byte)(ackBits & 0xFF));
				}
				if ((ackBits & 0xFF00) != 65280)
				{
					byteArrayReaderWriter.Write((byte)((ackBits & 0xFF00) >> 8));
				}
				if ((ackBits & 0xFF0000) != 16711680)
				{
					byteArrayReaderWriter.Write((byte)((ackBits & 0xFF0000) >> 16));
				}
				if (((int)ackBits & -16777216) != -16777216)
				{
					byteArrayReaderWriter.Write((byte)((uint)((int)ackBits & -16777216) >> 24));
				}
				return (int)byteArrayReaderWriter.WritePosition;
			}
		}

		public static int WritePacketHeader(byte[] packetBuffer, byte channelID, ushort sequence, ushort ack, uint ackBits)
		{
			using (ByteArrayReaderWriter byteArrayReaderWriter = ByteArrayReaderWriter.Get(packetBuffer))
			{
				byte b = 0;
				if ((ackBits & 0xFF) != 255)
				{
					b = (byte)(b | 2);
				}
				if ((ackBits & 0xFF00) != 65280)
				{
					b = (byte)(b | 4);
				}
				if ((ackBits & 0xFF0000) != 16711680)
				{
					b = (byte)(b | 8);
				}
				if (((int)ackBits & -16777216) != -16777216)
				{
					b = (byte)(b | 0x10);
				}
				int num = sequence - ack;
				if (num < 0)
				{
					num += 65536;
				}
				if (num <= 255)
				{
					b = (byte)(b | 0x20);
				}
				byteArrayReaderWriter.Write(b);
				byteArrayReaderWriter.Write(channelID);
				byteArrayReaderWriter.Write(sequence);
				if (num <= 255)
				{
					byteArrayReaderWriter.Write((byte)num);
				}
				else
				{
					byteArrayReaderWriter.Write(ack);
				}
				if ((ackBits & 0xFF) != 255)
				{
					byteArrayReaderWriter.Write((byte)(ackBits & 0xFF));
				}
				if ((ackBits & 0xFF00) != 65280)
				{
					byteArrayReaderWriter.Write((byte)((ackBits & 0xFF00) >> 8));
				}
				if ((ackBits & 0xFF0000) != 16711680)
				{
					byteArrayReaderWriter.Write((byte)((ackBits & 0xFF0000) >> 16));
				}
				if (((int)ackBits & -16777216) != -16777216)
				{
					byteArrayReaderWriter.Write((byte)((uint)((int)ackBits & -16777216) >> 24));
				}
				return (int)byteArrayReaderWriter.WritePosition;
			}
		}
	}
}
