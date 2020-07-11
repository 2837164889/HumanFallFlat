using UnityEngine;

public static class HingeJointExtensions
{
	public static void SetSpring(this HingeJoint joint, JointSpring spring)
	{
		JointSpring spring2 = joint.spring;
		if (spring2.damper != spring.damper || spring2.spring != spring.spring || spring2.targetPosition != spring.targetPosition)
		{
			joint.spring = spring;
		}
	}

	public static void SetLimits(this HingeJoint joint, JointLimits limits)
	{
		JointLimits limits2 = joint.limits;
		if (limits2.min != limits.min || limits2.max != limits.max)
		{
			joint.limits = limits;
		}
	}
}
