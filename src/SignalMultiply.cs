public class SignalMultiply : SignalBase
{
	public SignalBase[] sources;

	protected override void OnEnable()
	{
		base.OnEnable();
		for (int i = 0; i < sources.Length; i++)
		{
			sources[i].onValueChanged += SignalChanged;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		for (int i = 0; i < sources.Length; i++)
		{
			sources[i].onValueChanged += SignalChanged;
		}
	}

	private void SignalChanged(float val)
	{
		float num = 1f;
		for (int i = 0; i < sources.Length; i++)
		{
			num *= sources[i].value;
		}
		SetValue(num);
	}
}
