using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Signals/Math/SignalMathFract", 10)]
	public class SignalMathFract : SignalFunction
	{
		public override float Calculate(float input)
		{
			return input - (float)Mathf.FloorToInt(input);
		}
	}
}
