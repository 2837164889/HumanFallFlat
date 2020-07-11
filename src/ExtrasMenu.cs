using UnityEngine.UI;

public class ExtrasMenu : MenuTransition
{
	public Button logButton;

	public Button videoButton;

	public Button backButton;

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		if (VideoRepository.instance.HasAvailableItems())
		{
			videoButton.gameObject.SetActive(value: true);
			Link(logButton, videoButton);
			Link(videoButton, backButton);
		}
		else
		{
			videoButton.gameObject.SetActive(value: false);
			Link(logButton, backButton);
		}
		Link(backButton, logButton);
	}

	public void TextLogClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<TutorialLogMenu>();
		}
	}

	public void VideoLogClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<VideoLogMenu>();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<MainMenu>();
		}
	}

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public override void OnBack()
	{
		BackClick();
	}
}
