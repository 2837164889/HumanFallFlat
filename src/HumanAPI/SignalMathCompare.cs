using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Signals/MathSignalMathCompare", 10)]
	public class SignalMathCompare : Node
	{
		public enum SignalCompareOperation
		{
			EqualExact,
			EqualApprox,
			NotEqualExact,
			NotEqualApprox,
			LessThan,
			GreaterThan,
			LessThanOrEqual,
			GreaterThanOrEqual
		}

		public NodeInput in1;

		public NodeInput in2;

		public NodeOutput output;

		public NodeOutput invertedOutput;

		public SignalCompareOperation operation;

		public override void Process()
		{
			bool flag;
			switch (operation)
			{
			case SignalCompareOperation.EqualExact:
				flag = (in1.value == in2.value);
				break;
			case SignalCompareOperation.EqualApprox:
				flag = Mathf.Approximately(in1.value, in2.value);
				break;
			case SignalCompareOperation.NotEqualExact:
				flag = (in1.value != in2.value);
				break;
			case SignalCompareOperation.NotEqualApprox:
				flag = !Mathf.Approximately(in1.value, in2.value);
				break;
			case SignalCompareOperation.LessThan:
				flag = (in1.value < in2.value);
				break;
			case SignalCompareOperation.GreaterThan:
				flag = (in1.value > in2.value);
				break;
			case SignalCompareOperation.LessThanOrEqual:
				flag = (in1.value <= in2.value);
				break;
			case SignalCompareOperation.GreaterThanOrEqual:
				flag = (in1.value >= in2.value);
				break;
			default:
				flag = false;
				break;
			}
			output.SetValue((!flag) ? 0f : 1f);
			invertedOutput.SetValue((!flag) ? 1f : 0f);
		}
	}
}
