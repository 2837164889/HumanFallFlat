using UnityEngine;

public class ProportionalLinearServo : ProportionalServoMotor
{
	public Vector3 direction;

	public float maxOffset = 0.3f;

	private float maxForce;

	private ConfigurableJoint joint;

	private Rigidbody body;

	private Vector3 startPos;

	protected override void OnEnable()
	{
		base.OnEnable();
		body = GetComponent<Rigidbody>();
		joint = GetComponent<ConfigurableJoint>();
		if (body.isKinematic)
		{
			startPos = base.transform.localPosition;
			return;
		}
		direction = joint.axis;
		maxForce = joint.xDrive.maximumForce;
	}

	protected override void TargetValueChanged(float targetValue)
	{
		if (!(body == null))
		{
			if (body.isKinematic)
			{
				Vector3 position = base.transform.parent.TransformPoint(startPos + targetValue * direction);
				body.MovePosition(position);
			}
			else
			{
				body.WakeUp();
				joint.targetPosition = new Vector3(targetValue, 0f, 0f);
			}
		}
	}

	protected override void PowerChanged(float powerPercent)
	{
		base.PowerChanged(powerPercent);
		JointDrive xDrive = joint.xDrive;
		xDrive.maximumForce = powerPercent * maxForce;
		joint.xDrive = xDrive;
		body.WakeUp();
	}
}
