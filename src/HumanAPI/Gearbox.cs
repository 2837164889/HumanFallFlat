using UnityEngine;

namespace HumanAPI
{
	public class Gearbox : Node, IPostReset
	{
		public float gearChangeTime = 3f;

		public float springSnap = 5000f;

		public float springRelax = 100f;

		public float reverseAngle = -30f;

		public float neutralAngle;

		public float firstAngle = 30f;

		public float secondAngle = 60f;

		public float reverseValue = -0.2f;

		public float firstValue = 0.2f;

		public float secondValue = 1f;

		public HingeJoint steeringWheel;

		public float centeringSpring1st = 100f;

		public float centeringSpring2st = 300f;

		public HingeJoint joint;

		public int gear;

		private float nextChangeIn;

		public NodeOutput output;

		private Quaternion invInitialLocalRotation;

		private int grabbedInGear;

		private bool grabbed;

		protected override void OnEnable()
		{
			base.OnEnable();
			invInitialLocalRotation = Quaternion.Inverse(joint.transform.localRotation);
		}

		private float GetJointAngle()
		{
			(invInitialLocalRotation * joint.transform.localRotation).ToAngleAxis(out float angle, out Vector3 axis);
			if (Vector3.Dot(axis, joint.axis) < 0f)
			{
				return 0f - angle;
			}
			return angle;
		}

		private void FixedUpdate()
		{
			if (joint == null)
			{
				return;
			}
			float jointAngle = GetJointAngle();
			bool flag = GrabManager.IsGrabbedAny(joint.gameObject);
			if (!grabbed && flag)
			{
				grabbedInGear = gear;
			}
			grabbed = flag;
			if (grabbed)
			{
				if (jointAngle <= (reverseAngle + neutralAngle) / 2f)
				{
					gear = -1;
				}
				else if (jointAngle <= (neutralAngle + firstAngle) / 2f)
				{
					gear = 0;
				}
				else if (jointAngle <= (firstAngle + secondAngle) / 2f)
				{
					gear = 1;
				}
				else
				{
					gear = 2;
				}
				JointLimits limits = joint.limits;
				limits.min = reverseAngle;
				if (grabbedInGear < 1)
				{
					if (gear == 2)
					{
						gear = 1;
					}
					limits.max = firstAngle;
				}
				else
				{
					limits.max = secondAngle;
				}
				joint.SetLimits(limits);
				if (steeringWheel != null)
				{
					JointSpring spring = steeringWheel.spring;
					spring.spring = ((gear != 2) ? centeringSpring1st : centeringSpring2st);
					steeringWheel.spring = spring;
				}
			}
			float value = 0f;
			JointSpring spring2 = joint.spring;
			switch (gear)
			{
			case -1:
				spring2.targetPosition = reverseAngle;
				value = reverseValue;
				break;
			case 0:
				spring2.targetPosition = neutralAngle;
				value = 0f;
				break;
			case 1:
				spring2.targetPosition = firstAngle;
				value = firstValue;
				break;
			case 2:
				spring2.targetPosition = secondAngle;
				value = secondValue;
				break;
			}
			joint.SetSpring(spring2);
			output.SetValue(value);
		}

		public void PostResetState(int checkpoint)
		{
			JointSpring spring = joint.spring;
			spring.targetPosition = neutralAngle;
			joint.spring = spring;
			gear = 0;
		}
	}
}
