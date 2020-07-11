using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Automatic Door Dual", 10)]
	public class AutomaticDoorDual : Node
	{
		public NodeInput input;

		public ConfigurableJoint leftJoint;

		public ConfigurableJoint rightJoint;

		public float doorForce = 1000f;

		public float doorSpeed = 1f;

		public float gap = 2f;

		private Rigidbody leftDoor;

		private Rigidbody rightDoor;

		private float targetGap;

		private float tensionDist = 0.05f;

		protected void Awake()
		{
			leftDoor = leftJoint.GetComponent<Rigidbody>();
			rightDoor = rightJoint.GetComponent<Rigidbody>();
			Vector3 position = (leftJoint.transform.position + rightJoint.transform.position) / 2f;
			ConfigurableJoint configurableJoint = leftJoint;
			bool autoConfigureConnectedAnchor = false;
			rightJoint.autoConfigureConnectedAnchor = autoConfigureConnectedAnchor;
			configurableJoint.autoConfigureConnectedAnchor = autoConfigureConnectedAnchor;
			leftJoint.anchor = leftJoint.transform.InverseTransformPoint(position);
			rightJoint.anchor = rightJoint.transform.InverseTransformPoint(position);
			leftJoint.SetXMotionAnchorsAndLimits(gap / 4f, gap / 2f);
			rightJoint.SetXMotionAnchorsAndLimits(gap / 4f, gap / 2f);
		}

		public override void Process()
		{
			base.Process();
			if (SignalManager.skipTransitions)
			{
				targetGap = Mathf.Lerp(0f, gap, input.value);
				ApplyTargetGap();
				leftJoint.ApplyXMotionTarget();
				rightJoint.ApplyXMotionTarget();
			}
		}

		private void FixedUpdate()
		{
			float num = targetGap;
			float magnitude = (leftDoor.transform.TransformPoint(leftJoint.anchor) - rightDoor.transform.TransformPoint(leftJoint.anchor)).magnitude;
			float target = Mathf.Lerp(0f, gap, input.value);
			targetGap = Mathf.MoveTowards(magnitude, target, doorSpeed * Time.fixedDeltaTime);
			float num2 = (!(Mathf.Abs(input.value) >= 0.5f)) ? (0f - tensionDist) : tensionDist;
			targetGap += num2 * 2f;
			targetGap = Mathf.Clamp(targetGap, 0f, gap);
			if (num != targetGap)
			{
				ApplyTargetGap();
			}
		}

		private void ApplyTargetGap()
		{
			JointDrive xDrive = leftJoint.xDrive;
			xDrive.positionSpring = doorForce / tensionDist;
			xDrive.positionDamper = doorForce / doorSpeed;
			leftJoint.xDrive = xDrive;
			rightJoint.xDrive = xDrive;
			leftJoint.targetPosition = new Vector3((0f - targetGap) / 2f + gap / 4f, 0f, 0f);
			rightJoint.targetPosition = new Vector3((0f - targetGap) / 2f + gap / 4f, 0f, 0f);
			leftDoor.WakeUp();
			rightDoor.WakeUp();
		}
	}
}
