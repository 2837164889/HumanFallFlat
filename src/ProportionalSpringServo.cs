using UnityEngine;

public class ProportionalSpringServo : ProportionalServoMotor
{
	private ConfigurableJoint[] joints;

	private Rigidbody body;

	protected override void OnEnable()
	{
		base.OnEnable();
		body = GetComponent<Rigidbody>();
		joints = GetComponents<ConfigurableJoint>();
	}

	protected override void TargetValueChanged(float targetValue)
	{
		if (!(body == null))
		{
			for (int i = 0; i < joints.Length; i++)
			{
				ConfigurableJoint configurableJoint = joints[i];
				SoftJointLimit linearLimit = configurableJoint.linearLimit;
				linearLimit.limit = targetValue;
				configurableJoint.linearLimit = linearLimit;
			}
			body.WakeUp();
		}
	}
}
