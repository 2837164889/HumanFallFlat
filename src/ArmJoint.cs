using HumanAPI;
using UnityEngine;

public class ArmJoint : AngularJoint
{
	public float phase = 0.5f;

	protected override void UpdateLimitJoint()
	{
		float num = 0f - Mathf.Lerp(minValue, maxValue, phase) + centerValue;
		joint.lowAngularXLimit = new SoftJointLimit
		{
			limit = num
		};
		joint.highAngularXLimit = new SoftJointLimit
		{
			limit = num + 0.01f
		};
		body.GetComponent<Rigidbody>().WakeUp();
	}
}
