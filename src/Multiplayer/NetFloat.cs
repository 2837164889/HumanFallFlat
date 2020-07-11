using UnityEngine;

namespace Multiplayer
{
	public class NetFloat
	{
		public static bool debug;

		public float range;

		public ushort fullBits;

		public ushort deltaSmall;

		public ushort deltaLarge;

		public NetFloat(float range, ushort fullBits, ushort deltaSmall, ushort deltaLarge)
		{
			this.range = range;
			this.fullBits = fullBits;
			this.deltaSmall = deltaSmall;
			this.deltaLarge = deltaLarge;
		}

		public void CollectState(NetStream stream, float value)
		{
			int x = Quantize(value, range, fullBits);
			stream.Write(x, fullBits);
		}

		public float ApplyState(NetStream state)
		{
			return Dequantize(state.ReadInt32(fullBits), range, fullBits);
		}

		public float ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			float a = Dequantize(state0.ReadInt32(fullBits), range, fullBits);
			float b = Dequantize(state1.ReadInt32(fullBits), range, fullBits);
			return Mathf.Lerp(a, b, mix);
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			int num = state0?.ReadInt32(fullBits) ?? Quantize(0f, range, fullBits);
			int num2 = state1.ReadInt32(fullBits);
			if (num == num2)
			{
				delta.Write(v: false);
				return;
			}
			delta.Write(v: true);
			if (deltaSmall == fullBits)
			{
				delta.Write(num2, fullBits);
				return;
			}
			int num3 = num2 - num;
			int num4 = 1 << deltaLarge - 1;
			if (num3 < -num4 || num3 >= num4)
			{
				delta.Write(v: false);
				delta.Write(num2, fullBits);
				return;
			}
			delta.Write(v: true);
			if (deltaSmall == deltaLarge)
			{
				delta.Write(num3, deltaLarge);
			}
			else
			{
				delta.Write(num3, deltaSmall, deltaLarge);
			}
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			int num = state0?.ReadInt32(fullBits) ?? Quantize(0f, range, fullBits);
			if (!delta.ReadBool())
			{
				result.Write(num, fullBits);
				return;
			}
			int x;
			if (deltaSmall == fullBits)
			{
				x = delta.ReadInt32(fullBits);
			}
			else if (!delta.ReadBool())
			{
				x = delta.ReadInt32(fullBits);
			}
			else
			{
				int num2 = (deltaSmall != deltaLarge) ? delta.ReadInt32(deltaSmall, deltaLarge) : delta.ReadInt32(deltaLarge);
				x = num + num2;
			}
			result.Write(x, fullBits);
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			int num = 1;
			if (deltaSmall == fullBits)
			{
				return num + fullBits;
			}
			num++;
			if (deltaSmall == deltaLarge)
			{
				return num + Mathf.Max(fullBits, deltaLarge);
			}
			return num + Mathf.Max(fullBits, 1 + deltaLarge);
		}

		public static int Quantize(float value, float range, ushort bits)
		{
			int num = 1 << bits - 1;
			float num2 = value / range * (float)num;
			if (num2 < (float)(-num))
			{
				return -num;
			}
			if (num2 >= (float)num)
			{
				return num - 1;
			}
			return (int)num2;
		}

		public static float Dequantize(int value, float range, ushort bits)
		{
			int num = 1 << bits - 1;
			return range * (float)value / (float)num;
		}
	}
}
