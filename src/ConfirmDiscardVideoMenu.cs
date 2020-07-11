public class ConfirmDiscardVideoMenu : MenuTransition
{
	public static ConfirmDiscardNavigationTarget confirmTarget;

	public static ConfirmDiscardNavigationTarget backTarget = ConfirmDiscardNavigationTarget.Video;

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public void ConfirmClick()
	{
		if (MenuSystem.CanInvoke)
		{
			VideoMenu.hasUnappliedChanges = false;
			AdvancedVideoMenu.hasUnappliedChanges = false;
			switch (confirmTarget)
			{
			case ConfirmDiscardNavigationTarget.Options:
				TransitionForward<OptionsMenu>();
				break;
			case ConfirmDiscardNavigationTarget.Video:
				TransitionForward<VideoMenu>();
				break;
			case ConfirmDiscardNavigationTarget.AdvancedVideo:
				TransitionForward<AdvancedVideoMenu>();
				break;
			}
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (backTarget == ConfirmDiscardNavigationTarget.Video)
			{
				TransitionBack<VideoMenu>();
			}
			else
			{
				TransitionBack<AdvancedVideoMenu>();
			}
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
