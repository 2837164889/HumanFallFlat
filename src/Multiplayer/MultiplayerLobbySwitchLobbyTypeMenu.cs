using I2.Loc;
using TMPro;

namespace Multiplayer
{
	public class MultiplayerLobbySwitchLobbyTypeMenu : MenuTransition
	{
		public TextMeshProUGUI titleText;

		public override void ApplyMenuEffects()
		{
			MenuCameraEffects.FadeInPauseMenu();
		}

		public override void OnGotFocus()
		{
			base.OnGotFocus();
			titleText.text = ScriptLocalization.Get("MULTIPLAYER/SWITCHMODE.Title");
		}

		public void BackClick()
		{
			if (MenuSystem.CanInvoke)
			{
				TransitionBack<SelectPlayersMenu>();
			}
		}

		public override void OnBack()
		{
			BackClick();
		}

		public void SetLobbyType(bool internet)
		{
		}

		public void InternetClick()
		{
			if (MenuSystem.CanInvoke)
			{
				TransitionForward<MultiplayerSelectLobbyMenu>();
				SetLobbyType(internet: true);
			}
		}

		public void LocalClick()
		{
			if (MenuSystem.CanInvoke)
			{
				TransitionForward<MultiplayerSelectLobbyMenu>();
				SetLobbyType(internet: false);
			}
		}
	}
}
