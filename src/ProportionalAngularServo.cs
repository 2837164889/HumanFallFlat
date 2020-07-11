using UnityEngine;

public class ProportionalAngularServo : ProportionalServoMotor
{
	public Vector3 axis;

	private ConfigurableJoint joint;

	private Rigidbody body;

	private Quaternion startRot;

	protected override void OnEnable()
	{
		base.OnEnable();
		body = GetComponent<Rigidbody>();
		joint = GetComponent<ConfigurableJoint>();
		if (body.isKinematic)
		{
			startRot = body.rotation;
		}
	}

	protected override void TargetValueChanged(float targetValue)
	{
		if (!(body == null))
		{
			if (body.isKinematic)
			{
				Quaternion rot = startRot * Quaternion.AngleAxis(targetValue, axis);
				body.MoveRotation(rot);
			}
			else
			{
				body.WakeUp();
				joint.targetRotation = Quaternion.AngleAxis(targetValue, axis);
			}
		}
	}
}
