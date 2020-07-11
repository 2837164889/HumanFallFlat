using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/SignalMathClamp", 10)]
	public class SignalMathClamp : Node
	{
		public NodeInput input;

		public NodeOutput output;

		public float min;

		public float max = 1f;

		public override void Process()
		{
			output.SetValue(Mathf.Clamp(input.value, min, max));
		}
	}
}
