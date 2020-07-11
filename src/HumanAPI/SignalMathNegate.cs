using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/Math/SignalMathNegate", 10)]
	public class SignalMathNegate : Node
	{
		public NodeInput input;

		public NodeOutput output;

		public override void Process()
		{
			output.SetValue(0f - input.value);
		}
	}
}
