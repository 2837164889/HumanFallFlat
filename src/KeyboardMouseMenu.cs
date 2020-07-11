public class KeyboardMouseMenu : MenuTransition
{
	public MenuSlider sensitivitySlider;

	public MenuSelector invertSelector;

	public MenuSelector lookModeSelector;

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		sensitivitySlider.value = Options.mouseSensitivity;
		invertSelector.SelectIndex(Options.mouseInvert);
		lookModeSelector.SelectIndex(Options.mouseLookMode);
	}

	public void ConfigureClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<ConfigureKeysMenu>();
		}
	}

	public void SensitivityChanged(float value)
	{
		Options.mouseSensitivity = value;
	}

	public void InvertChanged(int value)
	{
		Options.mouseInvert = value;
	}

	public void LookModeChanged(int value)
	{
		Options.mouseLookMode = value;
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<ControlsMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
