using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/Math/SignalMathThreshold", 10)]
	public class SignalMathThreshold : SignalFunction
	{
		public float treshold;

		private float inputCache;

		private float resultCache;

		public override string Title => "Threshold: " + treshold;

		public override float Calculate(float input)
		{
			inputCache = input;
			resultCache = ((input > treshold) ? 1 : 0);
			return (input > treshold) ? 1 : 0;
		}
	}
}
