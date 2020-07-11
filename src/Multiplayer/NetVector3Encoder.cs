using UnityEngine;

namespace Multiplayer
{
	public class NetVector3Encoder
	{
		public Vector3 startPos = Vector3.zero;

		public float range;

		public ushort fullBits;

		public ushort deltaSmall;

		public ushort deltaLarge;

		public NetVector3Encoder(float range = 500f, ushort fullBits = 18, ushort deltaSmall = 4, ushort deltaLarge = 8)
		{
			this.range = range;
			this.fullBits = fullBits;
			this.deltaSmall = deltaSmall;
			this.deltaLarge = deltaLarge;
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			return NetVector3Delta.CalculateMaxDeltaSizeInBits(deltaSmall, deltaLarge, fullBits);
		}

		public NetVector3 Quantize(Vector3 value)
		{
			return NetVector3.Quantize(value, range, fullBits);
		}

		public Vector3 Dequantize(NetVector3 value)
		{
			return value.Dequantize(range);
		}

		public void CollectState(NetStream stream, Vector3 value)
		{
			NetVector3.Quantize(value, range, fullBits).Write(stream);
		}

		public Vector3 ApplyState(NetStream state)
		{
			return NetVector3.Read(state, fullBits).Dequantize(range);
		}

		public Vector3 ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			Vector3 a = NetVector3.Read(state0, fullBits).Dequantize(range);
			Vector3 vector = NetVector3.Read(state1, fullBits).Dequantize(range);
			if ((a - vector).sqrMagnitude > 15f)
			{
				a = vector;
			}
			return Vector3.Lerp(a, vector, mix);
		}

		public bool CalculateDelta(NetStream state0, NetStream state1, NetStream delta, bool writeChanged = true)
		{
			NetVector3 netVector = (state0 != null) ? NetVector3.Read(state0, fullBits) : NetVector3.Quantize(startPos, range, fullBits);
			NetVector3 netVector2 = NetVector3.Read(state1, fullBits);
			if (netVector == netVector2)
			{
				if (writeChanged)
				{
					delta.Write(v: false);
				}
				return false;
			}
			if (writeChanged)
			{
				delta.Write(v: true);
			}
			NetVector3.Delta(netVector, netVector2, deltaLarge).Write(delta, deltaSmall, deltaLarge, fullBits);
			return true;
		}

		public bool CalculateWorstCaseDelta(NetStream state0, NetStream state1, NetStream delta, bool writeChanged = true)
		{
			NetVector3 from = (state0 != null) ? NetVector3.Read(state0, fullBits) : NetVector3.Quantize(startPos, range, fullBits);
			NetVector3 to = (state1 != null) ? NetVector3.Read(state1, fullBits) : NetVector3.Quantize(startPos, range, fullBits);
			if (writeChanged)
			{
				delta.Write(v: true);
			}
			NetVector3.WorstCaseDelta(from, to).Write(delta, deltaSmall, deltaLarge, fullBits);
			return true;
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result, bool readChanged = true)
		{
			NetVector3 from = (state0 != null) ? NetVector3.Read(state0, fullBits) : NetVector3.Quantize(startPos, range, fullBits);
			if (delta == null || (readChanged && !delta.ReadBool()))
			{
				from.Write(result);
				return;
			}
			NetVector3Delta delta2 = NetVector3Delta.Read(delta, deltaSmall, deltaLarge, fullBits);
			NetVector3.AddDelta(from, delta2).Write(result);
		}
	}
}
