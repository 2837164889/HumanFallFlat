using System;
using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Signals/Math/SignalMathSin", 10)]
	public class SignalMathSin : SignalFunction
	{
		public override float Calculate(float input)
		{
			return Mathf.Sin((float)Math.PI * 2f * input);
		}
	}
}
