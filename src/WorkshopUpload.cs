using HumanAPI;
using I2.Loc;
using Multiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

public static class WorkshopUpload
{
	private static WorkshopItemMetadata meta;

	private static string folder;

	private static string thumbnail;

	private static string updateNotes;

	private static Action<WorkshopItemMetadata, bool, EResult> uploadCallback;

	private static CallResult<CreateItemResult_t> m_CreateItemResult;

	private static CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdateResult;

	private static CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

	private const string kRagdollPreset = "RagdollPreset";

	private const string kSkinTag = "Model";

	private const string kLevelTag = "Levels";

	private const string kLobbyTag = "Lobbies";

	public const string kThemeSeparator = ", ";

	public const int kMaxTagType = 3;

	public const string kTagTypeBase = "WORKSHOP/LevelTypeDesc";

	public const int kMaxTagRecommendedPlayers = 8;

	public const string kTagRecommendedPlayersBase = "WORKSHOP/RecommendedPlayersDesc";

	private static StringBuilder tagBuilder = new StringBuilder();

	private static bool m_bInitialized;

	[CompilerGenerated]
	private static CallResult<CreateItemResult_t>.APIDispatchDelegate _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate _003C_003Ef__mg_0024cache2;

	private static bool Initialize()
	{
		return true;
	}

	public static void Upload(WorkshopItemMetadata meta, string folder, string thumbnail, string updateNotes, Action<WorkshopItemMetadata, bool, EResult> uploadCallback)
	{
		if (!Initialize())
		{
			uploadCallback(meta, arg2: false, EResult.k_EResultConnectFailed);
			return;
		}
		WorkshopUpload.uploadCallback = uploadCallback;
		WorkshopUpload.meta = meta;
		WorkshopUpload.folder = folder;
		WorkshopUpload.thumbnail = thumbnail;
		WorkshopUpload.updateNotes = updateNotes;
		Upload();
	}

	private static void Upload()
	{
		if (meta.workshopId == 0)
		{
			m_CreateItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
			SteamAPICall_t hAPICall = SteamUGC.CreateItem((AppId_t)477160u, EWorkshopFileType.k_EWorkshopFileTypeFirst);
			m_CreateItemResult.Set(hAPICall);
		}
		else
		{
			PerformUpload(preExisting: true);
		}
	}

	private static void OnCreateItemResult(CreateItemResult_t res, bool bIOFailure)
	{
		m_CreateItemResult = null;
		if (res.m_bUserNeedsToAcceptWorkshopLegalAgreement || res.m_eResult == EResult.k_EResultInsufficientPrivilege || res.m_eResult == EResult.k_EResultTimeout || res.m_eResult == EResult.k_EResultNotLoggedOn)
		{
			uploadCallback(meta, res.m_bUserNeedsToAcceptWorkshopLegalAgreement, res.m_eResult);
			uploadCallback = null;
		}
		else
		{
			meta.workshopId = (ulong)res.m_nPublishedFileId;
			meta.Save(folder);
			PerformUpload();
		}
	}

	public static string TagStrings(string baseID, int value, bool english = false)
	{
		tagBuilder.Length = 0;
		tagBuilder.Append(baseID);
		tagBuilder.Append(value);
		if (english)
		{
			return LocalizationManager.GetTermTranslation(tagBuilder.ToString(), 3);
		}
		return ScriptLocalization.Get(tagBuilder.ToString());
	}

	private static void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
	{
		if (param.m_eResult == EResult.k_EResultOK && SteamUGC.GetQueryUGCResult(param.m_handle, 0u, out SteamUGCDetails_t pDetails) && pDetails.m_eResult == EResult.k_EResultFileNotFound)
		{
			meta.workshopId = 0uL;
			meta.Save(folder);
			Upload();
		}
		else
		{
			PerformUpload();
		}
	}

