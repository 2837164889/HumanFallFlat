namespace HumanAPI
{
	public abstract class SignalFunction : Node
	{
		public NodeInput input;

		public NodeOutput output;

		protected void Awake()
		{
			output.SetValue(Calculate(input.value));
		}

		public override void Process()
		{
			base.Process();
			output.SetValue(Calculate(input.value));
		}

		public abstract float Calculate(float input);
	}
}
