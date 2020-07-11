using UnityEngine;

namespace HumanAPI
{
	public class SignalSetRotation : Node
	{
		[Tooltip("The input the node gets from the graph")]
		public NodeInput input;

		[Tooltip("This value is signalled when a single full rotation has been achieved")]
		public NodeOutput tickedOver;

		[Tooltip("The amount to increaase any rotation by as a multiplier")]
		public float multiplier = 1f;

		[Tooltip("The direction the thing will rotate in")]
		public Vector3 rotationVector = new Vector3(1f, 0f, 0f);

		[Tooltip("If this body has a configurable joint, apply the rotation to that instead of the body itself")]
		public bool applyToJoint = true;

		[Tooltip("Signal system this node is using")]
		public SignalBase triggerSignal;

		public bool incrementlMovement;

		private float storedValue;

		private float storedRotation;

		private float storedOutput;

		private ConfigurableJoint joint;

		private Rigidbody body;

		private Quaternion initialRotation;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		protected override void OnEnable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Enabled ");
			}
			base.OnEnable();
			joint = GetComponent<ConfigurableJoint>();
			body = GetComponent<Rigidbody>();
			initialRotation = body.transform.rotation;
			if (triggerSignal != null)
			{
				triggerSignal.onValueChanged += SignalChanged;
				SignalChanged(triggerSignal.value);
			}
		}

		protected override void OnDisable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Disabled ");
			}
			base.OnDisable();
			if (triggerSignal != null)
			{
				triggerSignal.onValueChanged -= SignalChanged;
			}
		}

		public override void Process()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Process ");
			}
			base.Process();
			if (incrementlMovement)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Do the incremental stuff  ");
					Debug.Log(base.name + " Sored value * Multiplier =  " + storedValue * multiplier);
				}
				if (input.value == 0f)
				{
					storedValue = input.value + storedValue;
				}
				else
				{
					storedValue = 1f + storedValue;
				}
				SignalChanged(storedValue);
				storedRotation = storedValue * multiplier;
				if (storedRotation < 0f)
				{
					storedRotation *= -1f;
				}
				if (storedRotation >= 360f)
				{
					if (showDebug)
					{
						Debug.Log(base.name + " Gone too far ");
					}
					storedValue = 0f;
					storedRotation = 0f;
					storedOutput += 1f;
					tickedOver.SetValue(storedOutput);
				}
			}
			else
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Do the normal stuff ");
				}
				SignalChanged(input.value);
			}
		}

		private void SignalChanged(float val)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Signal Changed ");
				Debug.Log(base.name + " Value = " + val);
			}
			Quaternion quaternion = Quaternion.AngleAxis(val * multiplier, rotationVector);
			if (applyToJoint && joint != null)
			{
				joint.targetRotation = quaternion;
				return;
			}
			Quaternion quaternion2 = initialRotation * quaternion;
			if (body.isKinematic)
			{
				body.transform.rotation = quaternion2;
			}
			else
			{
				body.MoveRotation(quaternion2);
			}
		}
	}
}
