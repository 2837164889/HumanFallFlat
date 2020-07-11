public class CustomizationConfirmDiscardMenu : MenuTransition
{
	public void ConfirmClick()
	{
		if (MenuSystem.CanInvoke)
		{
			MenuSystem.instance.GetMenu<CustomizationPaintMenu>().ConfirmDiscard();
			TransitionBack<CustomizationEditMenu>();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationPaintMenu.dontReload = true;
			TransitionBack<CustomizationPaintMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
