public class ControlsMenu : MenuTransition
{
	public MenuSlider fovSlider;

	public MenuSlider smoothSlider;

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		fovSlider.value = Options.cameraFov;
		smoothSlider.value = Options.cameraSmoothing;
	}

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public void FovChanged(float value)
	{
		Options.cameraFov = (int)value;
	}

	public void SmoothChanged(float value)
	{
		Options.cameraSmoothing = (int)value;
	}

	public void MouseKeyboardClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<KeyboardMouseMenu>();
		}
	}

	public void ControllerClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<ControllerMenu>();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<OptionsMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
