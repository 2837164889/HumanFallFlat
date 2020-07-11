using System;

public class ConfirmResetMenu : MenuTransition
{
	public void ConfirmClick()
	{
		if (!MenuSystem.CanInvoke)
		{
			return;
		}
		throw new NotImplementedException();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<PlayMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
