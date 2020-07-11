using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/MathSignalMathInRange", 10)]
	public class SignalMathInRange : Node
	{
		public NodeInput input;

		public NodeOutput output;

		public NodeOutput invertedOutput;

		public float rangeLow = 0.5f;

		public float rangeHigh = 0.5f;

		public override string Title => "InRange: " + rangeLow + "-" + rangeHigh;

		public override void Process()
		{
			bool flag = input.value >= rangeLow && input.value <= rangeHigh;
			output.SetValue((!flag) ? 0f : 1f);
			invertedOutput.SetValue((!flag) ? 1f : 0f);
		}
	}
}
