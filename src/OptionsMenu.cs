using Multiplayer;

public class OptionsMenu : MenuTransition
{
	public static bool returnToPause;

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public void AudioClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<AudioMenu>();
		}
	}

	public void VideoClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<VideoMenu>();
		}
	}

	public void ControlsClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<ControlsMenu>();
		}
	}

	public void LanguageClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<LanguageMenu>();
		}
	}

	public void BackClick()
	{
		if (!MenuSystem.CanInvoke)
		{
			return;
		}
		if (returnToPause)
		{
			if (!NetGame.isLocal)
			{
				TransitionBack<MultiplayerPauseMenu>();
			}
			else
			{
				TransitionBack<PauseMenu>();
			}
		}
		else
		{
			TransitionBack<MainMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
