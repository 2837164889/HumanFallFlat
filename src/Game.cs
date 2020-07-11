using HumanAPI;
using I2.Loc;
using InControl;
using Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour, IGame, IDependency
{
	public StartupExperienceController startupXP;

	public static Game instance;

	public string[] levels;

	public string[] editorPickLevels;

	public int levelCount;

	public int currentLevelNumber = -1;

	public int currentCheckpointNumber;

	public int currentCheckpointSubObjectives;

	public List<int> currentSolvedCheckpoints = new List<int>();

	public WorkshopItemSource currentLevelType;

	public string editorLanguage = "english";

	public int editorStartLevel = 3;

	public int editorStartCheckpoint = 3;

	public Light defaultLight;

	public const int currentBuiltLevels = 13;

	public const int kMaxBuiltInLevels = 16;

	public const int kMaxBuiltInLobbies = 128;

	public static uint currentLevelID;

	public static Level currentLevel;

	public GameState state;

	public bool passedLevel;

	public NetPlayer playerPrefab;

	public Camera cameraPrefab;

	public Ragdoll ragdollPrefab;

	public Material skyboxMaterial;

	[NonSerialized]
	public GameProgress gameProgress;

	[NonSerialized]
	public static ulong multiplayerLobbyLevel;

	[NonSerialized]
	public bool singleRun;

	[NonSerialized]
	public bool HasSceneLoaded;

	public static Action<Human> OnDrowning;

	private AssetBundle bundle;

	public WorkshopLevelMetadata workshopLevel;

	public bool workshopLevelIsCustom;

	private Color skyColor;

	private const string kDefaultMixerIfNull = "Effects";

	private const int kInitialAudioSources = 200;

	public static bool GetKeyDown(KeyCode key)
	{
		return Input.GetKeyDown(key);
	}

	public static bool GetKeyUp(KeyCode key)
	{
		return Input.GetKeyUp(key);
	}

	public static bool GetKey(KeyCode key)
	{
		return Input.GetKey(key);
	}

	public bool IsWorkshopLevel()
	{
		WorkshopItemSource workshopItemSource = currentLevelType;
		if (workshopItemSource == WorkshopItemSource.Subscription || workshopItemSource == WorkshopItemSource.LocalWorkshop)
		{
			return true;
		}
		return false;
	}

	public void Initialize()
	{
		instance = this;
		Physics.autoSyncTransforms = false;
		Dependencies.Initialize<MenuCameraEffects>();
		Dependencies.OnInitialized(this);
		gameProgress = new GameProgress();
		defaultLight = GetComponentInChildren<Light>();
		state = GameState.Inactive;
		levelCount = levels.Length - 1;
	}

	private void Awake()
	{
		PostProcessLayer[] componentsInChildren = GetComponentsInChildren<PostProcessLayer>();
		PostProcessLayer[] array = componentsInChildren;
		foreach (PostProcessLayer postProcessLayer in array)
		{
			postProcessLayer.enabled = true;
		}
	}

	private void OnEnable()
	{
		Debug.Log("Game.OnEnable called");
		HasSceneLoaded = false;
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		HasSceneLoaded = true;
		if (NetGame.isServer && NetGame.currentLevelInstanceID != NetGame.nextLevelInstanceID)
		{
			NetGame.currentLevelInstanceID = 0;
		}
		Debug.Log("OnSceneLoaded: " + scene.name + "  mode=" + mode);
	}

	private void OnSceneUnloaded(Scene scene)
	{
		Debug.Log("OnSceneUnloaded: " + scene.name);
		if (NetGame.isServer && NetGame.currentLevelInstanceID != NetGame.nextLevelInstanceID)
		{
			NetGame.currentLevelInstanceID = 0;
		}
		GroundManager.ResetOnSceneUnload();
	}

	private void OnDisable()
	{
		Debug.Log("Game.OnDisable called");
		HasSceneLoaded = false;
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
	}

	public void Update()
	{
		if (!(MenuSystem.instance.activeMenu != null) && state == GameState.PlayingLevel && (!(MultiplayerLobbyController.instance != null) || !NetGame.isNetStarted) && Input.GetKeyDown(KeyCode.Escape) && MenuSystem.CanInvokeFromGame)
		{
			ShowPause();
		}
	}

	private void FixedUpdate()
	{
		if (!(MenuSystem.instance.activeMenu != null) && state == GameState.PlayingLevel && (!(MultiplayerLobbyController.instance != null) || !NetGame.isNetStarted))
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			if (activeDevice.MenuWasPressed && MenuSystem.CanInvokeFromGame)
			{
				ShowPause();
			}
		}
	}

	private void ShowPause()
	{
		if (MenuSystem.keyboardState == KeyboardState.None)
		{
			MenuSystem.instance.ShowPauseMenu();
			if (NetGame.isLocal)
			{
				Time.timeScale = 0f;
				state = GameState.Paused;
			}
		}
	}

	public void Resume()
	{
		Time.timeScale = 1f;
		state = GameState.PlayingLevel;
	}

	public void GameOver()
	{
		HumanAnalytics.instance.GameOver();
		App.instance.PauseLeave();
	}

	public void UnloadLevel()
	{
		StatsAndAchievements.Save();
		Resume();
		AfterUnload();
		MenuCameraEffects menuCameraEffects = MenuCameraEffects.instance;
		if ((bool)menuCameraEffects)
		{
			menuCameraEffects.ForceDisableOcclusion(forceDisableOcclusion: false);
		}
	}

	public void AfterUnload()
	{
		workshopLevel = null;
		workshopLevelIsCustom = false;
		state = GameState.Inactive;
		if (currentLevel != null)
		{
			DisableOnExit.ExitingLevel(currentLevel);
			defaultLight.gameObject.SetActive(value: true);
			currentLevel = null;
			StartCoroutine(UnloadBundle());
		}
		currentLevelNumber = -1;
		state = GameState.Inactive;
	}

	private IEnumerator UnloadBundle()
	{
		yield return null;
		if (bundle != null)
		{
			bundle.Unload(unloadAllLoadedObjects: true);
		}
	}

	public bool LevelLoaded(Level level)
	{
		currentLevel = level;
		return NetGame.isClient;
	}

	public void SolvePuzzle(int puzzle)
	{
		if (state == GameState.PlayingLevel && !currentSolvedCheckpoints.Contains(puzzle))
		{
			currentSolvedCheckpoints.Add(puzzle);
			Debug.Log("TODO: Save the puzzle progress");
		}
	}

	public bool IsCheckpointSolved(int puzzle)
	{
		return currentSolvedCheckpoints.Contains(puzzle);
	}

	public void EnterCheckpoint(int checkpoint, int subObjectives)
	{
		if (state != GameState.PlayingLevel)
		{
			return;
		}
		bool flag = false;
		if (currentCheckpointNumber < checkpoint)
		{
			flag = true;
		}
		else if (currentLevel.nonLinearCheckpoints && currentCheckpointNumber != checkpoint)
		{
			flag = true;
		}
		else if (currentCheckpointNumber == checkpoint && subObjectives != 0)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		Debug.Log("Passed " + checkpoint + ", subobjectives: " + subObjectives);
		int num = currentCheckpointNumber;
		int num2 = currentCheckpointSubObjectives;
		if (currentCheckpointNumber != checkpoint)
		{
			currentCheckpointSubObjectives = 0;
		}
		if (subObjectives != 0)
		{
			currentCheckpointSubObjectives |= 1 << subObjectives - 1;
		}
		currentCheckpointNumber = checkpoint;
		if (workshopLevel == null && currentLevelNumber != -1)
		{
			if (NetGame.isLocal)
			{
				WorkshopItemSource workshopItemSource = currentLevelType;
				if (workshopItemSource == WorkshopItemSource.EditorPick)
				{
					GameSave.PassCheckpointEditorPick((uint)currentLevelNumber, checkpoint, currentCheckpointSubObjectives);
				}
				else
				{
					GameSave.PassCheckpointCampaign((uint)currentLevelNumber, checkpoint, currentCheckpointSubObjectives);
				}
			}
			if (num != currentCheckpointNumber || num2 != currentCheckpointSubObjectives)
			{
				SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.SAVING, 1f, 1f);
			}
		}
		else
		{
			if (NetGame.isLocal)
			{
				GameSave.PassCheckpointWorkshop(workshopLevel.hash, checkpoint);
			}
			SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.SAVING, 1f, 1f);
		}
		if (NetGame.isServer)
		{
			NetSceneManager.EnterCheckpoint(checkpoint, subObjectives);
		}
	}

	public void EnterPassZone()
	{
		if (state == GameState.PlayingLevel && (!NetGame.isServer || Options.lobbyLockLevel != 1))
		{
			passedLevel = true;
		}
	}

	public void Fall(HumanBase humanBase, bool drown = false)
	{
		Human human = humanBase as Human;
		if (ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		if (passedLevel && ((currentLevelNumber >= 0 && currentLevelNumber < levels.Length) || workshopLevel != null || currentLevelType == WorkshopItemSource.EditorPick))
		{
			if (ReplayRecorder.isRecording)
			{
				ReplayRecorder.Stop();
			}
			else if (workshopLevel != null)
			{
				passedLevel = false;
				PlayerManager.SetSingle();
				App.instance.PauseLeave();
				GameSave.PassCheckpointWorkshop(workshopLevel.hash, 0);
			}
			else if (currentLevelType == WorkshopItemSource.EditorPick)
			{
				passedLevel = false;
				PlayerManager.SetSingle();
				App.instance.PauseLeave();
				if (NetGame.isLocal)
				{
					GameSave.StartFromBeginning();
				}
				for (int i = 0; i < Human.all.Count; i++)
				{
					Human.all[i].ReleaseGrab();
				}
			}
			else
			{
				StatsAndAchievements.PassLevel(levels[currentLevelNumber], human);
				for (int j = 0; j < Human.all.Count; j++)
				{
					Human.all[j].ReleaseGrab();
				}
				StartCoroutine(PassLevel());
			}
		}
		else
		{
			if (drown)
			{
				StatsAndAchievements.IncreaseDrownCount(human);
			}
			else
			{
				StatsAndAchievements.IncreaseFallCount(human);
			}
			Respawn(human, Vector3.zero);
			CheckpointRespawned(currentCheckpointNumber);
		}
	}

	public void Drown(Human human)
	{
		if (OnDrowning != null)
		{
			OnDrowning(human);
		}
		Fall(human, drown: true);
	}

	private void ResetAllPLayers()
	{
		for (int i = 0; i < Human.all.Count; i++)
		{
			ResetPlayer(Human.all[i]);
		}
	}

	public void RespawnAllPlayers(NetHost host)
	{
		for (int i = 0; i < host.players.Count; i++)
		{
			Respawn(host.players[i].human, Vector3.left * (i % 3) * 2f + Vector3.back * (i / 3) * 2f);
		}
	}

	public void RespawnAllPlayers()
	{
		for (int i = 0; i < Human.all.Count; i++)
		{
			Respawn(Human.all[i], Vector3.left * (i % 3) * 2f + Vector3.back * (i / 3) * 2f);
		}
	}

	private void ResetPlayer(Human human)
	{
		GrabManager.Release(human.gameObject);
		human.ReleaseGrab();
		human.Reset();
	}

	public void Respawn(Human human, Vector3 offset)
	{
		if (!(currentLevel == null))
		{
			passedLevel = false;
			Transform checkpoint = currentLevel.GetCheckpoint(currentCheckpointNumber);
			ResetPlayer(human);
			human.SpawnAt(checkpoint, offset);
		}
	}

	private bool GameIsCompleted()
	{
		if (currentLevelNumber == levelCount - 1)
		{
			return true;
		}
		if (currentLevelNumber < levelCount - 1)
		{
			return false;
		}
		if (!DLC.instance.SupportsDLC())
		{
			return false;
		}
		int num = currentLevelNumber + 1;
		int num2 = levelCount - num;
		for (int i = 0; i < num2; i++)
		{
			if (DLC.instance.LevelIsAvailable(num + i))
			{
				return false;
			}
		}
		return true;
	}

	private int GetNextLevel(int currentLevel)
	{
		currentLevel++;
		if (!DLC.instance.SupportsDLC())
		{
			return currentLevel;
		}
		for (int i = 0; i < 0; i++)
		{
			if (DLC.instance.LevelIsAvailable(currentLevel))
			{
				return currentLevel;
			}
			currentLevel++;
		}
		return currentLevel;
	}

	public IEnumerator PassLevel()
	{
		currentLevel.CompleteLevel();
		if (NetGame.isLocal)
		{
			GameSave.PassCheckpointCampaign((uint)currentLevelNumber, 0, 0);
		}
		if (GameIsCompleted())
		{
			if (singleRun)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_SINGLE_RUN);
			}
			if (NetGame.isLocal)
			{
				GameSave.CompleteGame(levelCount);
			}
			yield return null;
			App.instance.StartNextLevel((uint)levelCount, 0);
		}
		else
		{
			int nextLevel = GetNextLevel(currentLevelNumber);
			if (NetGame.isLocal)
			{
				GameSave.PassCheckpointCampaign((uint)nextLevel, 0, 0);
			}
			yield return null;
			App.instance.StartNextLevel((uint)nextLevel, 0);
		}
		StatsAndAchievements.Save();
	}

	private void FixupLoadedBundle(Scene scene)
	{
		if (!currentLevel.keepSkybox)
		{
			RenderSettings.skybox = skyboxMaterial;
			RenderSettings.ambientMode = AmbientMode.Flat;
			RenderSettings.ambientLight = skyColor;
		}
		BundleRepository.RebindScene(scene);
	}

	public void ReloadBundle()
	{
		StartCoroutine(ReloadBundleCoroutine());
	}

	private IEnumerator ReloadBundleCoroutine()
	{
		SignalManager.BeginReset();
		if (workshopLevel != null)
		{
			MenuCameraEffects.instance.RemoveOverride();
			workshopLevel.Reload();
		}
		Time.timeScale = 0f;
		SceneManager.LoadScene("Empty");
		yield return null;
		yield return null;
		yield return null;
		bundle = FileTools.LoadBundle(workshopLevel.dataPath);
		string[] scenePath = bundle.GetAllScenePaths();
		SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scenePath[0]));
		yield return null;
		SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.LOADING, 1f, 1f);
		yield return null;
		FixupLoadedBundle(SceneManager.GetActiveScene());
		bundle.Unload(unloadAllLoadedObjects: false);
		currentLevel.BeginLevel();
		ResetAllPLayers();
		Time.timeScale = 1f;
		SignalManager.EndReset();
		FixAssetBundleImport();
	}

	public void BeginLoadLevel(string levelId, ulong levelNumber, int checkpointNumber, int checkpointSubObjectives, Action onComplete, WorkshopItemSource type)
	{
		passedLevel = false;
		StartCoroutine(LoadLevel(levelId, levelNumber, checkpointNumber, checkpointSubObjectives, onComplete, type));
	}

	private void FixAssetBundleAudioMixerGroups(bool lobby)
	{
		if (workshopLevel == null && currentLevelType != WorkshopItemSource.EditorPick && (!lobby || multiplayerLobbyLevel <= 16))
		{
			return;
		}
		Scene activeScene = SceneManager.GetActiveScene();
		AudioSource[] array = UnityEngine.Object.FindObjectsOfType<AudioSource>();
		GameAudio gameAudio = UnityEngine.Object.FindObjectOfType<GameAudio>();
		if (gameAudio == null)
		{
			return;
		}
		AudioMixer mainMixer = gameAudio.mainMixer;
		if (mainMixer == null)
		{
			return;
		}
		AudioMixerGroup[] array2 = mainMixer.FindMatchingGroups("Effects");
		AudioSource[] array3 = array;
		foreach (AudioSource audioSource in array3)
		{
			if (audioSource.outputAudioMixerGroup != null)
			{
				AudioMixerGroup[] array4 = mainMixer.FindMatchingGroups(audioSource.outputAudioMixerGroup.name);
				if (array4.Length > 0)
				{
					audioSource.outputAudioMixerGroup = array4[0];
				}
			}
			else
			{
				audioSource.outputAudioMixerGroup = array2[0];
			}
		}
	}

	private void FixAssetBundleShaders(bool lobby)
	{
	}

	public void FixAssetBundleImport(bool lobby = false)
	{
		FixAssetBundleAudioMixerGroups(lobby);
		FixAssetBundleShaders(lobby);
	}

	private IEnumerator LoadLevel(string levelId, ulong levelNumber, int checkpointNumber, int checkpointSubObjectives, Action onComplete, WorkshopItemSource levelType)
	{
		bool localLevel = false;
		NetScope.ClearAllButPlayers();
		BeforeLoad();
		skyColor = RenderSettings.ambientLight;
		skyboxMaterial = RenderSettings.skybox;
		state = GameState.LoadingLevel;
		bool isBundle = !string.IsNullOrEmpty(levelId) || (levelNumber > 16 && levelNumber != ulong.MaxValue);
		if (isBundle)
		{
			if (string.IsNullOrEmpty(levelId))
			{
				bool loaded2 = false;
				WorkshopRepository.instance.levelRepo.GetLevel(levelNumber, levelType, delegate(WorkshopLevelMetadata l)
				{
					workshopLevel = l;
					loaded2 = true;
				});
				while (!loaded2)
				{
					yield return null;
				}
			}
			else
			{
				localLevel = levelId.StartsWith("lvl:");
				workshopLevel = WorkshopRepository.instance.levelRepo.GetItem(levelId);
			}
			RatingMenu.instance.LoadInit();
			if (!localLevel && workshopLevel != null)
			{
				App.StartPlaytimeForItem(workshopLevel.workshopId);
				RatingMenu.instance.QueryRatingStatus(workshopLevel.workshopId);
			}
		}
		App.StartPlaytimeLocalPlayers();
		if (currentLevelNumber != (int)levelNumber)
		{
			Level oldLevel = currentLevel;
			SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.LOADING);
			Application.backgroundLoadingPriority = ThreadPriority.Low;
			string sceneName = string.Empty;
			currentLevelType = levelType;
			switch (levelType)
			{
			case WorkshopItemSource.BuiltIn:
				sceneName = levels[levelNumber];
				break;
			case WorkshopItemSource.EditorPick:
				Debug.Log("Loading editor pick level");
				sceneName = editorPickLevels[(uint)levelNumber];
				break;
			}
			Debug.Log("scename = " + sceneName);
			if (!isBundle)
			{
				if (string.IsNullOrEmpty(sceneName))
				{
					sceneName = levels[levelNumber];
				}
			}
			else
			{
				if (!localLevel && workshopLevel != null)
				{
					bool loaded = false;
					WorkshopRepository.instance.levelRepo.LoadLevel(workshopLevel.workshopId, delegate(WorkshopLevelMetadata l)
					{
						workshopLevel = l;
						loaded = true;
					});
					while (!loaded)
					{
						yield return null;
					}
				}
				bundle = null;
				if (workshopLevel != null)
				{
					bundle = FileTools.LoadBundle(workshopLevel.dataPath);
				}
				if (bundle == null)
				{
					SubtitleManager.instance.ClearProgress();
					Debug.Log("Level load failed.");
					App.instance.ServerFailedToLoad();
					SignalManager.EndReset();
					yield break;
				}
				string[] scenePath = bundle.GetAllScenePaths();
				if (string.IsNullOrEmpty(sceneName))
				{
					sceneName = Path.GetFileNameWithoutExtension(scenePath[0]);
				}
			}
			instance.HasSceneLoaded = false;
			SwitchAssetBundle.LoadingCurrentScene loader2 = SwitchAssetBundle.LoadSceneAsync(sceneName);
			if (loader2 == null)
			{
				AsyncOperation sceneLoader2 = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
				while (!sceneLoader2.isDone || !instance.HasSceneLoaded)
				{
					yield return null;
				}
				sceneLoader2 = null;
				SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.LOADING, 1f, 1f);
				currentLevelNumber = (int)levelNumber;
			}
			else
			{
				while (!loader2.isDone || !instance.HasSceneLoaded)
				{
					yield return null;
				}
				loader2 = null;
				SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.LOADING, 1f, 1f);
				currentLevelNumber = (int)levelNumber;
			}
		}
		if (currentLevel == null)
		{
			SignalManager.EndReset();
			onComplete?.Invoke();
			yield break;
		}
		if (isBundle)
		{
			FixupLoadedBundle(SceneManager.GetActiveScene());
			bundle.Unload(unloadAllLoadedObjects: false);
		}
		if (currentLevelNumber >= 0 && !isBundle)
		{
			HumanAnalytics.instance.LoadLevel(levels[currentLevelNumber], (int)levelNumber, checkpointNumber, 0f);
		}
		FixAssetBundleImport();
		AfterLoad(checkpointNumber, checkpointSubObjectives);
		if (NetGame.isLocal)
		{
			if (levelType == WorkshopItemSource.BuiltIn && currentLevelNumber < levelCount - 1)
			{
				GameSave.PassCheckpointCampaign((uint)currentLevelNumber, checkpointNumber, checkpointSubObjectives);
			}
			if (levelType == WorkshopItemSource.EditorPick)
			{
				GameSave.PassCheckpointEditorPick((uint)currentLevelNumber, checkpointNumber, checkpointSubObjectives);
			}
		}
		onComplete?.Invoke();
	}

	public void BeforeLoad()
	{
		SignalManager.BeginReset();
	}

	public void AfterLoad(int checkpointNumber, int subobjectives)
	{
		MenuCameraEffects menuCameraEffects = MenuCameraEffects.instance;
		if ((bool)menuCameraEffects && currentLevelNumber >= 0 && currentLevelNumber < levels.Length)
		{
			string text = levels[currentLevelNumber];
			if (text != null && text == "Halloween")
			{
				menuCameraEffects.ForceDisableOcclusion(forceDisableOcclusion: true);
			}
			else
			{
				menuCameraEffects.ForceDisableOcclusion(forceDisableOcclusion: false);
			}
		}
		state = GameState.PlayingLevel;
		defaultLight.gameObject.SetActive(value: false);
		currentCheckpointNumber = Mathf.Min(checkpointNumber, currentLevel.checkpoints.Length - 1);
		currentCheckpointSubObjectives = subobjectives;
		currentLevel.BeginLevel();
		if (currentLevel.prerespawn != null)
		{
			currentLevel.prerespawn(currentCheckpointNumber, startingLevel: true);
		}
		RespawnAllPlayers();
		currentLevel.Reset(currentCheckpointNumber, currentCheckpointSubObjectives);
		CheckpointLoaded(checkpointNumber);
		SignalManager.EndReset();
		currentLevel.PostEndReset(currentCheckpointNumber);
	}

	public void CheckpointLoaded(int checkpointNumber)
	{
		if (checkpointNumber > 0 && currentLevel != null && checkpointNumber < currentLevel.checkpoints.Length && currentLevel.checkpoints[checkpointNumber] != null)
		{
			Checkpoint component = currentLevel.checkpoints[checkpointNumber].GetComponent<Checkpoint>();
			if (component != null)
			{
				component.LoadHere();
			}
		}
	}

	private void CheckpointRespawned(int checkpointNumber)
	{
		if (!(currentLevel == null) && currentLevel.checkpoints != null && checkpointNumber >= 0 && checkpointNumber < currentLevel.checkpoints.Length && !(currentLevel.checkpoints[checkpointNumber] == null) && Human.all.Count <= 1)
		{
			Checkpoint component = currentLevel.checkpoints[checkpointNumber].GetComponent<Checkpoint>();
			if (component != null)
			{
				component.RespawnHere();
			}
		}
	}

	public void RestartCheckpoint()
	{
		RestartCheckpoint(currentCheckpointNumber, currentCheckpointSubObjectives);
	}

	public void RestartCheckpoint(int checkpointNumber, int subObjectives)
	{
		if (!currentLevel.respawnLocked)
		{
			currentCheckpointNumber = Mathf.Min(checkpointNumber, currentLevel.checkpoints.Length - 1);
			currentCheckpointSubObjectives = subObjectives;
			SignalManager.BeginReset();
			if (currentLevel.prerespawn != null)
			{
				currentLevel.prerespawn(checkpointNumber, startingLevel: false);
			}
			RespawnAllPlayers();
			currentLevel.Reset(currentCheckpointNumber, currentCheckpointSubObjectives);
			if (NetGame.isServer)
			{
				NetSceneManager.ResetLevel(currentCheckpointNumber, currentCheckpointSubObjectives);
			}
			currentLevel.BeginLevel();
			CheckpointLoaded(currentCheckpointNumber);
			SignalManager.EndReset();
			currentLevel.PostEndReset(currentCheckpointNumber);
		}
	}

	public void RestartLevel(bool reset = true)
	{
		if (currentLevel.respawnLocked)
		{
			return;
		}
		SignalManager.BeginReset();
		currentCheckpointNumber = 0;
		currentCheckpointSubObjectives = 0;
		if (currentLevel.prerespawn != null)
		{
			currentLevel.prerespawn(-1, startingLevel: false);
		}
		RespawnAllPlayers();
		if (reset)
		{
			currentLevel.Reset(currentCheckpointNumber, currentCheckpointSubObjectives);
			if (NetGame.isServer)
			{
				NetSceneManager.ResetLevel(currentCheckpointNumber, currentCheckpointSubObjectives);
			}
		}
		currentLevel.BeginLevel();
		SignalManager.EndReset();
		if (reset)
		{
			currentLevel.PostEndReset(currentCheckpointNumber);
		}
	}
}
