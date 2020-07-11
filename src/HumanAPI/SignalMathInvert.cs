using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Signals/Math/SignalMathInvert", 10)]
	public class SignalMathInvert : SignalFunction
	{
		public bool binaryOutput = true;

		public override float Calculate(float input)
		{
			if (binaryOutput)
			{
				if (input < 0.5f)
				{
					return 1f;
				}
				return 0f;
			}
			return 1f - input;
		}
	}
}