	private static void PerformUpload(bool preExisting = false)
	{
		if (preExisting)
		{
			UGCQueryHandle_t handle = SteamUGC.CreateQueryUGCDetailsRequest(new PublishedFileId_t[1]
			{
				new PublishedFileId_t(meta.workshopId)
			}, 1u);
			OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
			SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(handle);
			OnSteamUGCQueryCompletedCallResult.Set(hAPICall);
			return;
		}
		UGCUpdateHandle_t uGCUpdateHandle_t = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), (PublishedFileId_t)meta.workshopId);
		string[] pTags = null;
		string type = meta.type;
		bool flag = false;
		if (type.Equals("RagdollPreset"))
		{
			type = "Model";
			flag = true;
			pTags = new string[1]
			{
				"Model"
			};
		}
		if (!flag)
		{
			List<string> list = new List<string>();
			if (meta.typeTags == 2)
			{
				list.Add("Lobbies");
			}
			else
			{
				list.Add("Levels");
				if (meta.typeTags > 0 && meta.typeTags < 3)
				{
					list.Add(TagStrings("WORKSHOP/LevelTypeDesc", meta.typeTags, english: true));
				}
			}
			if (meta.playerTags > 0 && meta.playerTags < 8)
			{
				list.Add(TagStrings("WORKSHOP/RecommendedPlayersDesc", meta.playerTags, english: true));
			}
			if (meta.themeTags != null)
			{
				string[] separator = new string[1]
				{
					", "
				};
				string[] collection = meta.themeTags.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				list.AddRange(collection);
			}
			pTags = list.ToArray();
		}
		SteamUGC.SetItemTags(uGCUpdateHandle_t, pTags);
		SteamUGC.SetItemTitle(uGCUpdateHandle_t, meta.title);
		SteamUGC.SetItemDescription(uGCUpdateHandle_t, meta.description);
		SteamUGC.SetItemPreview(uGCUpdateHandle_t, FileTools.ToLocalPath(thumbnail));
		SteamUGC.SetItemContent(uGCUpdateHandle_t, FileTools.ToLocalPath(folder));
		if (flag)
		{
			SteamUGC.SetItemVisibility(uGCUpdateHandle_t, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
		}
		m_SubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
		SteamAPICall_t hAPICall2 = SteamUGC.SubmitItemUpdate(uGCUpdateHandle_t, updateNotes);
		m_SubmitItemUpdateResult.Set(hAPICall2);
	}

	private static void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t res, bool bIOFailure)
	{
		m_SubmitItemUpdateResult = null;
		if (res.m_bUserNeedsToAcceptWorkshopLegalAgreement || res.m_eResult != EResult.k_EResultOK)
		{
			uploadCallback(meta, res.m_bUserNeedsToAcceptWorkshopLegalAgreement, res.m_eResult);
			uploadCallback = null;
		}
		else
		{
			ShowWorkshopItem(meta.workshopId);
			uploadCallback(meta, arg2: false, EResult.k_EResultOK);
			uploadCallback = null;
		}
	}

	public static void UploadMonitor()
	{
		if (!NetTransportSteam.sSteamServersConnected)
		{
			bool flag = false;
			if (m_CreateItemResult != null)
			{
				m_CreateItemResult.Cancel();
				m_CreateItemResult = null;
				flag = true;
			}
			if (m_SubmitItemUpdateResult != null)
			{
				m_SubmitItemUpdateResult.Cancel();
				m_SubmitItemUpdateResult = null;
				flag = true;
			}
			if (flag && uploadCallback != null)
			{
				uploadCallback(meta, arg2: false, EResult.k_EResultCancelled);
				uploadCallback = null;
			}
		}
	}

	public static void ShowWorkshopItem(ulong steamId)
	{
		SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/" + steamId);
	}

	public static void ShowWorkshop()
	{
		SteamFriends.ActivateGameOverlayToWebPage("steam://url/SteamWorkshopPage/" + 477160);
	}

	public static void ShowLevelWorkshop()
	{
		SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/workshop/browse/?appid=477160&requiredtags[]=Levels");
	}

	public static void ShowLobbyWorkshop()
	{
		SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/workshop/browse/?appid=477160&requiredtags[]=Lobbies");
	}

	public static void ShowSkinWorkshop()
	{
		ShowUrl("http://steamcommunity.com/workshop/browse/?appid=477160&requiredtags[]=Model");
	}

	public static void ShowWorkshopAgreement()
	{
		SteamFriends.ActivateGameOverlayToWebPage("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
	}

	public static void ShowUrl(string url)
	{
		SteamFriends.ActivateGameOverlayToWebPage(url);
	}
}
