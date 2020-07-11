using HumanAPI;
using I2.Loc;
using Multiplayer;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelInformationBox : MonoBehaviour
{
	private enum InformationStrings
	{
		kEnabled,
		kDisabled,
		kInGame,
		kInLobby,
		kPlayers,
		kLobbyLevel,
		kJoinGameInProgress,
		kLockLevel,
		kInviteOnly,
		kMaxInformationStrings
	}

	public RawImage LevelImage;

	public Text LevelText;

	public Text LaunchedText;

	public Text LobbyNameText;

	public Text MaxPlayersText;

	public Text InviteText;

	public Text JoinGameInProgressText;

	public Text LockLevelText;

	private const int kMaxStringLength = 128;

	private const byte kTriStateBoolFalse = 0;

	private const byte kTriStateBoolTrue = 1;

	private const byte kTriStateBoolUnInit = 2;

	private static readonly string[] sInformationStrings = new string[9]
	{
		"MENU/COMMON/Enabled",
		"MENU/COMMON/Disabled",
		"MULTIPLAYER/LOBBYINFO.InGame",
		"MULTIPLAYER/LOBBYINFO.InLobby",
		"MULTIPLAYER/LOBBYINFO.Players",
		"MULTIPLAYER/LOBBYINFO.LobbyLevel",
		"MULTIPLAYER/LOBBYSETTINGS.GameInProgress",
		"MULTIPLAYER/LOBBYSETTINGS.LockLevel",
		"MULTIPLAYER/LOBBYSETTINGS.InviteOnly"
	};

	private static string[] sLocalisedStrings;

	private static StringBuilder sbInformationStrings = new StringBuilder(128);

	private static int sNumberInformationStrings;

	private NetTransport.LobbyDisplayInfo prevDispInfo;

	private void Awake()
	{
		sNumberInformationStrings = 9;
		sLocalisedStrings = new string[sNumberInformationStrings];
		GetInformationStrings();
	}

	private void OnEnable()
	{
		GetInformationStrings();
		ClearDisplay();
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void GetInformationStrings()
	{
		for (int i = 0; i < sNumberInformationStrings; i++)
		{
			sLocalisedStrings[i] = ScriptLocalization.Get(sInformationStrings[i]);
		}
	}

	private string MakeLobbyTitle(string lobbyTitle)
	{
		lobbyTitle = WorkshopRepository.GetLobbyTitle(lobbyTitle);
		sbInformationStrings.Length = 0;
		sbInformationStrings.AppendFormat("{0}: {1}", sLocalisedStrings[5], lobbyTitle);
		return sbInformationStrings.ToString();
	}

	private string MakePlayers(uint currentPlayers, uint maxPlayers, int stringID)
	{
		sbInformationStrings.Length = 0;
		sbInformationStrings.AppendFormat("{0}: {1}/{2}", sLocalisedStrings[stringID], currentPlayers, maxPlayers);
		return sbInformationStrings.ToString();
	}

	private string MakeFlag(uint state, uint feature, uint mask, int stringID)
	{
		if ((feature & mask) == 0)
		{
			return string.Empty;
		}
		int num = ((state & mask) == 0) ? 1 : 0;
		sbInformationStrings.Length = 0;
		sbInformationStrings.AppendFormat("{0}: {1}", sLocalisedStrings[stringID], sLocalisedStrings[num]);
		return sbInformationStrings.ToString();
	}

	public void ClearDisplay()
	{
		prevDispInfo.InitBlank();
		MaxPlayersText.text = string.Empty;
		JoinGameInProgressText.text = string.Empty;
		InviteText.text = string.Empty;
		LockLevelText.text = string.Empty;
		LaunchedText.text = string.Empty;
		LobbyNameText.text = string.Empty;
		if (LevelText != null)
		{
			LevelText.text = string.Empty;
		}
		if (LevelImage != null)
		{
			LevelImage.texture = null;
			LevelImage.enabled = false;
		}
	}

	public void UpdateDisplay(NetTransport.ILobbyEntry data)
	{
		NetTransport.LobbyDisplayInfo info = default(NetTransport.LobbyDisplayInfo);
		if (data == null)
		{
			info.InitBlank();
		}
		else
		{
			data.getDisplayInfo(out info);
		}
		UpdateDisplay(info);
	}

	public void UpdateDisplay(NetTransport.LobbyDisplayInfo dispInfo)
	{
		if (InviteText != null)
		{
			InviteText.gameObject.SetActive(NetGame.instance.transport.CanSendInvite());
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (dispInfo.FeaturesMask == 0)
		{
			ClearDisplay();
			base.gameObject.SetActive(value: false);
			return;
		}
		if ((dispInfo.FeaturesMask & 0x20) != 0 && (dispInfo.Flags & 0x20) == 0)
		{
			ClearDisplay();
			base.gameObject.SetActive(value: false);
			return;
		}
		uint num = dispInfo.Compare(ref prevDispInfo);
		prevDispInfo = dispInfo;
		uint num2 = 3221225472u;
		if (((int)num & -1073741824) != 0)
		{
			if (dispInfo.ShouldDisplayAllAttr(3221225472u))
			{
				MaxPlayersText.text = MakePlayers(dispInfo.NumPlayersForDisplay, dispInfo.MaxPlayers, 4);
			}
			else
			{
				MaxPlayersText.text = string.Empty;
			}
		}
		if ((num & 1) != 0)
		{
			JoinGameInProgressText.text = MakeFlag(dispInfo.Flags, dispInfo.FeaturesMask, 1u, 6);
		}
		if ((num & 4) != 0)
		{
			InviteText.text = MakeFlag(dispInfo.Flags, dispInfo.FeaturesMask, 4u, 8);
		}
		if ((num & 2) != 0)
		{
			LockLevelText.text = MakeFlag(dispInfo.Flags, dispInfo.FeaturesMask, 2u, 7);
		}
		if ((num & 8) != 0)
		{
			if ((dispInfo.FeaturesMask & 8) != 0)
			{
				LaunchedText.text = (((dispInfo.Flags & 8) == 0) ? sLocalisedStrings[3] : sLocalisedStrings[2]);
			}
			else
			{
				LaunchedText.text = string.Empty;
			}
		}
		if ((num & 0x8000000) != 0)
		{
			if ((dispInfo.FeaturesMask & 0x8000000) != 0)
			{
				LobbyNameText.text = MakeLobbyTitle(dispInfo.LobbyTitle);
			}
			else
			{
				LobbyNameText.text = string.Empty;
			}
		}
		if (LevelText == null || LevelImage == null || (num & 0x20000000) == 0)
		{
			return;
		}
		if ((dispInfo.FeaturesMask & 0x20000000) == 0)
		{
			LevelText.text = string.Empty;
			LevelImage.texture = null;
			LevelImage.enabled = false;
			return;
		}
		string path;
		switch (dispInfo.LevelType)
		{
		case WorkshopItemSource.BuiltIn:
			WorkshopRepository.instance.LoadBuiltinLevels();
			path = "builtin:" + dispInfo.LevelID;
			break;
		case WorkshopItemSource.EditorPick:
			WorkshopRepository.instance.LoadEditorPickLevels();
			path = "editorpick:" + dispInfo.LevelID;
			break;
		default:
			path = "ws:" + dispInfo.LevelID + "/";
			break;
		}
		WorkshopLevelMetadata item = WorkshopRepository.instance.levelRepo.GetItem(path);
		if (item != null)
		{
			LevelText.text = item.title;
			LevelImage.texture = item.thumbnailTexture;
			LevelImage.enabled = (LevelImage.texture != null);
		}
		else if (dispInfo.LevelID == (ulong)(Game.instance.levels.Length - 1))
		{
			LevelText.text = ScriptLocalization.Get("LEVEL/" + Game.instance.levels[Game.instance.levels.Length - 1]);
			LevelImage.texture = HFFResources.instance.FindTextureResource("LevelImages/" + Game.instance.levels[Game.instance.levels.Length - 1]);
			LevelImage.enabled = true;
		}
		else
		{
			LevelText.text = string.Empty;
			LevelImage.texture = null;
			LevelImage.enabled = false;
			StartCoroutine(GetNewLevel(dispInfo.LevelID));
		}
	}

	private IEnumerator GetNewLevel(ulong levelID)
	{
		bool loaded = false;
		WorkshopLevelMetadata levelData;
		WorkshopRepository.instance.levelRepo.LoadLevel(levelID, delegate(WorkshopLevelMetadata l)
		{
			levelData = l;
			loaded = true;
			if (levelData != null && (prevDispInfo.FeaturesMask & 0x20000000) != 0 && prevDispInfo.LevelID == levelID)
			{
				LevelText.text = levelData.title;
				LevelImage.texture = levelData.thumbnailTexture;
				LevelImage.enabled = (LevelImage.texture != null);
			}
		});
		while (!loaded)
		{
			yield return null;
		}
	}
}
