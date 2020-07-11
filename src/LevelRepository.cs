using HumanAPI;
using Steamworks;
using System;
using UnityEngine;

public class LevelRepository : WorkshopTypeRepository<WorkshopLevelMetadata>
{
	private Action<WorkshopLevelMetadata> onRead;

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

	private CallResult<RemoteStorageDownloadUGCResult_t> OnUGCDownloadCallResult;

	private SteamUGCDetails_t UGCDetails;

	private Callback<DownloadItemResult_t> OnDownloadItemCallResult;

	public void GetLevel(ulong levelId, WorkshopItemSource levelType, Action<WorkshopLevelMetadata> onRead)
	{
		if (OnSteamUGCQueryCompletedCallResult != null)
		{
			OnSteamUGCQueryCompletedCallResult.Cancel();
			OnSteamUGCQueryCompletedCallResult = null;
		}
		if (OnUGCDownloadCallResult != null)
		{
			OnUGCDownloadCallResult.Cancel();
			OnUGCDownloadCallResult = null;
		}
		switch (levelType)
		{
		case WorkshopItemSource.BuiltIn:
			onRead(GetItem("builtin:" + levelId.ToString()));
			return;
		case WorkshopItemSource.EditorPick:
			onRead(GetItem("editorpick:" + levelId.ToString()));
			return;
		}
		WorkshopLevelMetadata item = GetItem("ws:" + levelId.ToString() + "/");
		if (item != null)
		{
			onRead(item);
			return;
		}
		this.onRead = onRead;
		UGCQueryHandle_t handle = SteamUGC.CreateQueryUGCDetailsRequest(new PublishedFileId_t[1]
		{
			new PublishedFileId_t(levelId)
		}, 1u);
		OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
		SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(handle);
		OnSteamUGCQueryCompletedCallResult.Set(hAPICall);
	}

	private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
	{
		if (param.m_eResult != EResult.k_EResultOK)
		{
			onRead(null);
			onRead = null;
			return;
		}
		bool queryUGCResult = SteamUGC.GetQueryUGCResult(param.m_handle, 0u, out UGCDetails);
		OnUGCDownloadCallResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnDownloadThumbnail);
		SteamAPICall_t hAPICall = SteamRemoteStorage.UGCDownload(UGCDetails.m_hPreviewFile, 0u);
		OnUGCDownloadCallResult.Set(hAPICall);
	}

	private void OnDownloadThumbnail(RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
	{
		if (param.m_eResult != EResult.k_EResultOK)
		{
			onRead(null);
			onRead = null;
			return;
		}
		byte[] array = new byte[param.m_nSizeInBytes];
		SteamRemoteStorage.UGCRead(UGCDetails.m_hPreviewFile, array, array.Length, 0u, EUGCReadAction.k_EUGCRead_Close);
		onRead(new BuiltinLevelMetadata
		{
			folder = "none",
			workshopId = UGCDetails.m_nPublishedFileId.m_PublishedFileId,
			itemType = WorkshopItemType.Level,
			title = UGCDetails.m_rgchTitle,
			cachedThumbnailBytes = array
		});
		onRead = null;
	}

	public void LoadLevel(ulong levelId, Action<WorkshopLevelMetadata> onRead)
	{
		PublishedFileId_t publishedFileId_t = new PublishedFileId_t(levelId);
		uint itemState = SteamUGC.GetItemState(publishedFileId_t);
		Debug.Log("itemstate: " + itemState + " " + publishedFileId_t);
		if ((itemState & 8) != 0 || itemState == 0)
		{
			this.onRead = onRead;
			OnDownloadItemCallResult = Callback<DownloadItemResult_t>.Create(OnDownloadLevel);
			if (!SteamUGC.DownloadItem(publishedFileId_t, bHighPriority: true))
			{
				onRead(null);
				this.onRead = null;
			}
		}
		else
		{
			onRead(WorkshopItemMetadata.Load<WorkshopLevelMetadata>("ws:" + publishedFileId_t.m_PublishedFileId));
			this.onRead = null;
		}
	}

	private void OnDownloadLevel(DownloadItemResult_t param)
	{
		if (onRead != null)
		{
			if (param.m_eResult == EResult.k_EResultOK)
			{
				onRead(WorkshopItemMetadata.Load<WorkshopLevelMetadata>("ws:" + param.m_nPublishedFileId));
			}
			else
			{
				onRead(null);
			}
			onRead = null;
		}
	}
}
