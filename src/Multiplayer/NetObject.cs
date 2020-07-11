using UnityEngine;

namespace Multiplayer
{
	public class NetObject : MonoBehaviour
	{
		public Transform reftransform;

		public int id;

		private Vector3 startPos;

		private Quaternion startRot;

		private Vector3 netPos;

		private Quaternion netRot;

		public const float posRange = 500f;

		public const int possmall = 5;

		public const int poslarge = 9;

		public const int posfull = 18;

		public const int rotsmall = 5;

		public const int rotlarge = 8;

		public const int rotfull = 9;

		private void Awake()
		{
			startPos = base.transform.localPosition;
			startRot = base.transform.localRotation;
		}

		private void Update()
		{
			if (Game.GetKeyDown(KeyCode.Space))
			{
				base.transform.localPosition = startPos;
				base.transform.localRotation = startRot;
			}
		}

		public void WriteDelta(NetStream stream, NetStream reference, NetStream fullStream)
		{
			netPos = base.transform.localPosition;
			NetVector3 netVector = NetVector3.Quantize(netPos, 500f, 18);
			NetVector3 netVector2 = (reference != null) ? NetVector3.Read(reference, 18) : default(NetVector3);
			netRot = base.transform.localRotation;
			NetQuaternion netQuaternion = NetQuaternion.Quantize(netRot, 9);
			NetQuaternion netQuaternion2 = (reference != null) ? NetQuaternion.Read(reference, 9) : default(NetQuaternion);
			if (netVector != netVector2 || netQuaternion != netQuaternion2)
			{
				stream.Write(v: true);
				NetVector3.Delta(netVector2, netVector, 9).Write(stream, 5, 9, 18);
				NetQuaternion.Delta(netQuaternion2, netQuaternion, 8).Write(stream, 5, 8, 9);
			}
			else
			{
				stream.Write(v: false);
			}
			netVector.Write(fullStream);
			netQuaternion.Write(fullStream);
		}

		public void ReadDelta(NetStream stream, NetStream reference, NetStream fullStream)
		{
			bool flag = stream.ReadBool();
			NetVector3 netVector = (reference != null) ? NetVector3.Read(reference, 18) : default(NetVector3);
			NetQuaternion netQuaternion = (reference != null) ? NetQuaternion.Read(reference, 9) : default(NetQuaternion);
			NetVector3 netVector2;
			NetQuaternion netQuaternion2;
			if (flag)
			{
				NetVector3Delta delta = NetVector3Delta.Read(stream, 5, 9, 18);
				netVector2 = NetVector3.AddDelta(netVector, delta);
				NetQuaternionDelta delta2 = NetQuaternionDelta.Read(stream, 5, 8, 9);
				netQuaternion2 = NetQuaternion.AddDelta(netQuaternion, delta2);
			}
			else
			{
				netVector2 = netVector;
				netQuaternion2 = netQuaternion;
			}
			netVector2.Write(fullStream);
			netQuaternion2.Write(fullStream);
		}

		public void ApplySnapshot(NetStream stream)
		{
			NetVector3 netVector = NetVector3.Read(stream, 18);
			NetQuaternion netQuaternion = NetQuaternion.Read(stream, 9);
			netPos = netVector.Dequantize(500f);
			netRot = netQuaternion.Dequantize();
			base.transform.localPosition = netPos;
			base.transform.localRotation = netRot;
		}
	}
}
