using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class PipePort : SteamPort, IPostEndReset, IPostReset
	{
		private const float kDisconnectIfBelow = -40f;

		private const float kPipeSpringTolerance = 0.025f;

		public bool isMale;

		public bool connectable = true;

		[Tooltip("After triggering this checkpoint pipe will no longer reset to it's starting position")]
		public int saveCheckpoint = -1;

		public Collider pipeCollider;

		public PipePort startPipe;

		public NodeOutput leak;

		private float breakDelay = 3f;

		private float breakTreshold = 0.2f;

		private float tensionDistance = 0.1f;

		private float bendArm = 2f;

		private float springPull = 50000f;

		private float damperPull = 1000f;

		private float springBend = 5000f;

		private float damperBend = 500f;

		public static List<PipePort> malePipes = new List<PipePort>();

		public static List<PipePort> femalePipes = new List<PipePort>();

		private SpringJoint springIn;

		private SpringJoint springCenter;

		private bool isMaster;

		private PipePort connectedPipe;

		private PipePort ignorePipe;

		private float breakableIn;

		internal Rigidbody parentBody;

		private int order;

		public float tensionPhase;

		public float oldTensionPhase;

		public override bool isOpen => connectedPipe == null;

		public override float ownPressure => 0f;

		public override SteamPort connectedPort => connectedPipe;

		public static List<PipePort> GetList(bool male)
		{
			return (!male) ? femalePipes : malePipes;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (!connectable)
			{
				return;
			}
			if (pipeCollider == null)
			{
				pipeCollider = GetComponentInParent<Collider>();
				if (pipeCollider == null)
				{
					Debug.LogError("No collider for pipePort");
				}
			}
			GetList(isMale).Add(this);
			parentBody = GetComponentInParent<Rigidbody>();
			order = GetInstanceID();
			List<PipePort> list = GetList(!isMale);
			for (int i = 0; i < list.Count; i++)
			{
				Physics.IgnoreCollision(pipeCollider, list[i].pipeCollider);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (connectable)
			{
				if ((bool)connectedPipe)
				{
					DisconnectPipe();
				}
				GetList(isMale).Remove(this);
			}
		}

		private PipePort Scan()
		{
			float a = 0.5f;
			float a2 = 0.5f;
			Vector3 position = base.transform.position;
			Vector3 lhs = -base.transform.forward;
			PipePort pipePort = null;
			float num = 0f;
			List<PipePort> list = GetList(!isMale);
			for (int i = 0; i < list.Count; i++)
			{
				PipePort pipePort2 = list[i];
				float num2 = Mathf.InverseLerp(a, 0f, (position - pipePort2.transform.position).magnitude);
				float num3 = Mathf.InverseLerp(a2, 1f, Vector3.Dot(lhs, pipePort2.transform.forward));
				float num4 = num2 * num3;
				if (num < num4)
				{
					num = num4;
					pipePort = pipePort2;
				}
			}
			return (!(num > 0.1f)) ? null : pipePort;
		}

		public void ScanAndConnect()
		{
			PipePort pipePort = Scan();
			if (pipePort != null && pipePort != ignorePipe)
			{
				ConnectPipe(pipePort);
			}
		}

		private void FixedUpdate()
		{
			if (parentBody == null || !connectable)
			{
				return;
			}
			bool flag = GrabManager.IsGrabbedAny(parentBody.gameObject);
			if (flag && connectedPipe == null)
			{
				PipePort pipePort = Scan();
				if (pipePort != null && pipePort != ignorePipe)
				{
					ConnectPipe(pipePort);
					return;
				}
			}
			if (!flag)
			{
				ignorePipe = null;
			}
			if (!(connectedPipe != null) || !isMaster)
			{
				return;
			}
			Vector3 position = base.transform.position;
			if (position.y < -40f && springIn != null)
			{
				DisconnectPipe();
				return;
			}
			if (breakableIn > 0f)
			{
				ApplyJointForce(loose: false, Apply: false);
				breakableIn -= Time.fixedDeltaTime;
				return;
			}
			bool loose = flag || (connectedPipe.parentBody != null && GrabManager.IsGrabbedAny(connectedPipe.parentBody.gameObject));
			ApplyJointForce(loose, Apply: false);
			float magnitude = (base.transform.position - connectedPipe.transform.position).magnitude;
			if (springIn != null && magnitude > breakTreshold)
			{
				DisconnectPipe();
			}
		}

		public void ApplyJointForce(bool loose, bool Apply)
		{
			tensionPhase = Mathf.MoveTowards(tensionPhase, (!loose) ? 1 : 0, Time.fixedDeltaTime);
			if (Apply || oldTensionPhase != tensionPhase)
			{
				springIn.spring = Mathf.Lerp(springBend / 1000f, springBend, tensionPhase);
				springCenter.spring = Mathf.Lerp(springPull / 1000f, springPull, tensionPhase);
				springIn.damper = damperBend;
				springCenter.damper = damperPull;
			}
			oldTensionPhase = tensionPhase;
		}

		public void DisconnectPipe()
		{
			PipePort pipePort = connectedPipe;
			Object.Destroy(springIn);
			Object.Destroy(springCenter);
			connectedPipe.ignorePipe = this;
			ignorePipe = connectedPipe;
			connectedPipe = null;
			if (pipePort.springIn != null)
			{
				Object.Destroy(pipePort.springIn);
				Object.Destroy(pipePort.springCenter);
			}
			pipePort.connectedPipe = null;
			SteamSystem.Recalculate(node);
			SteamSystem.Recalculate(pipePort.node);
		}

		public bool ConnectPipe(PipePort other)
		{
			connectedPipe = other;
			other.connectedPipe = this;
			isMaster = true;
			connectedPipe.isMaster = false;
			springIn = Attach(other, base.transform.position + bendArm * base.transform.forward, connectedPipe.transform.position - (bendArm + tensionDistance) * connectedPipe.transform.forward);
			springCenter = Attach(other, base.transform.position, connectedPipe.transform.position);
			tensionPhase = 0.5f;
			ApplyJointForce(loose: false, Apply: true);
			SteamSystem.Recalculate(node);
			SteamSystem.Recalculate(other.node);
			breakableIn = breakDelay;
			other.breakableIn = breakDelay;
			return true;
		}

		private SpringJoint Attach(PipePort pipe, Vector3 anchor, Vector3 connectedAnchor)
		{
			if (parentBody == null)
			{
				return null;
			}
			SpringJoint springJoint = parentBody.gameObject.AddComponent<SpringJoint>();
			springJoint.anchor = parentBody.transform.InverseTransformPoint(anchor);
			springJoint.autoConfigureConnectedAnchor = false;
			springJoint.damper = 0f;
			springJoint.tolerance = 0.025f;
			Rigidbody rigidbody = pipe.parentBody;
			if (rigidbody != null)
			{
				springJoint.connectedBody = rigidbody;
				springJoint.connectedAnchor = rigidbody.transform.InverseTransformPoint(connectedAnchor);
				springJoint.enableCollision = true;
			}
			else
			{
				springJoint.connectedAnchor = connectedAnchor;
			}
			return springJoint;
		}

		public void PostResetState(int checkpoint)
		{
			if ((saveCheckpoint <= 0 || checkpoint < saveCheckpoint) && connectedPipe != null && connectedPipe != startPipe && connectedPipe.startPipe != this)
			{
				DisconnectPipe();
			}
		}

		public void PostEndResetState(int checkpoint)
		{
			if ((saveCheckpoint <= 0 || checkpoint < saveCheckpoint) && startPipe != null && connectedPipe == null && parentBody != null && base.gameObject.activeInHierarchy && startPipe.gameObject.activeInHierarchy)
			{
				ConnectPipe(startPipe);
			}
		}

		public override void ApplySystemState(bool isOpenSystem, float pressure)
		{
			if (isOpen)
			{
				leak.SetValue(pressure);
			}
			else
			{
				leak.SetValue(0f);
			}
		}
	}
}
