using ReliableNetcode.Utils;

namespace ReliableNetcode
{
	internal class FragmentReassemblyData
	{
		public ushort Sequence;

		public ushort Ack;

		public uint AckBits;

		public int NumFragmentsReceived;

		public int NumFragmentsTotal;

		public ByteBuffer PacketDataBuffer = new ByteBuffer();

		public int PacketBytes;

		public int HeaderOffset;

		public bool[] FragmentReceived = new bool[256];

		public void StoreFragmentData(byte channelID, ushort sequence, ushort ack, uint ackBits, int fragmentID, int fragmentSize, byte[] fragmentData, int fragmentBytes)
		{
			int src = 0;
			if (fragmentID == 0)
			{
				byte[] buffer = BufferPool.GetBuffer(10);
				int num = PacketIO.WritePacketHeader(buffer, channelID, sequence, ack, ackBits);
				HeaderOffset = 10 - num;
				if (PacketDataBuffer.Length < 10 + fragmentSize)
				{
					PacketDataBuffer.SetSize(10 + fragmentSize);
				}
				PacketDataBuffer.BufferCopy(buffer, 0, HeaderOffset, num);
				src = num;
				fragmentBytes -= num;
				BufferPool.ReturnBuffer(buffer);
			}
			int num2 = 10 + fragmentID * fragmentSize;
			int num3 = num2 + fragmentBytes;
			if (PacketDataBuffer.Length < num3)
			{
				PacketDataBuffer.SetSize(num3);
			}
			if (fragmentID == NumFragmentsTotal - 1)
			{
				PacketBytes = (NumFragmentsTotal - 1) * fragmentSize + fragmentBytes;
			}
			PacketDataBuffer.BufferCopy(fragmentData, src, 10 + fragmentID * fragmentSize, fragmentBytes);
		}
	}
}
