using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	[RequireComponent(typeof(NetIdentity))]
	public class HoseConnector : MonoBehaviour, IPostReset, INetBehavior
	{
		private enum PlugEventType
		{
			PlugPower,
			PlugNoPower,
			UnplugPower,
			UnplugNoPower,
			Short
		}

		public Rigidbody body;

		public Transform alignmentTransform;

		public Transform ropeFixPoint;

		public SpringJoint springIn;

		public SpringJoint springOut;

		public float torque = 0.2f;

		public float spring = 1000f;

		public float damper = 100f;

		public HoseSocket connectedSocket;

		public FixedJoint connectionJoint;

		private float breakDelay = 3f;

		public float breakTreshold = 0.1f;

		public float tensionDistance = 0.1f;

		public float dispacementTolerance = 1f;

		private HoseSocket ignoreSocketConnect;

		private bool canBreak;

		private float breakableIn;

		internal GameObject[] grablist;

		private HoseSocket startSocket;

		private uint evtPlug;

		private NetIdentity identity;

		private void Start()
		{
			alignmentTransform = base.transform;
			startSocket = connectedSocket;
			if (startSocket != null)
			{
				Connect(connectedSocket);
			}
		}

		private void FixedUpdate()
		{
			if (ReplayRecorder.isPlaying || NetGame.isClient)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < grablist.Length; i++)
			{
				if (GrabManager.IsGrabbedAny(grablist[i]))
				{
					flag = true;
				}
			}
			if (flag && connectedSocket == null)
			{
				HoseSocket hoseSocket = HoseSocket.Scan(alignmentTransform.position);
				if (hoseSocket != null && hoseSocket != ignoreSocketConnect)
				{
					if (Connect(hoseSocket))
					{
						canBreak = false;
						breakableIn = breakDelay;
						return;
					}
					Collider[] array = Physics.OverlapSphere(base.transform.position, 5f);
					for (int j = 0; j < array.Length; j++)
					{
						Rigidbody componentInParent = array[j].GetComponentInParent<Rigidbody>();
						if (componentInParent != null && !componentInParent.isKinematic)
						{
							componentInParent.AddExplosionForce(20000f, base.transform.position, 5f);
							Human componentInParent2 = componentInParent.GetComponentInParent<Human>();
							if (componentInParent2 != null)
							{
								componentInParent2.MakeUnconscious();
							}
						}
					}
					SendPlug(PlugEventType.Short);
				}
			}
			if (!flag)
			{
				ignoreSocketConnect = null;
				canBreak = true;
			}
			if (breakableIn > 0f)
			{
				breakableIn -= Time.fixedDeltaTime;
			}
			else if (connectedSocket != null)
			{
				springIn.spring = ((!flag) ? spring : (spring / 2f));
				springOut.spring = ((!flag) ? spring : (spring / 2f));
				float num = (alignmentTransform.position - alignmentTransform.forward * dispacementTolerance - connectedSocket.transform.position).magnitude - dispacementTolerance;
				if (springIn != null && springOut != null && num > breakTreshold)
				{
					Disconnect();
				}
			}
		}

		private void Disconnect()
		{
			Object.Destroy(springIn);
			Object.Destroy(springOut);
			ignoreSocketConnect = connectedSocket;
			connectedSocket = null;
		}

		private bool Connect(HoseSocket scan)
		{
			connectedSocket = scan;
			springIn = Attach(scan, alignmentTransform.position + torque * alignmentTransform.forward, connectedSocket.transform.position + (torque + tensionDistance) * connectedSocket.transform.forward);
			springOut = Attach(scan, alignmentTransform.position - torque * alignmentTransform.forward, connectedSocket.transform.position - (torque + tensionDistance / 2f) * connectedSocket.transform.forward);
			return true;
		}

		private SpringJoint Attach(HoseSocket socket, Vector3 anchor, Vector3 connectedAnchor)
		{
			SpringJoint springJoint = base.gameObject.AddComponent<SpringJoint>();
			springJoint.anchor = base.transform.InverseTransformPoint(anchor);
			springJoint.autoConfigureConnectedAnchor = false;
			Rigidbody componentInParent = socket.GetComponentInParent<Rigidbody>();
			if (componentInParent != null)
			{
				springJoint.connectedBody = componentInParent;
				springJoint.connectedAnchor = componentInParent.transform.InverseTransformPoint(connectedAnchor);
				springJoint.enableCollision = true;
			}
			else
			{
				springJoint.connectedAnchor = connectedAnchor;
			}
			springJoint.spring = spring;
			springJoint.damper = damper;
			return springJoint;
		}

		public void PostResetState(int checkpoint)
		{
			if (connectedSocket != null)
			{
				Disconnect();
			}
			if (startSocket != null)
			{
				Connect(startSocket);
			}
		}

		public void StartNetwork(NetIdentity identity)
		{
			this.identity = identity;
			evtPlug = identity.RegisterEvent(OnPlug);
		}

		private void OnPlug(NetStream stream)
		{
			PlugEventType type = (PlugEventType)stream.ReadUInt32(3);
			PlayPlug(type);
		}

		private void SendPlug(PlugEventType type)
		{
			PlayPlug(type);
			if (NetGame.isServer || ReplayRecorder.isRecording)
			{
				NetStream netStream = identity.BeginEvent(evtPlug);
				netStream.Write((uint)type, 3);
				identity.EndEvent();
			}
		}

		private void PlayPlug(PlugEventType type)
		{
			switch (type)
			{
			case PlugEventType.PlugPower:
				SparkPool.instance.Emit(10, base.transform.position);
				CircuitSounds.PlugWireCurrent(base.transform.position);
				break;
			case PlugEventType.PlugNoPower:
				CircuitSounds.PlugWireNoCurrent(base.transform.position);
				break;
			case PlugEventType.UnplugPower:
				SparkPool.instance.Emit(10, base.transform.position);
				CircuitSounds.UplugWireCurrent(base.transform.position);
				break;
			case PlugEventType.UnplugNoPower:
				CircuitSounds.UnplugWireNoCurrent(base.transform.position);
				break;
			case PlugEventType.Short:
				SparkPool.instance.Emit(30, base.transform.position, 10f);
				CircuitSounds.PlugWireShortCircuit(base.transform.position);
				if (EasterShortCircuit.instance != null)
				{
					EasterShortCircuit.instance.ShorCircuit();
				}
				break;
			}
		}

		public void CollectState(NetStream stream)
		{
			stream.WriteNetId((connectedSocket != null) ? connectedSocket.sceneId : 0u);
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			state0.ReadNetId();
			ApplyState(state1);
		}

		public void ApplyState(NetStream state)
		{
			uint num = state.ReadNetId();
			HoseSocket hoseSocket = (num == 0) ? null : HoseSocket.FindById(num);
			if (connectedSocket != hoseSocket)
			{
				if (connectedSocket != null)
				{
					Disconnect();
				}
				if (hoseSocket != null)
				{
					Connect(hoseSocket);
				}
			}
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			uint num = state0?.ReadNetId() ?? 0;
			uint num2 = state1.ReadNetId();
			if (num == num2)
			{
				delta.Write(v: false);
				return;
			}
			delta.Write(v: true);
			delta.WriteNetId(num2);
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			uint id = state0?.ReadNetId() ?? 0;
			if (delta.ReadBool())
			{
				result.WriteNetId(delta.ReadNetId());
			}
			else
			{
				result.WriteNetId(id);
			}
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			return 11;
		}

		public void SetMaster(bool isMaster)
		{
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
		}
	}
}
