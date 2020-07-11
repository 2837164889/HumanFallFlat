using UnityEngine;

namespace Multiplayer
{
	public class NetQuaternionEncoder
	{
		public Quaternion startRot = Quaternion.identity;

		public ushort fullBits;

		public ushort deltaSmall;

		public ushort deltaLarge;

		public NetQuaternionEncoder(ushort fullBits = 9, ushort deltaSmall = 4, ushort deltaLarge = 6)
		{
			this.fullBits = fullBits;
			this.deltaSmall = deltaSmall;
			this.deltaLarge = deltaLarge;
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			return NetQuaternionDelta.CalculateMaxDeltaSizeInBits(deltaSmall, deltaLarge, fullBits);
		}

		public void CollectState(NetStream stream, Quaternion value)
		{
			NetQuaternion.Quantize(value, fullBits).Write(stream);
		}

		public Quaternion ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			Quaternion a = NetQuaternion.Read(state0, fullBits).Dequantize();
			Quaternion b = NetQuaternion.Read(state1, fullBits).Dequantize();
			return Quaternion.Slerp(a, b, mix);
		}

		public Quaternion ApplyState(NetStream state)
		{
			return NetQuaternion.Read(state, fullBits).Dequantize();
		}

		public bool CalculateDelta(NetStream state0, NetStream state1, NetStream delta, bool writeChanged = true)
		{
			NetQuaternion netQuaternion = (state0 != null) ? NetQuaternion.Read(state0, fullBits) : NetQuaternion.Quantize(startRot, fullBits);
			NetQuaternion netQuaternion2 = NetQuaternion.Read(state1, fullBits);
			if (netQuaternion == netQuaternion2)
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
			NetQuaternion.Delta(netQuaternion, netQuaternion2, deltaLarge).Write(delta, deltaSmall, deltaLarge, fullBits);
			return true;
		}

		public bool CalculateWorstCaseDelta(NetStream state0, NetStream state1, NetStream delta, bool writeChanged = true)
		{
			NetQuaternion from = (state0 != null) ? NetQuaternion.Read(state0, fullBits) : NetQuaternion.Quantize(startRot, fullBits);
			NetQuaternion to = (state1 != null) ? NetQuaternion.Read(state1, fullBits) : NetQuaternion.Quantize(startRot, fullBits);
			if (writeChanged)
			{
				delta.Write(v: true);
			}
			NetQuaternion.WorstCaseDelta(from, to).Write(delta, deltaSmall, deltaLarge, fullBits);
			return true;
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result, bool readChanged = true)
		{
			NetQuaternion from = (state0 != null) ? NetQuaternion.Read(state0, fullBits) : NetQuaternion.Quantize(startRot, fullBits);
			if (delta == null || (readChanged && !delta.ReadBool()))
			{
				from.Write(result);
				return;
			}
			NetQuaternionDelta delta2 = NetQuaternionDelta.Read(delta, deltaSmall, deltaLarge, fullBits);
			NetQuaternion.AddDelta(from, delta2).Write(result);
		}
	}
}
