using HumanAPI;
using UnityEngine;

public class SignalScriptSetAngularVelocity1 : Node
{
	[Tooltip("Input for this node ")]
	public NodeInput input;

	[Tooltip("Max Force doesnt appear to be used")]
	public float maxForce = 1f;

	[Tooltip("Reference to the joint being tracked")]
	public HingeJoint joint;

	[Tooltip("The body object to spin in connection to the motor of the body")]
	public Rigidbody body;

	[Tooltip("This allows the amount of rotation to be increased from the input")]
	public float targetVelocityMultipier = 1f;

	private float incomingSignal;

	[Tooltip("Use this to invett the rotation")]
	public bool inverse;

	[Tooltip("Use this to show the prints coming from the script")]
	public bool showDebug;

	public override void Process()
	{
		incomingSignal = input.value;
		incomingSignal *= targetVelocityMultipier;
		JointMotor motor = joint.motor;
		motor.force = maxForce;
		if (incomingSignal > 0f)
		{
			motor.targetVelocity = incomingSignal;
			if (showDebug)
			{
				Debug.Log(base.name + " Positive Direction ");
				Debug.Log(base.name + " incomingSignal = " + incomingSignal);
			}
		}
		if (incomingSignal < 0f)
		{
			motor.targetVelocity = incomingSignal;
			if (showDebug)
			{
				Debug.Log(base.name + " Negative Direction ");
				Debug.Log(base.name + " incomingSignal = " + incomingSignal);
			}
		}
		joint.motor = motor;
	}
}
