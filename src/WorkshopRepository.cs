using HumanAPI;
using I2.Loc;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopRepository : MonoBehaviour, IDependency
{
	public const string kFileIDLevel = "lvl";

	public const string kFileIDLevelColon = "lvl:";

	public const string kBuiltinLobbyPath = "builtinlobby:";

	public WorkshopRagdollModel[] builtinModels;

	public static WorkshopRepository instance;

	private Dictionary<WorkshopItemType, ModelPartRepository> modelRepos = new Dictionary<WorkshopItemType, ModelPartRepository>
	{
		{
			WorkshopItemType.ModelFull,
			new ModelPartRepository()
		},
		{
			WorkshopItemType.ModelHead,
			new ModelPartRepository()
		},
		{
			WorkshopItemType.ModelLowerBody,
			new ModelPartRepository()
		},
		{
			WorkshopItemType.ModelUpperBody,
			new ModelPartRepository()
		}
	};

	public LevelRepository levelRepo = new LevelRepository();

	public PresetRepository presetRepo = new PresetRepository();

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

	private UGCQueryHandle_t refreshResults = default(UGCQueryHandle_t);

	private List<WorkshopItemMetadata> levelsToRefresh = new List<WorkshopItemMetadata>();

	private DLCSystem dlc = new DLCSystem();

	private static readonly string[] multiplayerLobbyNames = new string[3]
	{
		"WorkshopLobby",
		"Lobby",
		"Xmas"
	};

	private const string kLobbyTitleMagic = "\u0011\u0012";

	private const int kLobbyTitleIDIndex = 2;

	private const int kLobbyTitleIDBase = 48;

	public ModelPartRepository GetPartRepository(WorkshopItemType type)
	{
		return modelRepos[type];
	}

	public static WorkshopLevelMetadata GetLevelMetaDataFromID(ulong levelID)
	{
		string path = "ws:" + levelID;
		string path2 = FileTools.Combine(path, "metadata.json");
		string text = FileTools.ReadAllText(path2);
		if (text == null)
		{
			return null;
		}
		return JsonUtility.FromJson<WorkshopLevelMetadata>(text);
	}

	private void ReadFolder(WorkshopItemSource source, string folder)
	{
		WorkshopItemMetadata workshopItemMetadata = WorkshopItemMetadata.Load(folder);
		if (workshopItemMetadata == null)
		{
			return;
		}
		switch (workshopItemMetadata.itemType)
		{
		case WorkshopItemType.Level:
			break;
		case WorkshopItemType.Levels:
			if (source == WorkshopItemSource.Subscription || source == WorkshopItemSource.LocalWorkshop)
			{
				levelRepo.AddItem(source, workshopItemMetadata);
			}
			break;
		case WorkshopItemType.Lobbies:
			if (source == WorkshopItemSource.SubscriptionLobbies || source == WorkshopItemSource.LocalWorkshop)
			{
				levelRepo.AddItem(source, workshopItemMetadata);
			}
			break;
		case WorkshopItemType.RagdollPreset:
			presetRepo.AddItem(source, workshopItemMetadata);
			break;
		default:
			modelRepos[workshopItemMetadata.itemType].AddItem(source, workshopItemMetadata);
			break;
		}
	}

	private void Clear(WorkshopItemSource source)
	{
		foreach (ModelPartRepository value in modelRepos.Values)
		{
			value.Clear(source);
		}
		levelRepo.Clear(source);
		presetRepo.Clear(source);
	}

	public void ReloadLocalModels()
	{
		modelRepos[WorkshopItemType.ModelFull].Clear(WorkshopItemSource.LocalWorkshop);
		modelRepos[WorkshopItemType.ModelHead].Clear(WorkshopItemSource.LocalWorkshop);
		modelRepos[WorkshopItemType.ModelUpperBody].Clear(WorkshopItemSource.LocalWorkshop);
		modelRepos[WorkshopItemType.ModelLowerBody].Clear(WorkshopItemSource.LocalWorkshop);
		string[] array = FileTools.ListDirectories("mdl:", ignoreMissingFile: true);
		for (int i = 0; i < array.Length; i++)
		{
			ReadFolder(WorkshopItemSource.LocalWorkshop, array[i]);
		}
	}

	public void ReloadLocalLevels()
	{
		levelRepo.Clear(WorkshopItemSource.LocalWorkshop);
		string[] array = FileTools.ListDirectories("lvl:");
		for (int i = 0; i < array.Length; i++)
		{
			ReadFolder(WorkshopItemSource.LocalWorkshop, array[i]);
		}
	}

	private bool CheckValidLevel(ref PublishedFileId_t[] subscriptions, ulong level, bool lobby)
	{
		int num = (!lobby) ? 16 : 128;
		if (level < (ulong)num)
		{
			return true;
		}
		PublishedFileId_t[] array = subscriptions;
		for (int i = 0; i < array.Length; i++)
		{
			PublishedFileId_t publishedFileId_t = array[i];
			if (publishedFileId_t.m_PublishedFileId == level)
			{
				return true;
			}
		}
		return false;
	}

	private void CheckStoredLevelAndLobbySubscriptions(ref PublishedFileId_t[] subscriptions)
	{
		ulong multiplayerLobbyLevelStore = Options.multiplayerLobbyLevelStore;
		ulong level = (ulong)Options.lobbyLevel;
		if (!CheckValidLevel(ref subscriptions, level, lobby: false))
		{
			Options.lobbyLevel = 0;
		}
		if (!CheckValidLevel(ref subscriptions, multiplayerLobbyLevelStore, lobby: true))
		{
			Game.multiplayerLobbyLevel = 0uL;
			Options.multiplayerLobbyLevelStore = 0uL;
		}
	}

	private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
	{
		if (param.m_eResult == EResult.k_EResultOK)
		{
			refreshResults = param.m_handle;
		}
		OnSteamUGCQueryCompletedCallResult = null;
	}

	private int CalcBatchSize(int maxLevels, int currentLevel, int maxBatch)
	{
		int num = maxLevels - currentLevel;
		if (num >= maxBatch)
		{
			return maxBatch;
		}
		return num;
	}

	private IEnumerator RefreshTitleDescription()
	{
		int levelsCount = levelsToRefresh.Count;
		if (levelsCount != 0)
		{
			int startCount = 0;
			int batchCount = CalcBatchSize(levelsCount, startCount, 50);
			List<PublishedFileId_t> publishFields = new List<PublishedFileId_t>(50);
			while (batchCount > 0)
			{
				publishFields.Clear();
				for (int i = 0; i < batchCount; i++)
				{
					publishFields.Add(new PublishedFileId_t(levelsToRefresh[i + startCount].workshopId));
				}
				UGCQueryHandle_t m_UGCQueryHandle = SteamUGC.CreateQueryUGCDetailsRequest(publishFields.ToArray(), (uint)publishFields.Count);
				SteamUGC.SetReturnLongDescription(m_UGCQueryHandle, bReturnLongDescription: true);
				OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
				SteamAPICall_t h = SteamUGC.SendQueryUGCRequest(m_UGCQueryHandle);
				OnSteamUGCQueryCompletedCallResult.Set(h);
				while (OnSteamUGCQueryCompletedCallResult != null)
				{
					yield return null;
				}
				for (int j = 0; j < batchCount; j++)
				{
					if (SteamUGC.GetQueryUGCResult(refreshResults, (uint)j, out SteamUGCDetails_t UGCDetails))
					{
						bool flag = false;
						WorkshopItemMetadata workshopItemMetadata = levelsToRefresh[j + startCount];
						if (!workshopItemMetadata.title.Equals(UGCDetails.m_rgchTitle))
						{
							workshopItemMetadata.title = UGCDetails.m_rgchTitle;
							flag = true;
						}
						if (!workshopItemMetadata.description.Equals(UGCDetails.m_rgchDescription))
						{
							workshopItemMetadata.description = UGCDetails.m_rgchDescription;
							flag = true;
						}
						if (flag)
						{
							workshopItemMetadata.Save(workshopItemMetadata.folder);
						}
					}
				}
				SteamUGC.ReleaseQueryUGCRequest(refreshResults);
				startCount += batchCount;
				batchCount = CalcBatchSize(levelsCount, startCount, 50);
			}
		}
		levelsToRefresh.Clear();
	}

	private void RefreshSubscriptions()
	{
		ReloadSubscriptions(isLobby: true);
		List<WorkshopLevelMetadata> list = levelRepo.BySource(WorkshopItemSource.SubscriptionLobbies);
		foreach (WorkshopLevelMetadata item in list)
		{
			levelsToRefresh.Add(item);
		}
		List<RagdollPresetMetadata> list2 = presetRepo.BySource(WorkshopItemSource.SubscriptionLobbies);
		foreach (RagdollPresetMetadata item2 in list2)
		{
			levelsToRefresh.Add(item2);
		}
		ReloadSubscriptions();
		list = levelRepo.BySource(WorkshopItemSource.Subscription);
		foreach (WorkshopLevelMetadata item3 in list)
		{
			levelsToRefresh.Add(item3);
		}
		StartCoroutine(RefreshTitleDescription());
	}

	public bool ReloadSubscriptions(bool isLobby = false, bool onlyLevels = false)
	{
		bool result = true;
		Clear(WorkshopItemSource.Subscription);
		Clear(WorkshopItemSource.SubscriptionLobbies);
		if (onlyLevels)
		{
			Clear(WorkshopItemSource.BuiltInLobbies);
		}
		try
		{
			int numSubscribedItems = (int)SteamUGC.GetNumSubscribedItems();
			PublishedFileId_t[] subscriptions = new PublishedFileId_t[numSubscribedItems];
			PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(subscriptions, (uint)numSubscribedItems);
			int num = 0;
			PublishedFileId_t[] array2 = subscriptions;
			foreach (PublishedFileId_t publishedFileId_t in array2)
			{
				uint itemState = SteamUGC.GetItemState(publishedFileId_t);
				if ((itemState & 4) != 0)
				{
					array[num] = publishedFileId_t;
					num++;
				}
				else
				{
					result = false;
				}
			}
			CheckStoredLevelAndLobbySubscriptions(ref subscriptions);
			for (int j = 0; j < num; j++)
			{
				ReadFolder((!isLobby) ? WorkshopItemSource.Subscription : WorkshopItemSource.SubscriptionLobbies, "ws:" + array[j].ToString() + "/");
			}
			return result;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return result;
		}
	}

	public void ReloadLocalPresets()
	{
		presetRepo.Clear(WorkshopItemSource.LocalWorkshop);
		string[] array = FileTools.ListDirectories("pr:Presets");
		for (int i = 0; i < array.Length; i++)
		{
			ReadFolder(WorkshopItemSource.LocalWorkshop, array[i]);
		}
		presetRepo.LoadSkins();
	}

	public static AssetBundle LoadBundle(WorkshopItemMetadata wsMeta)
	{
		string path = FileTools.Combine(wsMeta.folder, "data");
		return FileTools.LoadBundle(path);
	}

	public void Initialize()
	{
		instance = this;
		dlc.Init();
		LoadBuiltinModels();
		LoadBuiltinLevels();
		ReloadLocalModels();
		ReloadLocalPresets();
		RefreshSubscriptions();
		presetRepo.MigrateLegacySkin();
		Dependencies.OnInitialized(this);
	}

	private void LoadBuiltinModels()
	{
		for (int i = 0; i < builtinModels.Length; i++)
		{
			if (builtinModels[i] != null)
			{
				WorkshopItemMetadata meta = builtinModels[i].meta;
				modelRepos[meta.itemType].AddItem(WorkshopItemSource.BuiltIn, meta);
			}
		}
		presetRepo.LoadSkins();
	}

	public static string GetLobbyFilename(ulong id)
	{
		return multiplayerLobbyNames[id];
	}

	public static string GetLobbyName(ulong id)
	{
		return ScriptLocalization.Get("LOBBY/" + multiplayerLobbyNames[id]);
	}

	public void LoadBuiltinLevels(bool requestLobbies = false)
	{
		if (requestLobbies)
		{
			for (int i = 0; i < multiplayerLobbyNames.Length; i++)
			{
				AddLobby((ulong)i, multiplayerLobbyNames[i]);
			}
			return;
		}
		AddLevel(0uL, "Intro");
		AddLevel(1uL, "Train");
		AddLevel(2uL, "Carry");
		AddLevel(3uL, "Climb");
		AddLevel(4uL, "Break");
		AddLevel(5uL, "Siege");
		AddLevel(6uL, "Water");
		AddLevel(7uL, "Power");
		AddLevel(8uL, "Aztec");
		AddLevel(9uL, "Halloween");
		AddLevel(10uL, "Steam");
		AddLevel(11uL, "Ice");
	}

	public void LoadEditorPickLevels(bool requestLobbies = false)
	{
		if (requestLobbies)
		{
			for (int i = 0; i < multiplayerLobbyNames.Length; i++)
			{
				AddLobby((ulong)i, multiplayerLobbyNames[i]);
			}
		}
		else
		{
			AddEditorPickLevel(0uL, "Thermal");
			AddEditorPickLevel(1uL, "Factory");
		}
	}

	private void AddLevel(ulong number, string name)
	{
		levelRepo.AddItem(WorkshopItemSource.BuiltIn, new BuiltinLevelMetadata
		{
			folder = "builtin:" + number,
			workshopId = number,
			levelType = WorkshopItemSource.BuiltIn,
			itemType = WorkshopItemType.Level,
			internalName = name,
			title = ScriptLocalization.Get("LEVEL/" + name),
			_thumbPath = "res:LevelImages/" + name
		});
	}

	private void AddLobby(ulong number, string name)
	{
		levelRepo.AddItem(WorkshopItemSource.BuiltInLobbies, new BuiltinLevelMetadata
		{
			folder = "builtinlobby:" + number,
			workshopId = number,
			itemType = WorkshopItemType.Lobbies,
			internalName = name,
			title = GetLobbyName(number),
			_thumbPath = "res:LobbyImages/" + name
		});
	}

	private void AddEditorPickLevel(ulong number, string name)
	{
		levelRepo.AddItem(WorkshopItemSource.EditorPick, new BuiltinLevelMetadata
		{
			folder = "editorpick:" + number,
			workshopId = number,
			levelType = WorkshopItemSource.EditorPick,
			itemType = WorkshopItemType.Level,
			internalName = name,
			title = ScriptLocalization.Get("LEVEL/" + name),
			_thumbPath = "res:LevelImages/" + name
		});
	}

	public void SetLobbyTitle(ulong lobbyID)
	{
		if (lobbyID >= 128)
		{
			List<WorkshopLevelMetadata> list = instance.levelRepo.BySource(WorkshopItemSource.SubscriptionLobbies);
			foreach (WorkshopLevelMetadata item in list)
			{
				if (item.workshopId == lobbyID)
				{
					App.instance.SetLobbyTitle(item.title);
					return;
				}
			}
			WorkshopLevelMetadata levelMetaDataFromID = GetLevelMetaDataFromID(lobbyID);
			if (levelMetaDataFromID != null)
			{
				App.instance.SetLobbyTitle(levelMetaDataFromID.title);
				return;
			}
			Game.multiplayerLobbyLevel = 0uL;
			lobbyID = 0uL;
		}
		string lobbyTitle = string.Empty;
		if (lobbyID < 128)
		{
			lobbyTitle = "\u0011\u0012" + (char)(lobbyID + 48);
		}
		App.instance.SetLobbyTitle(lobbyTitle);
	}

	public static string GetLobbyTitle(string lobbyTitle)
	{
		if (lobbyTitle.Length == "\u0011\u0012".Length + 1 && lobbyTitle.StartsWith("\u0011\u0012"))
		{
			int num = lobbyTitle[2];
			lobbyTitle = GetLobbyName((ulong)(num - 48));
		}
		return lobbyTitle;
	}
}
