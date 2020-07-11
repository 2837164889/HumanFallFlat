using I2.Loc;
using Multiplayer;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Dialogs
{
	public static void ShowHostServerProgress(Action onCancel)
	{
		DialogOverlay.Show(1f, showProgress: true, T("MULTIPLAYER/HOSTING.Title"), null, T("MULTIPLAYER/HOSTING.CANCEL"), onCancel);
	}

	public static void ShowListGamesProgress()
	{
		DialogOverlay.Show(1f, showProgress: true, T("MULTIPLAYER/LOBBY.ListingLobbies"), null, null, null);
	}

	public static void ShowJoinGameProgress(Action onCancel)
	{
		DialogOverlay.Show(1f, showProgress: true, T("MULTIPLAYER/CONNECTING.Title"), null, (onCancel == null) ? null : T("MULTIPLAYER/CONNECTING.CANCEL"), onCancel);
	}

	public static void ShowLoadLevelProgress(ulong levelID)
	{
		DialogOverlay.Show(1f, showProgress: true, T("TUTORIAL/LOADING"), null, null, null);
	}

	internal static void ShowCantAcceptInvite()
	{
		DialogOverlay.Show(1f, showProgress: false, T("MULTIPLAYER/CANTACCEPT.Title"), null, T("MULTIPLAYER/CANTACCEPT.CONTINUE"), delegate
		{
			HideProgress();
		});
	}

	public static void HideProgress(GameObject previousMenuItemToFocus = null, int hideDelay = 0)
	{
		if (previousMenuItemToFocus != null)
		{
			EventSystem.current.SetSelectedGameObject(previousMenuItemToFocus);
		}
		DialogOverlay.Hide(hideDelay);
	}

	public static void FailedToLoad(Action onBack, bool hideOldMenu = false)
	{
		MultiplayerErrorMenu.Show(T("WORKSHOP/FailedToLoadTitle"), T("WORKSHOP/FailedToLoadDesc"), T("MENU.PLAY/CONTINUE"), onBack, hideOldMenu);
	}

	public static void ConnectionKicked(Action onBack, bool hideOldMenu = false)
	{
		MultiplayerErrorMenu.Show(T("MULTIPLAYER/CONNECTIONLOST.Title"), T("MULTIPLAYER/KICKED.Description"), T("MULTIPLAYER/CONNECTIONLOST.CONTINUE"), onBack, hideOldMenu);
	}

	public static void ConnectionLost(Action onBack, bool hideOldMenu = false)
	{
		MultiplayerErrorMenu.Show(T("MULTIPLAYER/CONNECTIONLOST.Title"), T("MULTIPLAYER/CONNECTIONLOST.Description"), T("MULTIPLAYER/CONNECTIONLOST.CONTINUE"), onBack, hideOldMenu);
	}

	public static void ConnectionLostToHost(Action onBack, bool hideOldMenu = false)
	{
		ConnectionLost(onBack);
	}

	public static void ConnectionFailed(string error, Action onBack, bool hideOldMenu = false)
	{
		MultiplayerErrorMenu.Show(T("MULTIPLAYER/CONNECTIONFAILED.Title"), ErrorToString(error), T("MULTIPLAYER/CONNECTIONFAILED.CONTINUE"), onBack, hideOldMenu);
	}

	public static void ConnectionFailedVersion(string serverVersion, Action onBack, bool hideOldMenu = false)
	{
		MultiplayerErrorMenu.Show(T("MULTIPLAYER/CONNECTIONFAILED.Title"), string.Format(T("MULTIPLAYER/IncompatibleVersion"), serverVersion, VersionDisplay.fullVersion), T("MULTIPLAYER/CONNECTIONFAILED.CONTINUE"), onBack, hideOldMenu);
	}

	public static void CreateServerFailed(string error, Action onBack, bool hideOldMenu = false)
	{
		error = ((!string.IsNullOrEmpty(error)) ? ErrorToString(error) : string.Empty);
		MultiplayerErrorMenu.Show(T("MULTIPLAYER/SERVERFAILED.Title"), error, T("MULTIPLAYER/SERVERFAILED.CONTINUE"), onBack, hideOldMenu);
	}

	public static string T(string key)
	{
		string text = ScriptLocalization.Get(key);
		if (string.IsNullOrEmpty(text))
		{
			return key;
		}
		return text;
	}

	private static string ErrorToString(string error)
	{
		string text = ScriptLocalization.Get("MULTIPLAYER/" + error);
		if (string.IsNullOrEmpty(text))
		{
			return error;
		}
		return text;
	}
}
