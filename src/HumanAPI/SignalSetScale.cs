using UnityEngine;

namespace HumanAPI
{
	public class SignalSetScale : Node
	{
		public NodeInput input;

		public Vector3 targetScale = new Vector3(2f, 2f, 2f);

		public SignalBase triggerSignal;

		private Transform targetObject;

		private Vector3 initialScale;

		private void Start()
		{
			targetObject = GetComponent<Transform>();
			initialScale = targetObject.localScale;
			if (triggerSignal != null)
			{
				triggerSignal.onValueChanged += SignalChanged;
				SignalChanged(triggerSignal.value);
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

		public override void Process()
		{
			base.Process();
			SignalChanged(input.value);
		}

		private void SignalChanged(float val)
		{
			targetObject.localScale = Vector3.Lerp(initialScale, targetScale, val);
		}
	}
}
