using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/Math/SignalMathDiv", 10)]
	public class SignalMathDiv : Node
	{
		public NodeInput in1;

		public NodeInput in2;

		public NodeOutput output;

		public override void Process()
		{
			if (in2.value != 0f)
			{
				output.SetValue(in1.value / in2.value);
			}
		}
	}
}
