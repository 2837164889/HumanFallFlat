using System;
using UnityEngine;

namespace HumanAPI
{
	public class SignalSetAngularVelocity : Node
	{
		[Tooltip("Input for this node ")]
		public NodeInput input;

		[Tooltip("Max Force doesnt appear to be used")]
		public float maxForce = 1f;

		[Tooltip("Used when working out the target velocity")]
		public float maxVelocity = 1f;

		[Tooltip("Used in fixed update to the angular velocity")]
		public bool infiniteTorque;

		[Tooltip("Used in fixed update to the angular velocity")]
		public bool freeSpin;

		[Tooltip("Whether or not the thing should move without a power source")]
		public bool Powered;

		[Tooltip("Var for checking the change in the incoming graph node setting")]
		public SignalBase triggerSignal;

		[Tooltip("Reference to the joint being tracked")]
		public HingeJoint joint;

		[Tooltip("The body object to spin in connection to the motor of the body")]
		public Rigidbody body;

		[Tooltip("Use this to show the prints coming from the script")]
		public bool showDebug;

		protected override void OnEnable()
		{
			base.OnEnable();
			if (joint == null)
			{
				joint = GetComponent<HingeJoint>();
			}
			if (body == null)
			{
				body = GetComponent<Rigidbody>();
			}
			if (body != null)
			{
				body.maxAngularVelocity = 100f;
				if (triggerSignal != null)
				{
					triggerSignal.onValueChanged += SignalChanged;
					SignalChanged(triggerSignal.value);
				}
				else if (Powered)
				{
					SignalChanged(1f);
				}
			}
			else if (showDebug)
			{
				Debug.Log(base.name + " There is nothing set within the body var ");
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (triggerSignal != null)
			{
				triggerSignal.onValueChanged -= SignalChanged;
			}
		}

		private void SignalChanged(float val)
		{
			if (joint != null)
			{
				float num = maxForce * ((!freeSpin) ? 1f : Mathf.Abs(val));
				joint.useMotor = (num > 0f);
				joint.motor = new JointMotor
				{
					force = num,
					targetVelocity = maxVelocity * val
				};
			}
			else if (showDebug)
			{
				Debug.Log(base.name + " There is nothing set within the joint var ");
			}
		}

		public override void Process()
		{
			base.Process();
			SignalChanged(input.value);
		}

		private void FixedUpdate()
		{
			if (triggerSignal != null)
			{
				if (infiniteTorque && (triggerSignal.value != 0f || !freeSpin))
				{
					body.angularVelocity = base.transform.TransformDirection(joint.axis.normalized) * maxVelocity * ((float)Math.PI / 180f) * triggerSignal.value;
				}
			}
			else if (infiniteTorque && (input.value != 0f || !freeSpin))
			{
				body.angularVelocity = base.transform.TransformDirection(joint.axis.normalized) * maxVelocity * ((float)Math.PI / 180f) * input.value;
			}
		}
	}
}
