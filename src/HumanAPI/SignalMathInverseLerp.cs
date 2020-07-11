using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Signals/Math/SignalMathInverseLerp", 10)]
	public class SignalMathInverseLerp : SignalFunction
	{
		public float from;

		public float to;

		public override float Calculate(float input)
		{
			return Mathf.InverseLerp(from, to, input);
		}
	}
}
