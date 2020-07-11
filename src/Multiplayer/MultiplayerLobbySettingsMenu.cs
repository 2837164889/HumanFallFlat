namespace Multiplayer
{
	public class MultiplayerLobbySettingsMenu : MenuTransition
	{
		public MenuSelector maxPlayersSelector;

		public MenuSelector inviteOnlySelector;

		public MenuSelector joingInProgressSelector;

		public MenuSelector levelProgressSelector;

		public override void ApplyMenuEffects()
		{
			MenuCameraEffects.FadeInPauseMenu();
		}

		public override void OnGotFocus()
		{
			base.OnGotFocus();
			inviteOnlySelector.gameObject.SetActive(value: true);
			maxPlayersSelector.SelectIndex(Options.lobbyMaxPlayers - 2);
			inviteOnlySelector.SelectIndex(Options.lobbyInviteOnly);
			joingInProgressSelector.SelectIndex(Options.lobbyJoinInProgress);
			levelProgressSelector.SelectIndex(Options.lobbyLockLevel);
		}

		public void BackClick()
		{
			if (MenuSystem.CanInvoke)
			{
				TransitionBack<MultiplayerLobbyMenu>();
			}
		}

		public override void OnBack()
		{
			BackClick();
		}

		public void InviteOnlyChanged(int value)
		{
			Options.lobbyInviteOnly = value;
			NetGame.instance.transport.UpdateLobbyType();
			NetGame.instance.transport.UpdateOptionsLobbyData();
		}

		public void MaxPlayersChanged(int value)
		{
			Options.lobbyMaxPlayers = value + 2;
			NetGame.instance.transport.UpdateLobbyPlayers();
			App.instance.OnClientCountChanged();
			NetGame.instance.transport.UpdateOptionsLobbyData();
		}

		public void JoinInProgressChanged(int value)
		{
			Options.lobbyJoinInProgress = value;
			NetGame.instance.transport.UpdateJoinInProgress();
			NetGame.instance.transport.UpdateOptionsLobbyData();
		}

		public void LevelProgressChanged(int value)
		{
			Options.lobbyLockLevel = value;
			NetGame.instance.transport.UpdateOptionsLobbyData();
		}
	}
}
