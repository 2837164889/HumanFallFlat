namespace HumanAPI
{
	[AddNodeMenuItem]
	public class SignalMathConstant : Node
	{
		public float outputValue0;

		public float outputValue1;

		public float outputValue2;

		public float outputValue3;

		public NodeOutput output0;

		public NodeOutput output1;

		public NodeOutput output2;

		public NodeOutput output3;

		private void Update()
		{
			if (output0.value != outputValue0)
			{
				output0.SetValue(outputValue0);
			}
			if (output1.value != outputValue1)
			{
				output1.SetValue(outputValue1);
			}
			if (output2.value != outputValue2)
			{
				output2.SetValue(outputValue2);
			}
			if (output3.value != outputValue3)
			{
				output3.SetValue(outputValue3);
			}
		}
	}
}
