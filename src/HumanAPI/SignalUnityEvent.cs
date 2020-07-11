using UnityEngine.Events;

namespace HumanAPI
{
	public class SignalUnityEvent : Node, IReset
	{
		public NodeInput input;

		public bool onlyTriggerOnce;

		public UnityEvent triggerEvent;

		public UnityEvent resetEvent;

		private bool hasTriggered;

		private float prevInput;

		public override void Process()
		{
			if (!onlyTriggerOnce || !hasTriggered)
			{
				if (input.value >= 0.5f && prevInput < 0.5f)
				{
					triggerEvent.Invoke();
					hasTriggered = true;
				}
				prevInput = input.value;
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			prevInput = 0f;
			hasTriggered = false;
			resetEvent.Invoke();
		}
	}
}
