using UnityEngine;

namespace HumanAPI
{
	public class SignalHold : Node
	{
		public float delay = 1f;

		private float timer;

		public NodeInput input;

		public NodeOutput output;

		protected void Awake()
		{
			priority = NodePriority.Update;
		}

		private void Update()
		{
			if (timer != 0f)
			{
				timer -= Time.deltaTime;
				if (timer <= 0f)
				{
					timer = 0f;
					output.SetValue(0f);
				}
			}
		}

		public override void Process()
		{
			base.Process();
			if (Mathf.Abs(input.value) >= 0.5f)
			{
				output.SetValue(1f);
				timer = 0f;
			}
			else if (output.value > 0.5f && timer == 0f)
			{
				timer = delay;
			}
		}
	}
}
