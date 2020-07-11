using UnityEngine;

namespace HumanAPI
{
	public class SignalSetTranslation : Node
	{
		[Tooltip("The incoming value to do something with")]
		public NodeInput input;

		[Tooltip("The amount of motion to apply when getting a signal0")]
		public float multiplier = 1f;

		[Tooltip("The axis to move the thing in ")]
		public Vector3 translationVector = new Vector3(1f, 0f, 0f);

		[Tooltip("Affect local translation rather than global. Doesn't currently work for rigid bodies")]
		public bool transformLocal;

		[Tooltip("Trigge3r signal coming from somewhere ")]
		public SignalBase triggerSignal;

		private Rigidbody body;

		private Vector3 initialPosition;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		protected override void OnEnable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " On Enable ");
			}
			base.OnEnable();
			body = GetComponent<Rigidbody>();
			if (transformLocal && body == null)
			{
				initialPosition = base.transform.localPosition;
			}
			else
			{
				initialPosition = base.transform.position;
			}
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
				Debug.Log(base.name + " On Disable");
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
				Debug.Log(base.name + " Process Signal ");
			}
			base.Process();
			SignalChanged(input.value);
		}

		private void SignalChanged(float val)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " We are running the signal changed stuff ");
			}
			Vector3 b = val * multiplier * translationVector;
			if (body != null)
			{
				body.MovePosition(initialPosition + b);
			}
			else if (transformLocal)
			{
				base.transform.localPosition = initialPosition + b;
			}
			else
			{
				base.transform.position = initialPosition + b;
			}
		}
	}
}
