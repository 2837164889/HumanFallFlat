public class SignalButton : SignalBase, IGrabbable
{
	public bool toggle;

	public bool inverse;

	public bool toggleOnRelease;

	public void OnGrab()
	{
		if (toggle)
		{
			SetValue(1f - value);
		}
		else
		{
			SetValue((!inverse) ? 1 : 0);
		}
	}

	public void OnRelease()
	{
		if (toggleOnRelease)
		{
			if (toggle)
			{
				SetValue(1f - value);
			}
			else
			{
				SetValue(inverse ? 1 : 0);
			}
		}
	}
}
