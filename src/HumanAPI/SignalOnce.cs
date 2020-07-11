using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	public class SignalOnce : Node, IReset
	{
		public NodeInput input;

		public NodeOutput output;

		public NodeInput reset;

		public bool latch;

		public bool dontReset;

		private bool sentSignal;

		public override void Process()
		{
			base.Process();
			if (reset.value >= 0.5f)
			{
				sentSignal = false;
			}
			if (Mathf.Abs(input.value) >= 0.5f && !sentSignal)
			{
				sentSignal = true;
				output.SetValue(1f);
			}
			else if (!latch || !sentSignal)
			{
				output.SetValue(0f);
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			if (!dontReset)
			{
				sentSignal = false;
			}
		}
	}
}
