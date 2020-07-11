using System;
using UnityEngine;

namespace HumanAPI
{
	public class LinearServoJoint : ServoBase
	{
		public Rigidbody body;

		public Rigidbody connectedBody;

		private ConfigurableJoint joint;

		private Vector3 connectedAxis;

		private Vector3 connectedCenter;

		private Vector3 localForward;

		private Vector3 connectedOffset;

		protected override void Awake()
		{
			isKinematic = body.isKinematic;
			connectedAxis = base.transform.forward;
			connectedCenter = base.transform.position;
			connectedOffset = body.transform.position - connectedCenter;
			if (connectedBody != null)
			{
				connectedAxis = connectedBody.transform.InverseTransformDirection(connectedAxis);
				connectedCenter = connectedBody.transform.InverseTransformPoint(connectedCenter);
				connectedOffset = connectedBody.transform.InverseTransformVector(connectedOffset);
			}
			Vector3 anchor = body.transform.InverseTransformPoint(base.transform.position);
			localForward = body.transform.InverseTransformDirection(base.transform.forward);
			Vector3 secondaryAxis = body.transform.InverseTransformDirection(base.transform.right);
			if (!isKinematic)
			{
				float position = (maxValue + minValue) / 2f;
				SetPosition(position);
				joint = body.gameObject.AddComponent<ConfigurableJoint>();
				joint.axis = localForward;
				joint.secondaryAxis = secondaryAxis;
				joint.anchor = anchor;
				joint.connectedBody = connectedBody;
				joint.autoConfigureConnectedAnchor = false;
				joint.xMotion = ConfigurableJointMotion.Limited;
				joint.yMotion = ConfigurableJointMotion.Locked;
				joint.zMotion = ConfigurableJointMotion.Locked;
				joint.angularXMotion = ConfigurableJointMotion.Locked;
				joint.angularYMotion = ConfigurableJointMotion.Locked;
				joint.angularZMotion = ConfigurableJointMotion.Locked;
				joint.linearLimit = new SoftJointLimit
				{
					limit = (maxValue - minValue) / 2f
				};
				float num = (float)Math.PI / 180f;
				float positionSpring = maxForce / num;
				float positionDamper = maxForce / (maxSpeed * ((float)Math.PI / 180f));
				joint.xDrive = new JointDrive
				{
					maximumForce = maxForce,
					positionSpring = positionSpring,
					positionDamper = positionDamper
				};
			}
			base.Awake();
		}

		protected override float GetActualPosition()
		{
			if (connectedBody != null)
			{
				return Vector3.Dot(connectedBody.transform.InverseTransformPoint(body.transform.position) - connectedCenter, connectedAxis);
			}
			return Vector3.Dot(body.transform.position - connectedCenter, connectedAxis);
		}

		private void SetPosition(float pos)
		{
			Vector3 position = connectedCenter + connectedAxis * pos;
			if (connectedBody != null)
			{
				position = connectedBody.transform.TransformPoint(position);
			}
			body.MovePosition(position);
			if (!body.isKinematic)
			{
				body.transform.position = position;
			}
		}

		protected override void SetJoint(float pos, float spring, float damper)
		{
			joint.targetPosition = -new Vector3(pos - (maxValue - minValue) / 2f, 0f, 0f);
			JointDrive xDrive = joint.xDrive;
			xDrive.positionSpring = spring;
			xDrive.positionDamper = damper;
			joint.xDrive = xDrive;
			if (SignalManager.skipTransitions)
			{
				joint.ApplyXMotionTarget();
			}
			else
			{
				body.WakeUp();
			}
		}

		protected override void SetStatic(float pos)
		{
			SetPosition(pos);
		}
	}
}
