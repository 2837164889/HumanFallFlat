using UnityEngine;

namespace HumanAPI
{
	public class LinearServo : ServoBase
	{
		public Vector3 axis;

		public Rigidbody body;

		private Transform bodyTransform;

		private ConfigurableJoint joint;

		private Vector3 initialConnectedBodyPos;

		protected override void Awake()
		{
			if (body == null)
			{
				body = GetComponent<Rigidbody>();
			}
			joint = body.GetComponent<ConfigurableJoint>();
			bodyTransform = body.transform;
			isKinematic = body.isKinematic;
			initialConnectedBodyPos = GetConnectedAnchorPos();
			if (joint != null)
			{
				float num = maxValue - minValue;
				joint.SetXMotionAnchorsAndLimits(num / 2f - (initialValue - minValue), num);
				joint.xDrive = new JointDrive
				{
					maximumForce = maxForce
				};
			}
			base.Awake();
		}

		private Vector3 GetConnectedAnchorPos()
		{
			if (isKinematic)
			{
				return bodyTransform.InverseTransformPoint(bodyTransform.parent.position);
			}
			axis = joint.axis;
			if (joint.connectedBody == null)
			{
				return bodyTransform.InverseTransformPoint(Vector3.zero);
			}
			return bodyTransform.InverseTransformPoint(joint.connectedBody.position);
		}

		protected override float GetActualPosition()
		{
			return Vector3.Dot(GetConnectedAnchorPos() - initialConnectedBodyPos, -axis) - initialValue;
		}

		protected override void SetStatic(float pos)
		{
			Vector3 a = bodyTransform.InverseTransformPoint(bodyTransform.parent.position);
			Vector3 b = initialConnectedBodyPos - axis * pos;
			Vector3 b2 = bodyTransform.TransformDirection(a - b);
			Vector3 position = body.position + b2;
			body.MovePosition(position);
			bodyTransform.position = position;
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
	}
}
