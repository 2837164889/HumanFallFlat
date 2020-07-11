using HumanAPI;

public class SignalCombine : SignalBase
{
	public SignalBoolOperation operation;

	public SignalBase source1;

	public SignalBase source2;

	public bool staysOn = true;

	protected override void OnEnable()
	{
		base.OnEnable();
		source1.onValueChanged += SignalChanged;
		source2.onValueChanged += SignalChanged;
	}

	protected override void OnDisable()
	{
		source1.onValueChanged -= SignalChanged;
		source2.onValueChanged -= SignalChanged;
		base.OnDisable();
	}

	private void SignalChanged(float val)
	{
		if (!staysOn || !base.boolValue)
		{
			switch (operation)
			{
			case SignalBoolOperation.And:
				SetValue((source1.boolValue && source2.boolValue) ? 1 : 0);
				break;
			case SignalBoolOperation.Or:
				SetValue((source1.boolValue || source2.boolValue) ? 1 : 0);
				break;
			case SignalBoolOperation.Xor:
				SetValue((source1.boolValue ^ source2.boolValue) ? 1 : 0);
				break;
			}
		}
	}

	public override void ResetState(int checkpoint, int subObjectives)
	{
	}
}
