using Multiplayer;

public class CreditsMenu : MenuTransition
{
	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public void ResumeClick()
	{
		if (MenuSystem.CanInvoke)
		{
			Resume();
		}
	}

	private void Resume()
	{
		Game.instance.Resume();
		MenuCameraEffects.FadeInCredits();
		MenuSystem.instance.ExitMenus();
		FadeOutBack();
	}

	public void ExitClick()
	{
		if (MenuSystem.CanInvoke)
		{
			App.instance.PauseLeave();
		}
	}

	public override void OnBack()
	{
		ResumeClick();
	}
}
