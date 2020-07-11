using UnityEngine;

namespace HumanAPI
{
	public class SignalAngle : Node
	{
		public NodeOutput output;

		public HingeJoint joint;

		public float fromAngle;

		public float toAngle;

		public bool signedOutput = true;

		private Rigidbody body;

		private Quaternion invInitialLocalRotation;

		public float currentAngle;

		public HingeJoint limitJoint;

		public float currentValue;

		private void Awake()
		{
			if (joint == null)
			{
				joint = GetComponent<HingeJoint>();
			}
			body = joint.GetComponent<Rigidbody>();
			invInitialLocalRotation = joint.ReadInitialRotation();
		}

		private void FixedUpdate()
		{
			float xAngle = joint.GetXAngle(invInitialLocalRotation);
			float num;
			for (num = xAngle - currentAngle; num < -180f; num += 360f)
			{
			}
			while (num > 180f)
			{
				num -= 360f;
			}
			currentAngle += num;
			if (num == 0f)
			{
				return;
			}
			if (currentAngle < fromAngle + 60f)
			{
				if (limitJoint == null)
				{
					limitJoint = CreateLimitJoint(fromAngle - currentAngle, Mathf.Min(fromAngle - currentAngle + 120f, toAngle - currentAngle));
				}
			}
			else if (currentAngle > toAngle - 60f)
			{
				if (limitJoint == null)
				{
					limitJoint = CreateLimitJoint(Mathf.Max(toAngle - currentAngle - 120f, fromAngle - currentAngle), toAngle - currentAngle);
				}
			}
			else if (limitJoint != null && currentAngle > fromAngle + 90f && currentAngle < toAngle - 90f)
			{
				Object.Destroy(limitJoint);
			}
			currentAngle = Mathf.Clamp(currentAngle, fromAngle, toAngle);
			currentValue = Mathf.InverseLerp(fromAngle, toAngle, currentAngle);
			output.SetValue((!signedOutput) ? currentValue : Mathf.Lerp(-1f, 1f, currentValue));
		}

		private HingeJoint CreateLimitJoint(float min, float max)
		{
			HingeJoint hingeJoint = base.gameObject.AddComponent<HingeJoint>();
			hingeJoint.anchor = joint.anchor;
			hingeJoint.axis = joint.axis;
			hingeJoint.autoConfigureConnectedAnchor = false;
			hingeJoint.connectedBody = joint.connectedBody;
			hingeJoint.connectedAnchor = joint.connectedAnchor;
			hingeJoint.useLimits = true;
			hingeJoint.limits = new JointLimits
			{
				min = min,
				max = max
			};
			return hingeJoint;
		}

		public override void Process()
		{
			base.Process();
			currentValue = ((!signedOutput) ? output.value : Mathf.InverseLerp(-1f, 1f, output.value));
			currentAngle = Mathf.Lerp(fromAngle, toAngle, currentValue);
			joint.ApplyXAngle(invInitialLocalRotation, currentAngle);
		}
	}
}
