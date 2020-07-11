using UnityEngine;

namespace Multiplayer
{
	public struct NetQuaternionDelta
	{
		public bool isRelative;

		public byte sel;

		public int x;

		public int y;

		public int z;

		public void Write(NetStream stream, ushort bitsmall, ushort bitlarge, ushort bitfull)
		{
			stream.Write(isRelative);
			stream.Write(sel, 2);
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

		public static NetQuaternionDelta Read(NetStream stream, ushort bitsmall, ushort bitlarge, ushort bitfull)
		{
			NetQuaternionDelta netQuaternionDelta = default(NetQuaternionDelta);
			netQuaternionDelta.isRelative = stream.ReadBool();
			netQuaternionDelta.sel = (byte)(stream.ReadInt32(2) & 3);
			NetQuaternionDelta result = netQuaternionDelta;
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
			return $"{isRelative} {sel} ({x},{y},{z})";
		}

		internal static int CalculateMaxDeltaSizeInBits(ushort bitsmall, ushort bitlarge, ushort bitfull)
		{
			return 3 + 3 * Mathf.Max(bitfull, bitlarge + 1);
		}
	}
}
