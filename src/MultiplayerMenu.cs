using Multiplayer;
using UnityEngine;

public class MultiplayerMenu : MenuTransition
{
	public void HostClick()
	{
		if (MenuSystem.CanInvoke)
		{
			App.instance.HostGame();
		}
	}

	public void JoinClick()
	{
		if (MenuSystem.CanInvoke)
		{
			if (NetGame.instance.transport != null && NetGame.instance.transport.SupportsLobbyListings())
			{
				TransitionForward<MultiplayerSelectLobbyMenu>();
				return;
			}
			Debug.LogError("Need UI to enter IP");
			TransitionForward<MainMenu>();
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
