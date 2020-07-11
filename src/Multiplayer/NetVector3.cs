using UnityEngine;

namespace Multiplayer
{
	public struct NetVector3
	{
		public int x;

		public int y;

		public int z;

		public ushort bits;

		public NetVector3(int x, int y, int z, ushort bits)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.bits = bits;
		}

		public static NetVector3 Quantize(Vector3 vec, float range, ushort bits)
		{
			return new NetVector3(NetFloat.Quantize(vec.x, range, bits), NetFloat.Quantize(vec.y, range, bits), NetFloat.Quantize(vec.z, range, bits), bits);
		}

		public static NetVector3 Quantize(Vector3 vec, Vector3 range, ushort bits)
		{
			return new NetVector3(NetFloat.Quantize(vec.x, range.x, bits), NetFloat.Quantize(vec.y, range.y, bits), NetFloat.Quantize(vec.z, range.z, bits), bits);
		}

		public Vector3 Dequantize(float range)
		{
			return new Vector3(NetFloat.Dequantize(x, range, bits), NetFloat.Dequantize(y, range, bits), NetFloat.Dequantize(z, range, bits));
		}

		public Vector3 Dequantize(Vector3 range)
		{
			return new Vector3(NetFloat.Dequantize(x, range.x, bits), NetFloat.Dequantize(y, range.y, bits), NetFloat.Dequantize(z, range.z, bits));
		}

		public static NetVector3Delta Delta(NetVector3 from, NetVector3 to, ushort bitlarge)
		{
			int num = to.x - from.x;
			int num2 = to.y - from.y;
			int num3 = to.z - from.z;
			int num4 = 1 << bitlarge - 1;
			if (num < -num4 || num >= num4 || num2 < -num4 || num2 >= num4 || num3 < -num4 || num3 >= num4)
			{
				NetVector3Delta result = default(NetVector3Delta);
				result.isRelative = false;
				result.x = to.x;
				result.y = to.y;
				result.z = to.z;
				return result;
			}
			NetVector3Delta result2 = default(NetVector3Delta);
			result2.isRelative = true;
			result2.x = num;
			result2.y = num2;
			result2.z = num3;
			return result2;
		}

		public static NetVector3Delta WorstCaseDelta(NetVector3 from, NetVector3 to)
		{
			NetVector3Delta result = default(NetVector3Delta);
			result.isRelative = false;
			result.x = to.x;
			result.y = to.y;
			result.z = to.z;
			return result;
		}

		public static NetVector3 AddDelta(NetVector3 from, NetVector3Delta delta)
		{
			if (delta.isRelative)
			{
				return new NetVector3(from.x + delta.x, from.y + delta.y, from.z + delta.z, from.bits);
			}
			return new NetVector3(delta.x, delta.y, delta.z, from.bits);
		}

		public void Write(NetStream stream)
		{
			stream.Write(x, bits);
			stream.Write(y, bits);
			stream.Write(z, bits);
		}

		public static NetVector3 Read(NetStream stream, ushort bits)
		{
			return new NetVector3(stream.ReadInt32(bits), stream.ReadInt32(bits), stream.ReadInt32(bits), bits);
		}

		public void Write(NetStream stream, ushort bitsShort)
		{
			stream.Write(x, bitsShort, bits);
			stream.Write(y, bitsShort, bits);
			stream.Write(z, bitsShort, bits);
		}

		public static NetVector3 Read(NetStream stream, ushort bitsShort, ushort bits)
		{
			return new NetVector3(stream.ReadInt32(bitsShort, bits), stream.ReadInt32(bitsShort, bits), stream.ReadInt32(bitsShort, bits), bits);
		}

		public override string ToString()
		{
			Vector3 vector = Dequantize(500f);
			return $"({x},{y},{z}) ({vector.x},{vector.y}, {vector.z})";
		}

		public static bool operator ==(NetVector3 a, NetVector3 b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(NetVector3 a, NetVector3 b)
		{
			return !(a == b);
		}
	}
}
