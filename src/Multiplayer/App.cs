using HumanAPI;
using I2.Loc;
using Steamworks;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
	public class App : MonoBehaviour, IDependency
	{
		public const string kStartupScene = "Startup";

		public const string kCustomisationScene = "Customization";

		public static object stateLock = new object();

		private static AppSate _state = AppSate.Startup;

		public static App instance;

		private AssetBundle lobbyAssetbundle;

		private ulong previousLobbyID;

		private const float kCancelConnectDelay = 2f;

		private Action queueAfterLevelLoad;

		private ulong loadingLevel = 57005uL;

		private ulong loadedLevel = 57005uL;

		private uint loadedHash = 57005u;

		private ulong serverLoadedLevel = 57005uL;

		private uint serverLoadedHash = 57005u;

		public int startedCheckpoint;

		public static AppSate state
		{
			get
			{
				lock (stateLock)
				{
					return _state;
				}
			}
			set
			{
				if (NetGame.netlog)
				{
					UnityEngine.Debug.Log(value);
				}
				lock (stateLock)
				{
					_state = value;
				}
			}
		}

		public static bool isServer
		{
			get
			{
				lock (stateLock)
				{
					return _state >= AppSate.ServerHost && _state <= AppSate.ServerPlayLevel;
				}
			}
		}

		public static bool isClient
		{
			get
			{
				lock (stateLock)
				{
					return _state >= AppSate.ClientJoin && _state <= AppSate.ClientPlayLevel;
				}
			}
		}

		[Conditional("NEVER_SET_THIS_DEFINE_1234")]
		private void AssertState(int where = -1)
		{
			UnityEngine.Debug.LogErrorFormat("Wrong state {0} (site={1})", state, where);
		}

		[Conditional("NEVER_SET_THIS_DEFINE_1234")]
		private void WarnState(int where = -1)
		{
			UnityEngine.Debug.LogWarningFormat("Wrong state {0} (site={1})", state, where);
		}

		public void Awake()
		{
			instance = this;
		}

		public void Initialize()
		{
			if (state == AppSate.Startup)
			{
				instance = this;
				GameSave.Initialize();
				Dependencies.Initialize<StartupExperienceUI>();
				Dependencies.Initialize<NetGame>();
				Dependencies.Initialize<WorkshopRepository>();
				Dependencies.Initialize<RagdollTemplate>();
				Dependencies.Initialize<Game>();
				Dependencies.Initialize<GiftService>();
				Dependencies.OnInitialized(this);
				NetGame.instance.StartLocalGame();
				Options.Load();
				HumanAnalytics.instance.LogVersion(VersionDisplay.fullVersion);
			}
		}

		public void Update()
		{
		}

		public void OnRelayConnection(NetHost client)
		{
			NetChat.Print(string.Format(ScriptLocalization.MULTIPLAYER.Relayed, client.name));
		}

		public void BeginStartup()
		{
			if (state == AppSate.Startup)
			{
				object obj = NetGame.instance.transport.FetchLaunchInvitation();
				if (obj != null)
				{
					StartupExperienceController.instance.SkipStartupExperience(obj);
				}
				else
				{
					StartupExperienceController.instance.PlayStartupExperience();
				}
			}
		}

		public void StartupFinished()
		{
			lock (stateLock)
			{
				if (state == AppSate.Startup)
				{
					EnterMenu();
					MenuSystem.instance.ShowMainMenu(hideLogo: true);
				}
			}
		}

		public void BeginMenuDev()
		{
			lock (stateLock)
			{
				if (state == AppSate.Startup)
				{
					EnterMenu();
					StartupExperienceUI.instance.gameObject.SetActive(value: false);
				}
			}
		}

		public void BeginLevel()
		{
			lock (stateLock)
			{
				if (state == AppSate.Startup)
				{
					EnterMenu();
					StartupExperienceUI.instance.gameObject.SetActive(value: false);
					LaunchSinglePlayer((ulong)Game.instance.currentLevelNumber, Game.instance.currentLevelType, 0, 0);
				}
			}
		}

		private void EnterMenu()
		{
			lock (stateLock)
			{
				if (state != AppSate.Menu)
				{
					if (state == AppSate.ClientLobby || state == AppSate.ServerLobby)
					{
						ExitLobby();
					}
					if (state == AppSate.Customize || state == AppSate.PlayLevel || state == AppSate.ClientLobby || state == AppSate.ServerLobby)
					{
						Game.instance.HasSceneLoaded = false;
						SceneManager.LoadSceneAsync("Empty");
					}
					else if (state != 0)
					{
						Game.instance.HasSceneLoaded = false;
						SceneManager.LoadScene("Empty");
					}
					Game.instance.state = GameState.Inactive;
					state = AppSate.Menu;
				}
			}
		}

		private void EnterLobby(bool isServer, Action callback = null)
		{
			if (!RatingMenu.instance.ShowRatingMenu())
			{
				MenuSystem.instance.HideMenus();
			}
			StartCoroutine(EnterLobbyAsync(isServer, callback));
		}

		private IEnumerator EnterLobbyAsync(bool asServer, Action callback = null)
		{
			NetScope.ClearAllButPlayers();
			object obj = stateLock;
			Monitor.Enter(obj);
			try
			{
				state = ((!asServer) ? AppSate.ClientLoadLobby : AppSate.ServerLoadLobby);
				SuspendDeltasForLoad();
				Game.instance.HasSceneLoaded = false;
				string sceneName = null;
				if (Game.multiplayerLobbyLevel < 128)
				{
					sceneName = WorkshopRepository.GetLobbyFilename(Game.multiplayerLobbyLevel);
					goto IL_01fb;
				}
				bool loaded = false;
				WorkshopLevelMetadata workshopLevel = null;
				WorkshopRepository.instance.levelRepo.LoadLevel(Game.multiplayerLobbyLevel, delegate(WorkshopLevelMetadata l)
				{
					workshopLevel = l;
					loaded = true;
				});
				while (!loaded)
				{
					yield return null;
				}
				if (workshopLevel != null)
				{
					lobbyAssetbundle = FileTools.LoadBundle(workshopLevel.dataPath);
					string[] allScenePaths = lobbyAssetbundle.GetAllScenePaths();
					sceneName = Path.GetFileNameWithoutExtension(allScenePaths[0]);
					StopPlaytimeForItem(previousLobbyID);
					StartPlaytimeForItem(workshopLevel.workshopId);
					previousLobbyID = workshopLevel.workshopId;
					goto IL_01fb;
				}
				if (NetGame.isServer)
				{
					goto IL_01fb;
				}
				SubtitleManager.instance.ClearProgress();
				UnityEngine.Debug.Log("Level load failed.");
				instance.ServerFailedToLoad();
				SignalManager.EndReset();
				goto end_IL_0055;
				IL_01fb:
				if (string.IsNullOrEmpty(sceneName))
				{
					sceneName = WorkshopRepository.GetLobbyFilename(0uL);
					Game.multiplayerLobbyLevel = 0uL;
				}
				AsyncOperation loader = SceneManager.LoadSceneAsync(sceneName);
				if (loader != null)
				{
					while (!loader.isDone || !Game.instance.HasSceneLoaded)
					{
						yield return null;
					}
				}
				if (state != AppSate.ServerLoadLobby && state != AppSate.ClientLoadLobby)
				{
					UnityEngine.Debug.Log("Exiting wrong app state (" + state.ToString() + ")");
				}
				state = ((!asServer) ? AppSate.ClientLobby : AppSate.ServerLobby);
				ResumeDeltasAfterLoad();
				if (!RatingMenu.instance.ShowRatingMenu())
				{
					MenuSystem.instance.ShowMainMenu<MultiplayerLobbyMenu>();
				}
				Game.instance.state = GameState.Inactive;
				UpdateJoinable();
				callback?.Invoke();
				if (queueAfterLevelLoad != null)
				{
					Action action = queueAfterLevelLoad;
					queueAfterLevelLoad = null;
					if (NetGame.netlog)
					{
						UnityEngine.Debug.Log("Executing queue");
					}
					action();
				}
				if (lobbyAssetbundle != null)
				{
					lobbyAssetbundle.Unload(unloadAllLoadedObjects: false);
					lobbyAssetbundle = null;
				}
				Game.instance.FixAssetBundleImport(lobby: true);
				end_IL_0055:;
			}
			finally
			{
				
			}
		}

		private void ExitLobby(bool startingGame = false)
		{
			lock (stateLock)
			{
				UpdateJoinable(startingGame);
				MultiplayerLobbyController.Teardown();
			}
		}

		public void OnClientCountChanged()
		{
			UpdateJoinable();
		}

		private void UpdateJoinable(bool startingGame = false)
		{
			if (!NetGame.isServer)
			{
				return;
			}
			NetTransport transport = NetGame.instance.transport;
			if (NetGame.instance.players.Count < Options.lobbyMaxPlayers)
			{
				UnityEngine.Debug.Log("UpdateJoinable : hasSlots");
				if (state == AppSate.ServerLobby && !startingGame)
				{
					UnityEngine.Debug.Log("UpdateJoinable : in lobby");
					transport.SetJoinable(joinable: true, haveStarted: false);
				}
				else
				{
					UnityEngine.Debug.Log("UpdateJoinable : join in progress = " + Options.lobbyJoinInProgress.ToString());
					transport.SetJoinable(Options.lobbyJoinInProgress != 0, haveStarted: true);
				}
			}
			else
			{
				UnityEngine.Debug.Log("UpdateJoinable : noSlots");
				transport.SetJoinable(joinable: false, state != AppSate.ServerLobby || startingGame);
			}
		}

		public void HostGame(object sessionArgs = null)
		{
			lock (stateLock)
			{
				if (state == AppSate.Menu)
				{
					state = AppSate.ServerHost;
					Dialogs.ShowHostServerProgress(CancelHost);
					NetGame.instance.HostGame(sessionArgs);
				}
			}
		}

		public void OnHostGameSuccess()
		{
			lock (stateLock)
			{
				if (state == AppSate.ServerHost)
				{
					Dialogs.ShowLoadLevelProgress(57005uL);
					EnterLobby(isServer: true, delegate
					{
						NetChat.PrintHelp();
					});
				}
			}
		}

		public void OnHostGameFail(string error)
		{
			lock (stateLock)
			{
				if (state == AppSate.ServerHost)
				{
					Dialogs.HideProgress(null, 2);
					EnterMenu();
					if (error != null && error.Equals("!"))
					{
						MenuSystem.instance.ShowMenu<MainMenu>(hideOldMenu: true);
					}
					else
					{
						UnityEngine.Debug.LogError("OnHostGameFail Error = " + ((!string.IsNullOrEmpty(error)) ? error : "null"));
						Dialogs.CreateServerFailed(error, delegate
						{
							MenuSystem.instance.ShowMenu<MainMenu>();
						}, hideOldMenu: true);
					}
				}
			}
		}

		public void CancelHost()
		{
			lock (stateLock)
			{
				EnterMenu();
				MenuSystem.instance.ShowMainMenu(hideLogo: false, hideOldMenu: true);
				NetGame.instance.StopServer();
				Game.instance.state = GameState.Inactive;
			}
		}

		public void StopServer()
		{
			lock (stateLock)
			{
				if (state == AppSate.ServerLobby)
				{
					EnterMenu();
					if (MenuSystem.instance.activeMenu == null)
					{
						MenuSystem.instance.ShowMainMenu();
					}
					else
					{
						MenuSystem.instance.activeMenu.TransitionBack<MainMenu>();
					}
					NetGame.instance.StopServer();
					Game.instance.state = GameState.Inactive;
				}
			}
		}

		public void LeaveLobby()
		{
			lock (stateLock)
			{
				if (state == AppSate.ClientLobby)
				{
					EnterMenu();
					NetGame.instance.LeaveGame();
					if (MenuSystem.instance.activeMenu != null)
					{
						MenuSystem.instance.activeMenu.TransitionBack<MainMenu>();
					}
					else
					{
						MenuSystem.instance.ShowMenu<MainMenu>();
					}
					Game.instance.state = GameState.Inactive;
				}
			}
		}

		public void ChangeLobbyLevel(ulong level, WorkshopItemSource levelType)
		{
			lock (stateLock)
			{
				if (state == AppSate.ServerLobby)
				{
					NetGame.instance.ServerLoadLevel(level, levelType, start: false, 0u);
					if (MenuSystem.instance.activeMenu is MultiplayerLobbyMenu)
					{
						(MenuSystem.instance.activeMenu as MultiplayerLobbyMenu).RebindLevel();
					}
				}
			}
		}

		public void AcceptInvite(object server)
		{
			lock (stateLock)
			{
				if (state == AppSate.LoadLevel || state == AppSate.ClientLoadLevel || state == AppSate.ServerLoadLevel || state == AppSate.ClientLoadLobby || state == AppSate.ServerLoadLobby)
				{
					if (NetGame.netlog)
					{
						UnityEngine.Debug.Log("Queueing accept invite");
					}
					queueAfterLevelLoad = delegate
					{
						AcceptInvite(server);
					};
				}
				else
				{
					if (state == AppSate.PlayLevel || state == AppSate.ClientPlayLevel || state == AppSate.ServerPlayLevel || state == AppSate.ClientWaitServerLoad)
					{
						PauseLeave(instantLeave: true);
					}
					if (isClient)
					{
						LeaveLobby();
					}
					else if (isServer)
					{
						StopServer();
					}
					else if (state == AppSate.Customize)
					{
						LeaveCustomization();
					}
					if (server != null)
					{
						if (state == AppSate.Startup)
						{
							if (StartupExperienceController.instance != null)
							{
								StartupExperienceController.instance.LeaveGameStartupXP();
								StartupExperienceController.instance.DestroyStartupStuff();
							}
							if (StartupExperienceUI.instance != null)
							{
								if (StartupExperienceUI.instance.gameObject != null)
								{
									StartupExperienceUI.instance.gameObject.SetActive(value: false);
								}
								UnityEngine.Object.Destroy(StartupExperienceUI.instance.gameObject);
							}
							state = AppSate.Menu;
						}
						JoinGame(server);
					}
				}
			}
		}

		public void JoinGame(object server)
		{
			lock (stateLock)
			{
				if (state == AppSate.Menu)
				{
					state = AppSate.ClientJoin;
					loadingLevel = 57005uL;
					loadedLevel = 57005uL;
					serverLoadedLevel = 57005uL;
					queueAfterLevelLoad = null;
					Dialogs.ShowJoinGameProgress(CancelConnect);
					NetGame.instance.JoinGame(server);
				}
			}
		}

		public void OnConnectFail(string error)
		{
			lock (stateLock)
			{
				if (state == AppSate.ClientJoin)
				{
					EnterMenu();
					if (error != null && error.Equals("!"))
					{
						MenuSystem.instance.ShowMenu<MainMenu>(hideOldMenu: true);
					}
					else if (error != null && error.Equals("&"))
					{
						MenuSystem.instance.ShowMenu<MultiplayerSelectLobbyMenu>(hideOldMenu: true);
					}
					else
					{
						UnityEngine.Debug.LogError(error);
						Dialogs.ConnectionFailed(error, delegate
						{
							MenuSystem.instance.ShowMenu<MainMenu>();
						}, hideOldMenu: true);
					}
				}
			}
		}

		public void OnConnectFailVersion(string serverVersion)
		{
			lock (stateLock)
			{
				if (state == AppSate.ClientJoin)
				{
					EnterMenu();
					UnityEngine.Debug.LogError(serverVersion);
					Dialogs.ConnectionFailedVersion(serverVersion, delegate
					{
						MenuSystem.instance.ShowMenu<MainMenu>();
					}, hideOldMenu: true);
				}
			}
		}

		private void FinishCancelConnect()
		{
			MenuSystem.instance.ShowMainMenu(hideLogo: false, hideOldMenu: true);
		}

		private IEnumerator DelayedCancelConnect()
		{
			yield return new WaitForSeconds(2f);
			FinishCancelConnect();
			NetTransportSteam.RemoveOldPackets();
		}

		public void CancelConnect()
		{
			lock (stateLock)
			{
				if (state == AppSate.ClientJoin)
				{
					NetGame.instance.LeaveGame();
					EnterMenu();
					MenuSystem.instance.HideMenus();
					StartCoroutine(DelayedCancelConnect());
				}
			}
		}

		public void CancelConnectPhase2()
		{
			CancelConnect();
		}

		public void OnLostConnection(bool suppressMessage = false)
		{
			lock (stateLock)
			{
				if (state == AppSate.LoadLevel || state == AppSate.ClientLoadLevel || state == AppSate.ServerLoadLevel || state == AppSate.ClientLoadLobby || state == AppSate.ServerLoadLobby)
				{
					if (NetGame.netlog)
					{
						UnityEngine.Debug.Log("Queueing lostConnection");
					}
					queueAfterLevelLoad = delegate
					{
						OnLostConnection(suppressMessage);
					};
				}
				else
				{
					RatingMenu.instance.LevelOver();
					MultiplayerSelectLobbyMenu multiplayerSelectLobbyMenu = MenuSystem.instance.activeMenu as MultiplayerSelectLobbyMenu;
					if (multiplayerSelectLobbyMenu != null)
					{
						multiplayerSelectLobbyMenu.OnBack();
					}
					else
					{
						NetGame.instance.LeaveGame();
						if (state == AppSate.PlayLevel || state == AppSate.ClientPlayLevel || state == AppSate.ServerPlayLevel || state == AppSate.ClientWaitServerLoad)
						{
							ExitGame();
						}
						EnterMenu();
					}
					if (RatingMenu.instance.ShowRatingMenu())
					{
						RatingMenu.RatingDestination destination = RatingMenu.RatingDestination.kMainMenu;
						if (!suppressMessage)
						{
							destination = RatingMenu.RatingDestination.kLostConnection;
						}
						GotoRatingsMenu(destination, showLoading: false);
					}
					else
					{
						NetGame.isNetStarting = false;
						if (suppressMessage)
						{
							MenuSystem.instance.ShowMainMenu();
						}
						else
						{
							Dialogs.ConnectionLost(delegate
							{
								MenuSystem.instance.ShowMainMenu();
							});
						}
					}
				}
			}
		}

		public void KillGame()
		{
			NetGame.instance.LeaveGame();
			ExitGame();
			EnterMenu();
		}

		public void ServerFailedToLoad()
		{
			KillGame();
			Dialogs.FailedToLoad(delegate
			{
				MenuSystem.instance.ShowMainMenu();
			});
		}

		public void ServerKicked()
		{
			RatingMenu.instance.LevelOver();
			KillGame();
			if (RatingMenu.instance.ShowRatingMenu())
			{
				GotoRatingsMenu(RatingMenu.RatingDestination.kLostConnection, showLoading: false);
			}
			else
			{
				Dialogs.ConnectionKicked(delegate
				{
					MenuSystem.instance.ShowMainMenu();
				});
			}
		}

		public void ServerLostConnection(bool suppressMessage = false)
		{
			NetGame.isNetStarting = false;
			OnLostConnection(suppressMessage);
		}

		public void SuspendDeltasForLoad()
		{
			NetGame.instance.ignoreDeltasMask |= 2u;
		}

		public void ResumeDeltasAfterLoad()
		{
			NetGame.instance.ignoreDeltasMask &= 4294967293u;
			if (NetGame.isServer)
			{
				NetGame.currentLevelInstanceID = NetGame.nextLevelInstanceID;
			}
		}

		public void OnRequestLoadLevel(ulong level, WorkshopItemSource levelType, bool start, uint hash)
		{
			if (start && loadingLevel != level)
			{
				RatingMenu.instance.LoadInit();
			}
			if (NetGame.netlog)
			{
				UnityEngine.Debug.LogFormat("On RequestLoad {0} {1} {2} {3}", level, levelType, start, hash);
			}
			lock (stateLock)
			{
				if (state == AppSate.ClientLoadLevel || state == AppSate.ClientLoadLobby)
				{
					if (NetGame.netlog)
					{
						UnityEngine.Debug.Log("Queueing on request load");
					}
					queueAfterLevelLoad = delegate
					{
						OnRequestLoadLevel(level, levelType, start, hash);
					};
				}
				else
				{
					queueAfterLevelLoad = null;
					if (hash == 0)
					{
						serverLoadedLevel = 57005uL;
					}
					if (!start)
					{
						loadingLevel = (loadedLevel = 57005uL);
						loadedHash = 0u;
						queueAfterLevelLoad = null;
						if (state == AppSate.ClientLobby)
						{
							MultiplayerLobbyMenu multiplayerLobbyMenu = MenuSystem.instance.activeMenu as MultiplayerLobbyMenu;
							if (multiplayerLobbyMenu != null)
							{
								multiplayerLobbyMenu.RebindLevel();
							}
							state = AppSate.ClientLobby;
						}
						else if (isClient)
						{
							RatingMenu.instance.LevelOver();
							if (Game.instance.state == GameState.Paused || Game.instance.state == GameState.PlayingLevel)
							{
								ExitGame();
							}
							if (RatingMenu.instance.ShowRatingMenu())
							{
								GotoRatingsMenu(RatingMenu.RatingDestination.kMultiplayerLobby, showLoading: true);
							}
							else
							{
								MenuCameraEffects.FadeToBlack(0.02f);
								Dialogs.ShowLoadLevelProgress(57005uL);
							}
							EnterLobby(isServer: false, delegate
							{
								state = AppSate.ClientLobby;
							});
						}
					}
					else
					{
						if (loadingLevel != level)
						{
							if (state == AppSate.ClientLobby)
							{
								ExitLobby();
							}
							loadingLevel = level;
							LaunchGame(level, levelType, 0, 0, delegate
							{
								loadedLevel = level;
								loadedHash = Game.currentLevel.netHash;
								if (!LevelLoadedClient())
								{
									state = AppSate.ClientWaitServerLoad;
								}
							});
						}
						if (hash != 0)
						{
							serverLoadedLevel = level;
							serverLoadedHash = hash;
							LevelLoadedClient();
						}
					}
				}
			}
		}

		private bool LevelLoadedClient()
		{
			lock (stateLock)
			{
				if (serverLoadedLevel == loadedLevel)
				{
					Dialogs.HideProgress();
					MenuCameraEffects.FadeOut(1f);
					if (loadedHash == serverLoadedHash)
					{
						state = AppSate.ClientPlayLevel;
					}
					else
					{
						NetGame.instance.LeaveGame();
						ExitGame();
						EnterMenu();
						if (serverLoadedHash == 57005)
						{
							Dialogs.ConnectionFailed("LevelNotNetReady", delegate
							{
								MenuSystem.instance.ShowMainMenu();
							});
						}
						else
						{
							UnityEngine.Debug.LogError("Incompatible level. Server hash: " + serverLoadedHash + ". Client Hash: " + loadedHash);
							Dialogs.ConnectionFailed("IncompatibleLevel", delegate
							{
								MenuSystem.instance.ShowMainMenu();
							});
						}
						queueAfterLevelLoad = null;
					}
					return true;
				}
				return false;
			}
		}

		public void LaunchSinglePlayer(ulong level, WorkshopItemSource levelType, int checkpoint, int subObjectives)
		{
			startedCheckpoint = checkpoint;
			LaunchGame(level, levelType, checkpoint, subObjectives, delegate
			{
				MenuCameraEffects.FadeOut(1f);
				state = AppSate.PlayLevel;
			});
		}

		public void LaunchCustomLevel(string path, WorkshopItemSource levelType, int checkpoint, int subObjectives)
		{
			StartCoroutine(LaunchGame(path, 0uL, levelType, checkpoint, subObjectives, delegate
			{
				MenuCameraEffects.FadeOut(1f);
				state = AppSate.PlayLevel;
			}));
		}

		private void LaunchGame(ulong level, WorkshopItemSource levelType, int checkpoint, int subObjectives, Action onComplete)
		{
			StopPlaytimeForItem(previousLobbyID);
			previousLobbyID = 0uL;
			MenuSystem.instance.HideMenus();
			CheatCodes.cheatMode = false;
			if (levelType == WorkshopItemSource.BuiltIn && level == 0 && checkpoint == 0)
			{
				Game.instance.singleRun = true;
			}
			SuspendDeltasForLoad();
			StartCoroutine(LaunchGame(null, level, levelType, checkpoint, subObjectives, delegate
			{
				onComplete();
			}));
		}

		private IEnumerator LaunchGame(string levelPath, ulong level, WorkshopItemSource type, int checkpoint, int subObjectives, Action onComplete)
		{
			object obj = stateLock;
			Monitor.Enter(obj);
			try
			{
				if (state == AppSate.Menu || state == AppSate.PlayLevel || state == AppSate.ClientJoin || state == AppSate.ClientLobby || state == AppSate.ClientPlayLevel || state == AppSate.ClientWaitServerLoad || state == AppSate.ServerLobby || state != AppSate.ServerPlayLevel)
				{
				}
				bool ui = state == AppSate.Menu || state == AppSate.ServerLobby || state == AppSate.ClientLobby || state == AppSate.ClientJoin;
				if (ui)
				{
					MenuSystem.instance.FadeOutActive();
				}
				if (isServer || isClient)
				{
					MenuCameraEffects.FadeToBlack((!ui) ? 0.02f : 0.2f);
					Dialogs.ShowLoadLevelProgress(level);
				}
				if (isServer)
				{
					state = AppSate.ServerLoadLevel;
				}
				else if (isClient)
				{
					state = AppSate.ClientLoadLevel;
				}
				else
				{
					state = AppSate.LoadLevel;
				}
				queueAfterLevelLoad = null;
				if (ui)
				{
					yield return new WaitForSeconds(0.2f);
				}
				NetStream.DiscardPools();
				Game.instance.BeginLoadLevel(levelPath, level, checkpoint, subObjectives, delegate
				{
					lock (stateLock)
					{
						if (state == AppSate.LoadLevel || state == AppSate.ServerLoadLevel || state != AppSate.ClientLoadLevel)
						{
						}
						MenuSystem.instance.ExitMenus();
						NetStream.DiscardPools();
						ResumeDeltasAfterLoad();
						if (onComplete != null)
						{
							onComplete();
						}
						if (queueAfterLevelLoad != null)
						{
							Action action = queueAfterLevelLoad;
							queueAfterLevelLoad = null;
							if (NetGame.netlog)
							{
								UnityEngine.Debug.Log("Executing queue");
							}
							action();
						}
					}
				}, type);
			}
			finally
			{
				
			}
		}

		public static void StartPlaytimeLocalPlayers()
		{
			foreach (NetPlayer player in NetGame.instance.players)
			{
				if (player.isLocalPlayer && player.skin != null)
				{
					StartPlaytimeForItem(player.skin.workshopId);
				}
			}
		}

		public static void StopPlaytimeForItem(ulong item)
		{
			if (item != 0)
			{
				SteamUGC.StopPlaytimeTracking(new PublishedFileId_t[1]
				{
					new PublishedFileId_t(item)
				}, 1u);
			}
		}

		public static void StartPlaytimeForItem(ulong item)
		{
			if (item != 0)
			{
				SteamUGC.StartPlaytimeTracking(new PublishedFileId_t[1]
				{
					new PublishedFileId_t(item)
				}, 1u);
			}
		}

		private void ExitGame()
		{
			Game.instance.singleRun = false;
			SteamUGC.StopPlaytimeTrackingForAllItems();
			Game.instance.UnloadLevel();
		}

		public void StartGameServer(ulong level, WorkshopItemSource levelType)
		{
			startedCheckpoint = 0;
			lock (stateLock)
			{
				if (state == AppSate.ServerLobby)
				{
					ExitLobby(startingGame: true);
					NetGame.instance.ServerLoadLevel(level, levelType, start: true, 0u);
					LaunchGame(level, levelType, 0, 0, delegate
					{
						LevelLoadedServer(level, levelType, Game.currentLevel.netHash);
					});
				}
			}
		}

		public void NextLevelServer(ulong level, int checkpoint)
		{
			lock (stateLock)
			{
				if (state == AppSate.ServerPlayLevel)
				{
					NetGame.instance.ServerLoadLevel(level, WorkshopItemSource.BuiltIn, start: true, 0u);
					LaunchGame(level, WorkshopItemSource.BuiltIn, checkpoint, 0, delegate
					{
						LevelLoadedServer(level, WorkshopItemSource.BuiltIn, Game.currentLevel.netHash);
					});
				}
			}
		}

		public void LobbyLoadLevel(ulong level)
		{
			MenuCameraEffects.FadeToBlack(0.02f);
			Dialogs.ShowLoadLevelProgress(57005uL);
			Listener.instance.EndTransfromOverride();
			SceneManager.LoadScene("Empty");
			EnterLobby(NetGame.isServer);
			if (NetGame.isServer)
			{
				NetGame.instance.ServerLoadLobby(level, WorkshopItemSource.BuiltInLobbies, start: false, 0u);
			}
		}

		public void LevelLoadedServer(ulong level, WorkshopItemSource levelType, uint hash)
		{
			lock (stateLock)
			{
				if (state == AppSate.ServerLoadLevel)
				{
					Dialogs.HideProgress();
					MenuCameraEffects.FadeOut(1f);
					if (hash == 0 || hash == 57005)
					{
						NetGame.instance.StopServer();
						ExitGame();
						EnterMenu();
						UnityEngine.Debug.LogError("LevelNotNetReady");
						Dialogs.CreateServerFailed("LevelNotNetReady", delegate
						{
							MenuSystem.instance.ShowMenu<MainMenu>();
						});
					}
					else
					{
						state = AppSate.ServerPlayLevel;
						NetGame.instance.ServerLoadLevel(level, levelType, start: true, (hash != 0) ? hash : 57005u);
					}
				}
			}
		}

		public void StartNextLevel(ulong level, int checkpoint)
		{
			lock (stateLock)
			{
				if (state == AppSate.ServerPlayLevel)
				{
					NextLevelServer(level, checkpoint);
				}
				else
				{
					LaunchSinglePlayer(level, WorkshopItemSource.BuiltIn, checkpoint, 0);
				}
			}
		}

		private void GotoRatingsMenu(RatingMenu.RatingDestination destination, bool showLoading)
		{
			MenuCameraEffects.FadeInPauseMenu();
			RatingMenu.instance.SetDestination(destination);
			if (showLoading)
			{
				SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.LOADING, 1f, 1f);
			}
			MenuSystem.instance.ShowMainMenu<RatingMenu>();
		}

		public void PauseLeave(bool instantLeave = false)
		{
			lock (stateLock)
			{
				RatingMenu.instance.LevelOver();
				bool flag = RatingMenu.instance.ShowRatingMenu();
				bool flag2 = Game.instance.workshopLevel != null;
				ExitGame();
				if (state == AppSate.PlayLevel)
				{
					EnterMenu();
					PlayerManager.SetSingle();
					if (flag2)
					{
						MenuCameraEffects.instance.RemoveOverride();
						LevelSelectMenu2.instance.SetMultiplayerMode(inMultiplayer: false);
						LevelSelectMenu2.instance.ShowSubscribed();
						if (flag)
						{
							GotoRatingsMenu(RatingMenu.RatingDestination.kLevelSelectMenu, showLoading: false);
						}
						else
						{
							MenuSystem.instance.ShowMainMenu<LevelSelectMenu2>();
						}
					}
					else
					{
						MenuSystem.instance.ShowMainMenu();
					}
				}
				else if (state == AppSate.ServerPlayLevel)
				{
					if (!instantLeave)
					{
						if (flag)
						{
							GotoRatingsMenu(RatingMenu.RatingDestination.kMultiplayerLobby, showLoading: true);
						}
						else
						{
							MenuCameraEffects.FadeToBlack(0.02f);
							Dialogs.ShowLoadLevelProgress(57005uL);
						}
						EnterLobby(isServer: true);
						NetGame.instance.ServerLoadLevel(NetGame.instance.currentLevel, NetGame.instance.currentLevelType, start: false, 0u);
					}
					else
					{
						EnterMenu();
						MenuSystem.instance.ShowMainMenu();
						NetGame.instance.LeaveGame();
					}
				}
				else if (state == AppSate.ClientPlayLevel)
				{
					NetGame.instance.LeaveGame();
					EnterMenu();
					if (flag)
					{
						GotoRatingsMenu(RatingMenu.RatingDestination.kMainMenu, showLoading: false);
					}
					else
					{
						MenuSystem.instance.ShowMainMenu();
					}
				}
			}
		}

		public void EnterCustomization(Action controllerLoaded)
		{
			lock (stateLock)
			{
				state = AppSate.Customize;
				CustomizationController.onInitialized = controllerLoaded;
				SceneManager.LoadSceneAsync("Customization");
			}
		}

		internal void LeaveCustomization()
		{
			lock (stateLock)
			{
				if (MenuSystem.instance.activeMenu is CustomizationPaintMenu)
				{
					MenuSystem.instance.GetMenu<CustomizationPaintMenu>().paint.CancelStroke();
				}
				if (MenuSystem.instance.activeMenu is CustomizationWebcamMenu)
				{
					MenuSystem.instance.GetMenu<CustomizationWebcamMenu>().Teardown();
				}
				EnterMenu();
				if (CustomizationController.instance != null)
				{
					CustomizationController.instance.Teardown();
				}
			}
		}

		public void SetLobbyTitle(string lobbyTitle)
		{
			NetGame.instance.lobbyTitle = lobbyTitle;
			if (NetGame.isServer)
			{
				NetGame.instance.transport.UpdateLobbyTitle();
			}
		}

		public string GetLobbyTitle()
		{
			return NetGame.instance.lobbyTitle;
		}

		public void OnApplicationQuit()
		{
			lock (stateLock)
			{
				if (isServer)
				{
					NetGame.instance.StopServer();
				}
				else if (isClient)
				{
					NetGame.instance.LeaveGame();
				}
			}
		}
	}
}
