using UnityEngine;

public class HingeMotorOnSignal : SignalTweenBase
{
	public float speedOn = 1f;

	private HingeJoint joint;

	protected override void OnEnable()
	{
		base.OnEnable();
		GetComponent<Rigidbody>().maxAngularVelocity = 50f;
	}

	public override void OnValueChanged(float value)
	{
		base.OnValueChanged(value);
		if (joint == null)
		{
			joint = GetComponent<HingeJoint>();
		}
		float num = speedOn * value;
		JointMotor motor = joint.motor;
		motor.targetVelocity = num;
		joint.useMotor = (num != 0f);
		joint.motor = motor;
	}
}
