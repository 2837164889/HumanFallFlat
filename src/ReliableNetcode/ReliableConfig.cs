using System;

namespace ReliableNetcode
{
	internal class ReliableConfig
	{
		public string Name;

		public int MaxPacketSize;

		public int FragmentThreshold;

		public int MaxFragments;

		public int FragmentSize;

		public int SentPacketBufferSize;

		public int ReceivedPacketBufferSize;

		public int FragmentReassemblyBufferSize;

		public float RTTSmoothFactor;

		public float PacketLossSmoothingFactor;

		public float BandwidthSmoothingFactor;

		public int PacketHeaderSize;

		public Action<byte[], int> TransmitPacketCallback;

		public Action<ushort, byte[], int> ProcessPacketCallback;

		public Action<ushort> AckPacketCallback;

		public static ReliableConfig DefaultConfig()
		{
			ReliableConfig reliableConfig = new ReliableConfig();
			reliableConfig.Name = "endpoint";
			reliableConfig.MaxPacketSize = 65536;
			reliableConfig.FragmentThreshold = 1024;
			reliableConfig.MaxFragments = 16;
			reliableConfig.FragmentSize = 1024;
			reliableConfig.SentPacketBufferSize = 256;
			reliableConfig.ReceivedPacketBufferSize = 256;
			reliableConfig.FragmentReassemblyBufferSize = 64;
			reliableConfig.RTTSmoothFactor = 0.25f;
			reliableConfig.PacketLossSmoothingFactor = 0.1f;
			reliableConfig.BandwidthSmoothingFactor = 0.1f;
			reliableConfig.PacketHeaderSize = 28;
			return reliableConfig;
		}
	}
}
