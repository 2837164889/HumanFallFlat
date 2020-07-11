namespace HumanAPI
{
	public abstract class LerpBase : Node, IReset
	{
		public NodeInput input;

		public bool applyOnAwake;

		protected virtual void Awake()
		{
			if (applyOnAwake)
			{
				ApplyValue(input.value);
			}
		}

		public override void Process()
		{
			base.Process();
			ApplyValue(input.value);
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			Awake();
		}

		protected abstract void ApplyValue(float value);
	}
}
