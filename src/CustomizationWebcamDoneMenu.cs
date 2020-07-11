public class CustomizationWebcamDoneMenu : MenuTransition
{
	public override void OnBack()
	{
		BackClick();
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			MenuSystem.instance.GetMenu<CustomizationPaintMenu>().paint.CancelStroke();
			MenuSystem.instance.GetMenu<CustomizationWebcamMenu>().Teardown();
			CustomizationPaintMenu.dontReload = true;
			TransitionBack<CustomizationPaintMenu>();
		}
	}

	public void AcceptClick()
	{
		MenuSystem.instance.GetMenu<CustomizationPaintMenu>().paint.EndStroke();
		MenuSystem.instance.GetMenu<CustomizationWebcamMenu>().Teardown();
		CustomizationPaintMenu.dontReload = true;
		TransitionBack<CustomizationPaintMenu>();
	}

	public void RetakeClick()
	{
		TransitionBack<CustomizationWebcamMenu>();
	}
}
