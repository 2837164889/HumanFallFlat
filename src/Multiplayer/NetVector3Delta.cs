using UnityEngine;

namespace Multiplayer
{
	public struct NetVector3Delta
	{
		public bool isRelative;

		public int x;

		public int y;

		public int z;

		public void Write(NetStream stream, ushort bitsmall, ushort bitlarge, ushort bitfull)
		{
			stream.Write(isRelative);
			if (isRelative)
			{
				stream.Write(x, bitsmall, bitlarge);
				stream.Write(y, bitsmall, bitlarge);
				stream.Write(z, bitsmall, bitlarge);
			}
			else
			{
				stream.Write(x, bitfull);
				stream.Write(y, bitfull);
				stream.Write(z, bitfull);
			}
		}

		public static NetVector3Delta Read(NetStream stream, ushort bitsmall, ushort bitlarge, ushort bitfull)
		{
			NetVector3Delta netVector3Delta = default(NetVector3Delta);
			netVector3Delta.isRelative = stream.ReadBool();
			NetVector3Delta result = netVector3Delta;
			if (result.isRelative)
			{
				result.x = stream.ReadInt32(bitsmall, bitlarge);
				result.y = stream.ReadInt32(bitsmall, bitlarge);
				result.z = stream.ReadInt32(bitsmall, bitlarge);
			}
			else
			{
				result.x = stream.ReadInt32(bitfull);
				result.y = stream.ReadInt32(bitfull);
				result.z = stream.ReadInt32(bitfull);
			}
			return result;
		}

		public override string ToString()
		{
			return $"{isRelative} ({x},{y},{z}) ({NetFloat.Dequantize(x, 500f, 18)},{NetFloat.Dequantize(y, 500f, 18)},{NetFloat.Dequantize(z, 500f, 18)})";
		}

		public static int CalculateMaxDeltaSizeInBits(ushort bitsmall, ushort bitlarge, ushort bitfull)
		{
			return 1 + 3 * Mathf.Max(bitfull, bitlarge + 1);
		}
	}
}
