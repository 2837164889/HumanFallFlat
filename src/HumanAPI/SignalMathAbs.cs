using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/Math/SignalMathAbs", 10)]
	public class SignalMathAbs : SignalFunction
	{
		public override float Calculate(float input)
		{
			return Mathf.Abs(input);
		}
	}
}
