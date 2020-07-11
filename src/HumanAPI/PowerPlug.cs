using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	[RequireComponent(typeof(NetIdentity))]
	public class PowerPlug : CircuitConnector, IPostReset, INetBehavior
	{
		private enum PlugEventType
		{
			PlugPower,
			PlugNoPower,
			UnplugPower,
			UnplugNoPower,
			Short
		}

		[Tooltip("The rigid body being used as the plug")]
		public Rigidbody body;

		[Tooltip("Alignment location when attached")]
		public Transform alignmentTransform;

		[Tooltip("Alignment location for attached elements")]
		public Transform ropeFixPoint;

		[Tooltip("Joint which provides an inward orientation")]
		public SpringJoint springIn;

		[Tooltip("Joint which provides the outward orientation")]
		public SpringJoint springOut;

		[Tooltip("The torque needed to place and remove the plug")]
		public float torque = 0.2f;

		[Tooltip("Spring used in the connection and disconnection of the plug")]
		public float spring = 1000f;

		[Tooltip("Used when the plug is attached")]
		public float damper = 100f;

		[Tooltip("The socket the plug is connected to")]
		public PowerSocket connectedSocket;

		[Tooltip("The joint the plug is connected to")]
		public FixedJoint connectionJoint;

		private float breakDelay = 3f;

		private float reattachDelay = 2f;

		[Tooltip("The treshold the plug needs to pass in order to disconnect")]
		public float breakTreshold = 0.1f;

		[Tooltip("Allows for a weaker spring")]
		public float tensionDistance = 0.1f;

		[Tooltip("Displacement tolerance checked when connecting")]
		public float dispacementTolerance = 1f;

		private PowerSocket ignoreSocketConnect;

		private bool canBreak;

		private float breakableIn;

		private float reattachableIn;

		internal GameObject[] grablist;

		private PowerSocket startSocket;

		private SignalScriptNode1 sparks;

		[Tooltip("Instance of Circuit Sounds made instantiated on load if none exists")]
		public GameObject instanceofCircuitSounds;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		private uint evtPlug;

		private NetIdentity identity;

		private void Start()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Start ");
			}
			alignmentTransform = base.transform;
			startSocket = connectedSocket;
			if (startSocket != null)
			{
				Connect(connectedSocket);
			}
			if (!(CircuitSounds.instance == null))
			{
				return;
			}
			if (showDebug)
			{
				Debug.Log(base.name + " Trying to make instance ");
			}
			if (instanceofCircuitSounds != null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Circuit Sounds gameobject set ");
				}
				Object.Instantiate(instanceofCircuitSounds);
			}
		}

		private void FixedUpdate()
		{
			if (ReplayRecorder.isPlaying || NetGame.isClient)
			{
				return;
			}
			bool flag = false;
			if (grablist.Length != 0)
			{
				for (int i = 0; i < grablist.Length; i++)
				{
					if (GrabManager.IsGrabbedAny(grablist[i]))
					{
						flag = true;
					}
				}
			}
			if (flag && connectedSocket == null)
			{
				PowerSocket powerSocket = PowerSocket.Scan(alignmentTransform.position);
				if (powerSocket != null && powerSocket != ignoreSocketConnect && powerSocket.connected == null)
				{
					if (Connect(powerSocket))
					{
						if (parent.current != 0f)
						{
							SendPlug(PlugEventType.PlugPower);
						}
						else
						{
							SendPlug(PlugEventType.PlugNoPower);
						}
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
				return;
			}
			if (reattachableIn > 0f)
			{
				reattachableIn -= Time.fixedDeltaTime;
				if (reattachableIn < 0f)
				{
					ignoreSocketConnect = null;
				}
			}
			if (!(connectedSocket != null))
			{
				return;
			}
			springIn.spring = ((!flag) ? spring : (spring / 2f));
			springOut.spring = ((!flag) ? spring : (spring / 2f));
			float num = (alignmentTransform.position - alignmentTransform.forward * dispacementTolerance - connectedSocket.transform.position).magnitude - dispacementTolerance;
			if (springIn != null && springOut != null && num > breakTreshold)
			{
				if (parent.current != 0f)
				{
					SendPlug(PlugEventType.UnplugPower);
				}
				else
				{
					SendPlug(PlugEventType.UnplugNoPower);
				}
				Disconnect();
				reattachableIn = reattachDelay;
			}
		}

		private void Disconnect()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Disconnect ");
			}
			Circuit.Disconnect(this);
			SignalScriptNode1 component = GetComponent<SignalScriptNode1>();
			if (component != null)
			{
				component.SendSignal(2f);
			}
			Object.Destroy(springIn);
			Object.Destroy(springOut);
			ignoreSocketConnect = connectedSocket;
			connectedSocket = null;
		}

		private bool Connect(PowerSocket scan)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Connect ");
			}
			SignalScriptNode1 component = GetComponent<SignalScriptNode1>();
			if (component != null)
			{
				component.SendSignal(1f);
			}
			if (!Circuit.Connect(this, scan))
			{
				ignoreSocketConnect = scan;
				return false;
			}
			connectedSocket = scan;
			springIn = Attach(scan, alignmentTransform.position + torque * alignmentTransform.forward, connectedSocket.transform.position + (torque + tensionDistance) * connectedSocket.transform.forward);
			springOut = Attach(scan, alignmentTransform.position - torque * alignmentTransform.forward, connectedSocket.transform.position - (torque + tensionDistance / 2f) * connectedSocket.transform.forward);
			return true;
		}

		private SpringJoint Attach(PowerSocket socket, Vector3 anchor, Vector3 connectedAnchor)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Attach ");
			}
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
			if (showDebug)
			{
				Debug.Log(base.name + " Post Reset ");
			}
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
			if (showDebug)
			{
				Debug.Log(base.name + " Start Network ");
			}
			this.identity = identity;
			evtPlug = identity.RegisterEvent(OnPlug);
		}

		private void OnPlug(NetStream stream)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " On Plug ");
			}
			PlugEventType type = (PlugEventType)stream.ReadUInt32(3);
			PlayPlug(type);
		}

		private void SendPlug(PlugEventType type)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Send Plug ");
			}
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
				if (showDebug)
				{
					Debug.Log(base.name + " Plug Power ");
				}
				SparkPool.instance.Emit(10, base.transform.position);
				CircuitSounds.PlugWireCurrent(base.transform.position);
				break;
			case PlugEventType.PlugNoPower:
				if (showDebug)
				{
					Debug.Log(base.name + " Plug No Power ");
				}
				CircuitSounds.PlugWireNoCurrent(base.transform.position);
				break;
			case PlugEventType.UnplugPower:
				if (showDebug)
				{
					Debug.Log(base.name + " Unplug Power ");
				}
				SparkPool.instance.Emit(10, base.transform.position);
				CircuitSounds.UplugWireCurrent(base.transform.position);
				break;
			case PlugEventType.UnplugNoPower:
				if (showDebug)
				{
					Debug.Log(base.name + " Unplug No Power ");
				}
				CircuitSounds.UnplugWireNoCurrent(base.transform.position);
				break;
			case PlugEventType.Short:
				if (showDebug)
				{
					Debug.Log(base.name + " Short ");
				}
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
			if (showDebug)
			{
				Debug.Log(base.name + " Collect State ");
			}
			stream.WriteNetId((connectedSocket != null) ? connectedSocket.sceneId : 0u);
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply Lerped State ");
			}
			state0.ReadNetId();
			ApplyState(state1);
		}

		public void ApplyState(NetStream state)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply State ");
			}
			uint num = state.ReadNetId();
			PowerSocket powerSocket = (num == 0) ? null : PowerSocket.FindById(num);
			if (connectedSocket != powerSocket)
			{
				if (connectedSocket != null)
				{
					Disconnect();
				}
				if (powerSocket != null)
				{
					Connect(powerSocket);
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
			if (showDebug)
			{
				Debug.Log(base.name + " Add Delta ");
			}
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
			if (showDebug)
			{
				Debug.Log(base.name + " Max Size in Bits ");
			}
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
