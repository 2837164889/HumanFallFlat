using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/Math/SignalMathMul", 10)]
	public class SignalMathMul : Node
	{
		public NodeInput in1;

		public NodeInput in2;

		public NodeOutput output;

		public override void Process()
		{
			output.SetValue(in1.value * in2.value);
		}
	}
}
