using Multiplayer;
using System;
using UnityEngine;

namespace HumanAPI
{
	public abstract class JointImplementation : JointBase, INetBehavior
	{
		public Transform body;

		public Transform anchor;

		public bool useTransformAnchor;

		public bool enableCollision;

		[NonSerialized]
		public float centerValue;

		protected Rigidbody rigid;

		protected Rigidbody anchorRigid;

		protected Transform anchorTransform;

		protected Quaternion relativeRotation;

		protected Vector3 relativePosition;

		protected ConfigurableJoint joint;

		internal bool syncNetBody = true;

		private bool initialized;

		public int precision;

		private NetFloat encoder;

		public bool jointCreated => joint != null;

		private void Awake()
		{
			EnsureInitialized();
		}

		public override void EnsureInitialized()
		{
			if (initialized)
			{
				return;
			}
			initialized = true;
			useTension = (spring == 0f && damper == 0f);
			tensionDist = Mathf.Max(tensionDist, maxSpeed * Time.fixedDeltaTime);
			rigid = body.GetComponent<Rigidbody>();
			_isKinematic = (rigid == null || rigid.isKinematic);
			relativeRotation = body.rotation;
			relativePosition = body.position;
			if (anchor != null)
			{
				if (useTransformAnchor)
				{
					anchorTransform = anchor;
				}
				else
				{
					anchorRigid = anchor.GetComponentInParent<Rigidbody>();
					anchorTransform = ((!(anchorRigid != null)) ? anchor : anchorRigid.transform);
				}
				relativePosition = anchorTransform.InverseTransformPoint(relativePosition);
				relativeRotation = Quaternion.Inverse(anchorTransform.rotation) * relativeRotation;
			}
			CreateMainJoint();
		}

		public abstract void CreateMainJoint();

		public void DestroyMainJoint()
		{
			if (joint != null)
			{
				UnityEngine.Object.DestroyImmediate(joint);
				joint = null;
			}
		}

		public override float GetTarget()
		{
			return target;
		}

		public override void SetTarget(float target)
		{
			base.target = target;
		}

		public virtual void ResetState(int checkpoint, int subObjectives)
		{
			if (joint == null)
			{
				Vector3 position = relativePosition;
				Quaternion quaternion = relativeRotation;
				if (anchorTransform != null)
				{
					quaternion = anchorTransform.rotation * quaternion;
					position = anchorTransform.TransformPoint(relativePosition);
				}
				body.position = position;
				body.rotation = quaternion;
				CreateMainJoint();
			}
			else
			{
				SetValue(0f);
			}
		}

		public void StartNetwork(NetIdentity identity)
		{
			encoder = new NetFloat(1f, (ushort)(12 + precision), (ushort)(3 + precision), (ushort)(8 + precision));
		}

		public void SetMaster(bool isMaster)
		{
		}

		public virtual void CollectState(NetStream stream)
		{
			if (syncNetBody && useLimits)
			{
				encoder.CollectState(stream, Mathf.InverseLerp(minValue, maxValue, GetValue()));
			}
		}

		public virtual void ApplyState(NetStream state)
		{
			if (syncNetBody && useLimits)
			{
				float value = Mathf.Lerp(minValue, maxValue, encoder.ApplyState(state));
				SetValue(value);
			}
		}

		public virtual void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			if (syncNetBody && useLimits)
			{
				float value = Mathf.Lerp(minValue, maxValue, encoder.ApplyLerpedState(state0, state1, mix));
				SetValue(value);
			}
		}

		public virtual void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			if (syncNetBody && useLimits)
			{
				encoder.CalculateDelta(state0, state1, delta);
			}
		}

		public virtual void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			if (syncNetBody && useLimits)
			{
				encoder.AddDelta(state0, delta, result);
			}
		}

		public virtual int CalculateMaxDeltaSizeInBits()
		{
			if (!syncNetBody || !useLimits)
			{
				return 0;
			}
			return encoder.CalculateMaxDeltaSizeInBits();
		}
	}
}
