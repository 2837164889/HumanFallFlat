using UnityEngine;

namespace HumanAPI
{
	public class SignalAccumulate : Node, IReset
	{
		public NodeInput input;

		public NodeOutput output;

		public float countPerSecond = 1f;

		public bool clampMax = true;

		public float maxValue = 1f;

		public bool clampMin = true;

		public float minValue = -1f;

		public bool reset;

		private float initialValue;

		private void Start()
		{
			initialValue = output.value;
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			if (reset)
			{
				output.value = initialValue;
			}
		}

		private void FixedUpdate()
		{
			float num = output.value + input.value * Time.fixedDeltaTime * countPerSecond;
			if (clampMax)
			{
				num = Mathf.Min(maxValue, num);
			}
			if (clampMin)
			{
				num = Mathf.Max(minValue, num);
			}
			output.SetValue(num);
		}
	}
}
