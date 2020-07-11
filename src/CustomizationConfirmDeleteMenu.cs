using System;

public class CustomizationConfirmDeleteMenu : MenuTransition
{
	public static Action confirmDelete;

	public static Action cancelDelete;

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		CustomizationController.instance.cameraController.FocusCharacter();
	}

	public void ConfirmClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationPresetMenu.deleteSelectedOnLoad = true;
			TransitionBack<CustomizationPresetMenu>();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationPresetMenu.dontReload = true;
			TransitionBack<CustomizationPresetMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
