using UnityEngine;

namespace HumanAPI
{
	public class SignalScriptSetMotorTargetVelocity1 : Node
	{
		[Tooltip("The incoming value to do something with")]
		public NodeInput input;

		[Tooltip("The amount to multiply the incoming for in line with the motor velocity")]
		public float multiplier = 1f;

		private HingeJoint localHinge;

		private JointMotor localMotor;

		private float incomingValue;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		private void Start()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Starting ");
			}
			localHinge = GetComponent<HingeJoint>();
			localMotor = localHinge.motor;
		}

		public override void Process()
		{
			incomingValue = input.value;
			if (incomingValue == 0f)
			{
				incomingValue = -1f;
			}
			incomingValue *= multiplier;
			localMotor.targetVelocity = incomingValue;
			localHinge.motor = localMotor;
			if (showDebug)
			{
				Debug.Log(base.name + " Changing Velocity to " + incomingValue);
			}
		}

		private void Update()
		{
		}
	}
}
