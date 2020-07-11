using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Signals/Math/SignalMathLerp", 10)]
	public class SignalMathLerp : SignalFunction
	{
		public float from;

		public float to;

		public override float Calculate(float input)
		{
			return Mathf.Lerp(from, to, input);
		}
	}
}
