using UnityEngine;

namespace HumanAPI
{
	public class AngularServo : ServoBase
	{
		public Vector3 axis;

		public Rigidbody body;

		private Transform bodyTransform;

		private ConfigurableJoint joint;

		private Quaternion invInitialLocalRotation;

		protected override void Awake()
		{
			if (body == null)
			{
				body = GetComponent<Rigidbody>();
			}
			joint = body.GetComponent<ConfigurableJoint>();
			bodyTransform = body.transform;
			isKinematic = body.isKinematic;
			invInitialLocalRotation = joint.ReadInitialRotation();
			if (joint != null)
			{
				float num = maxValue - minValue;
				joint.SetXMotionAnchorsAndLimits(num / 2f - (initialValue - minValue), num);
				joint.lowAngularXLimit = new SoftJointLimit
				{
					limit = minValue - initialValue
				};
				joint.highAngularXLimit = new SoftJointLimit
				{
					limit = maxValue - initialValue
				};
				joint.angularXDrive = new JointDrive
				{
					maximumForce = maxForce
				};
			}
			base.Awake();
		}

		protected override float GetActualPosition()
		{
			return joint.GetXAngle(invInitialLocalRotation) - initialValue;
		}

		protected override void SetStatic(float pos)
		{
		}

		protected override void SetJoint(float pos, float spring, float damper)
		{
			joint.SetXAngleTarget(pos);
			JointDrive angularXDrive = joint.angularXDrive;
			angularXDrive.positionSpring = spring;
			angularXDrive.positionDamper = damper;
			joint.angularXDrive = angularXDrive;
			if (SignalManager.skipTransitions)
			{
				joint.ApplyXAngle(invInitialLocalRotation, pos);
			}
			else
			{
				body.WakeUp();
			}
		}
	}
}
