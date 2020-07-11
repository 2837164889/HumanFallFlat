using I2.Loc;
using Multiplayer;
using System;
using TMPro;

public class ConfirmMenu : MenuTransition
{
	public TextMeshProUGUI titleText;

	public TextMeshProUGUI descriptionText;

	public TextMeshProUGUI confirmText;

	public TextMeshProUGUI cancelText;

	private Action onConfirm;

	private Action onCancel;

	private void SetUp(string title, string description, string confirm, string cancel, Action onConfirm, Action onCancel)
	{
		titleText.text = ((!title.Contains("/")) ? title : ScriptLocalization.Get(title));
		descriptionText.text = ((!description.Contains("/")) ? description : ScriptLocalization.Get(description));
		confirmText.text = ScriptLocalization.Get(confirm);
		cancelText.text = ScriptLocalization.Get(cancel);
		this.onConfirm = onConfirm;
		this.onCancel = onCancel;
	}

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public void ConfirmClick()
	{
		if (MenuSystem.CanInvoke)
		{
			onConfirm();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			onCancel();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}

	public static void MultiplayerExitGame()
	{
		ConfirmMenu menu = MenuSystem.instance.GetMenu<ConfirmMenu>();
		if (NetGame.isServer)
		{
			menu.SetUp("MULTIPLAYER/CONFIRM.ExitToLobby", "MULTIPLAYER/CONFIRM.WillTransferToLobby", "MULTIPLAYER/CONFIRM.EXIT", "MULTIPLAYER/CONFIRM.CANCEL", delegate
			{
				App.instance.PauseLeave();
			}, delegate
			{
				menu.TransitionBack<MultiplayerPauseMenu>();
			});
		}
		else
		{
			menu.SetUp("MULTIPLAYER/CONFIRM.ExitMultiplayer", "MULTIPLAYER/CONFIRM.WillExitMultiplayer", "MULTIPLAYER/CONFIRM.EXIT", "MULTIPLAYER/CONFIRM.CANCEL", delegate
			{
				App.instance.PauseLeave();
			}, delegate
			{
				menu.TransitionBack<MultiplayerPauseMenu>();
			});
		}
		MenuSystem.instance.activeMenu.TransitionForward<ConfirmMenu>();
	}

	public static void MultiplayerLeaveLobby()
	{
		ConfirmMenu menu = MenuSystem.instance.GetMenu<ConfirmMenu>();
		if (NetGame.isServer)
		{
			if (NetGame.instance.allclients.Count <= 0)
			{
				App.instance.StopServer();
				return;
			}
			menu.SetUp("MULTIPLAYER/CONFIRM.StopLobby", "MULTIPLAYER/CONFIRM.WillDestroyLobby", "MULTIPLAYER/CONFIRM.STOPLOBBY", "MULTIPLAYER/CONFIRM.CANCEL", delegate
			{
				App.instance.StopServer();
			}, delegate
			{
				menu.TransitionBack<MultiplayerLobbyMenu>();
			});
		}
		else
		{
			menu.SetUp("MULTIPLAYER/CONFIRM.LeaveLobby", "MULTIPLAYER/CONFIRM.Leaving", "MULTIPLAYER/CONFIRM.LEAVE", "MULTIPLAYER/CONFIRM.CANCEL", delegate
			{
				App.instance.LeaveLobby();
			}, delegate
			{
				menu.TransitionBack<MultiplayerLobbyMenu>();
			});
		}
		MenuSystem.instance.activeMenu.TransitionForward<ConfirmMenu>();
	}
}
