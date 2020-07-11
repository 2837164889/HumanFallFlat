using Multiplayer;

public class ConfirmExitMenu : MenuTransition
{
	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public void ConfirmClick()
	{
		if (MenuSystem.CanInvoke)
		{
			PlayerManager.SetSingle();
			App.instance.PauseLeave();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<PauseMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
