using I2.Loc;
using Steamworks;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class GameSave
{
	public struct GameSaveState
	{
		public string version;

		public int progressLevel;

		public int progressCheckpoint;

		public int lastSaveLevel;

		public int lastSaveCheckpoint;

		public ulong lastWorkshopSaveLevel;

		public int lastWorkshopSaveCheckpoint;

		public bool enableLevelSelect;

		public WorkshopItemSource lastLevelType;

		public int lastSaveSubObjectives;

		public int saveVersion;

		public bool seenLatestLevel;
	}

	private const string kDeprecatedStringVersion = "0.6.0";

	private const int kCurrentVersion = 1;

	public const WorkshopItemSource latestLevelType = WorkshopItemSource.EditorPick;

	public const int latestLevelID = 1;

	private const string progressFileName = "progress.txt";

	private const string prefNameGameStarted = "gameStarted";

	private const string prefNameprogressLevel = "progressLevel";

	private const string prefNameprogressCheckpoint = "progressCheckpoint";

	private const string prefNameLastSaveLevel = "lastSaveLevel";

	private const string prefNameLastSaveCheckpoint = "lastSaveCheckpoint";

	private const string prefNameLastLevelType = "lastLevelType";

	private const string prefNameLastSaveSubObjectives = "subObjectives";

	private const string prefNameSaveVersion = "saveVersion";

	private const string prefNameSeenLatestLevel = "seenLatestLevel";

	private static GameSaveState savedState = default(GameSaveState);

	private static void ReadSteam()
	{
		if (SteamManager.Initialized)
		{
			try
			{
				if (SteamRemoteStorage.FileExists("progress.txt"))
				{
					byte[] array = new byte[SteamRemoteStorage.GetFileSize("progress.txt")];
					int count = SteamRemoteStorage.FileRead("progress.txt", array, array.Length);
					string @string = Encoding.UTF8.GetString(array, 0, count);
					if (@string.Length > 0)
					{
						GameSaveState gameSaveState = savedState = JsonUtility.FromJson<GameSaveState>(@string);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	private static void WriteSteam()
	{
		if (SteamManager.Initialized)
		{
			try
			{
				string text = JsonUtility.ToJson(savedState);
				byte[] array = new byte[Encoding.UTF8.GetByteCount(text)];
				Encoding.UTF8.GetBytes(text, 0, text.Length, array, 0);
				bool flag = SteamRemoteStorage.FileWrite("progress.txt", array, array.Length);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	public static void Initialize()
	{
		ReadSteam();
		if (savedState.lastSaveCheckpoint > 0 || savedState.lastSaveLevel > 0 || savedState.lastSaveSubObjectives != 0)
		{
			savedState.enableLevelSelect = true;
		}
		if (savedState.saveVersion != 1)
		{
			if (savedState.saveVersion < 1)
			{
				savedState.seenLatestLevel = false;
			}
			savedState.saveVersion = 1;
		}
	}

	public static void CompleteGame(int levelsCompleted)
	{
		savedState.progressLevel = 100;
		savedState.progressCheckpoint = 0;
		savedState.lastSaveLevel = 12;
		savedState.lastSaveCheckpoint = 0;
		SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.SAVING, 1f, 1f);
		savedState.lastLevelType = WorkshopItemSource.BuiltIn;
		WriteSteam();
	}

	public static void PassCheckpointCampaign(uint levelNumber, int checkpoint, int subObjectives)
	{
		savedState.lastSaveLevel = (int)levelNumber;
		savedState.lastSaveCheckpoint = checkpoint;
		savedState.lastSaveSubObjectives = subObjectives;
		savedState.enableLevelSelect = (savedState.enableLevelSelect || levelNumber + checkpoint > 0);
		savedState.lastLevelType = WorkshopItemSource.BuiltIn;
		if (levelNumber > savedState.progressLevel || (levelNumber == savedState.progressLevel && checkpoint > savedState.progressCheckpoint))
		{
			savedState.progressLevel = (int)levelNumber;
			savedState.progressCheckpoint = checkpoint;
		}
		WriteSteam();
	}

	public static void PassCheckpointEditorPick(uint levelNumber, int checkpoint, int subObjectives)
	{
		savedState.lastSaveLevel = (int)levelNumber;
		savedState.lastSaveCheckpoint = checkpoint;
		savedState.lastSaveSubObjectives = subObjectives;
		savedState.lastLevelType = WorkshopItemSource.EditorPick;
		WriteSteam();
	}

	public static void PassCheckpointWorkshop(ulong workshopLevel, int checkpoint)
	{
		Game.instance.gameProgress.CustomLevelProgress(workshopLevel, checkpoint);
	}

	public static void GetWorkshopCheckpoint(out ulong workshopLevel, out int checkpoint)
	{
		workshopLevel = savedState.lastWorkshopSaveLevel;
		checkpoint = savedState.lastWorkshopSaveCheckpoint;
	}

	public static void ClearWorkshopCheckpoint()
	{
		savedState.lastWorkshopSaveLevel = 0uL;
		savedState.lastWorkshopSaveCheckpoint = 0;
		WriteSteam();
	}

	public static void StartFromBeginning()
	{
		savedState.version = "0.6.0";
		savedState.lastSaveLevel = (savedState.lastSaveCheckpoint = (savedState.lastSaveSubObjectives = 0));
		savedState.lastLevelType = WorkshopItemSource.BuiltIn;
		WriteSteam();
	}

	public static void StartNewGame()
	{
		savedState.version = "0.6.0";
		savedState.progressLevel = (savedState.progressCheckpoint = 0);
		savedState.lastSaveLevel = (savedState.lastSaveCheckpoint = (savedState.lastSaveSubObjectives = 0));
		savedState.lastLevelType = WorkshopItemSource.BuiltIn;
		WriteSteam();
	}

	public static void ExitLevel()
	{
	}

	public static void GetLastSave(out int levelNumber, out int checkpoint, out int subObjectives, out float timeSinceStart)
	{
		levelNumber = savedState.lastSaveLevel;
		checkpoint = savedState.lastSaveCheckpoint;
		subObjectives = savedState.lastSaveSubObjectives;
		timeSinceStart = 0f;
	}

	public static bool ProgressMoreOrEqual(int levelNumber, int checkpoint)
	{
		return savedState.progressLevel > levelNumber || (savedState.progressLevel == levelNumber && savedState.progressCheckpoint >= checkpoint);
	}

	public static void GetProgress(out int levelNumber, out int checkpoint)
	{
		levelNumber = savedState.progressLevel;
		checkpoint = savedState.progressCheckpoint;
	}

	public static WorkshopItemSource GetLastCheckpointLevelType()
	{
		return savedState.lastLevelType;
	}

	public static void SavePlayerTexture(int slot, Texture2D texture)
	{
		string path = Path.Combine(Application.persistentDataPath, "characterTexture" + slot + ".png");
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(path, bytes);
	}

	public static Texture2D LoadPlayerTexture(int slot)
	{
		try
		{
			string path = Path.Combine(Application.persistentDataPath, "characterTexture" + slot + ".png");
			byte[] array = File.ReadAllBytes(path);
			if (array != null)
			{
				Texture2D texture2D = new Texture2D(2048, 2048, TextureFormat.RGB24, mipmap: true, linear: true);
				texture2D.LoadImage(array);
				return texture2D;
			}
		}
		catch
		{
		}
		return null;
	}

	public static bool IsLevelSelectEnabled()
	{
		return savedState.enableLevelSelect;
	}

	public static bool HasSeenLatestLevel()
	{
		return savedState.seenLatestLevel;
	}

	public static void SeeLatestLevel()
	{
		savedState.seenLatestLevel = true;
		WriteSteam();
	}
}
