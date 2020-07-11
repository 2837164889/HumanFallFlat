public class SignalTreshold : SignalBase
{
	public SignalBase source;

	public bool staysOn = true;

	public float treshold = 0.5f;

	public bool invert;

	protected override void OnEnable()
	{
		base.OnEnable();
		source.onValueChanged += SignalChanged;
	}

	protected override void OnDisable()
	{
		source.onValueChanged -= SignalChanged;
		base.OnDisable();
	}

	private void SignalChanged(float val)
	{
		if (!staysOn || !base.boolValue)
		{
			if (invert)
			{
				SetValue(source.value < treshold);
			}
			else
			{
				SetValue(source.value > treshold);
			}
		}
	}
}
