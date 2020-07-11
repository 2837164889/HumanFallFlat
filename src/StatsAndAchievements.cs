using Multiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsAndAchievements : MonoBehaviour
{
	private const bool DebugMultiplier = false;

	private static StatsAndAchievements instance;

	public static float travelM;

	public static float climbM;

	public static float carryM;

	public static float shipM;

	public static float driveM;

	public static float dumpsterM;

	private static int savedTravelM;

	private static int savedClimbM;

	private static int savedCarryM;

	private static int savedShipM;

	private static int savedDriveM;

	private static int savedDumpsterM;

	public static int fall;

	public static int jump;

	public static int drown;

	private static bool m_bBlockClimb = false;

	private static List<Achievement> unlocked = new List<Achievement>();

	private CGameID m_GameID;

	private bool m_bRequestedStats;

	private bool m_bStatsValid;

	private bool m_bStoreStats;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;

	protected Callback<UserStatsStored_t> m_UserStatsStored;

	protected Callback<UserAchievementStored_t> m_UserAchievementStored;

	private static bool warningOnce = true;

	public static bool BlockClimb
	{
		get
		{
			return m_bBlockClimb;
		}
		set
		{
			m_bBlockClimb = value;
		}
	}

	private static void SyncStat(Stat stat, ref float field, bool write)
	{
		if (write)
		{
			SteamUserStats.SetStat(SteamAchievementMap.StatToSteam(stat), (int)field);
			return;
		}
		SteamUserStats.GetStat(SteamAchievementMap.StatToSteam(stat), out int pData);
		field = pData;
	}

	private static void SyncStat(Stat stat, ref int field, bool write)
	{
		if (write)
		{
			SteamUserStats.SetStat(SteamAchievementMap.StatToSteam(stat), field);
		}
		else
		{
			SteamUserStats.GetStat(SteamAchievementMap.StatToSteam(stat), out field);
		}
	}

	private static void SyncStats(bool write)
	{
		if (SteamManager.Initialized)
		{
			SyncStat(Stat.STAT_TRAVEL_M, ref travelM, write);
			SyncStat(Stat.STAT_FALL, ref fall, write);
			SyncStat(Stat.STAT_JUMP, ref jump, write);
			SyncStat(Stat.STAT_CLIMB_M, ref climbM, write);
			SyncStat(Stat.STAT_CARRY_M, ref carryM, write);
			SyncStat(Stat.STAT_DROWN, ref drown, write);
			SyncStat(Stat.STAT_SHIP_M, ref shipM, write);
			SyncStat(Stat.STAT_DRIVE_M, ref driveM, write);
			SyncStat(Stat.STAT_DUMPSTER_M, ref dumpsterM, write);
			savedTravelM = (int)travelM;
			savedClimbM = (int)climbM;
			savedCarryM = (int)carryM;
			savedShipM = (int)shipM;
			savedDriveM = (int)driveM;
			savedDumpsterM = (int)dumpsterM;
		}
	}

	private void OnEnable()
	{
		instance = this;
		SteamAchievementMap.ValidateMaps();
		if (SteamManager.Initialized)
		{
			m_GameID = new CGameID(SteamUtils.GetAppID());
			m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
			m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
			m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
			m_bRequestedStats = false;
			m_bStatsValid = false;
		}
	}

	private void Update()
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		if (!m_bRequestedStats)
		{
			if (!SteamManager.Initialized)
			{
				m_bRequestedStats = true;
				return;
			}
			bool flag = m_bRequestedStats = SteamUserStats.RequestCurrentStats();
		}
		if (m_bStatsValid && m_bStoreStats)
		{
			SyncStats(write: true);
			bool flag2 = SteamUserStats.StoreStats();
			m_bStoreStats = !flag2;
		}
	}

	private void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		if (!SteamManager.Initialized || (ulong)m_GameID != pCallback.m_nGameID)
		{
			return;
		}
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			m_bStatsValid = true;
			unlocked.Clear();
			for (int i = 0; i < SteamAchievementMap.achievementList.Count; i++)
			{
				Achievement achievement = SteamAchievementMap.achievementList[i];
				if (SteamUserStats.GetAchievement(SteamAchievementMap.AchievementToSteam(achievement), out bool pbAchieved) && pbAchieved)
				{
					unlocked.Add(achievement);
				}
			}
			SyncStats(write: false);
		}
		else
		{
			Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
		}
	}

	private void OnUserStatsStored(UserStatsStored_t pCallback)
	{
		if ((ulong)m_GameID == pCallback.m_nGameID && pCallback.m_eResult != EResult.k_EResultOK)
		{
			if (pCallback.m_eResult == EResult.k_EResultInvalidParam)
			{
				Debug.Log("StoreStats - some failed to validate");
				UserStatsReceived_t pCallback2 = default(UserStatsReceived_t);
				pCallback2.m_eResult = EResult.k_EResultOK;
				pCallback2.m_nGameID = (ulong)m_GameID;
				OnUserStatsReceived(pCallback2);
			}
			else
			{
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	private void OnAchievementStored(UserAchievementStored_t pCallback)
	{
		if ((ulong)m_GameID == pCallback.m_nGameID)
		{
			if (pCallback.m_nMaxProgress == 0)
			{
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
			}
			else
			{
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
			}
		}
	}

	internal static void ResetAchievements()
	{
		unlocked.Clear();
		if (SteamManager.Initialized)
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
			SteamUserStats.RequestCurrentStats();
			Debug.Log("Steam reset");
		}
	}

	public static void UnlockAchievement(Achievement achievement, bool incAchievement = false, int localPlayerIndex = -1)
	{
		if (CheatCodes.cheatMode)
		{
			return;
		}
		if (achievement <= Achievement.ACH_SINGLE_RUN || achievement >= Achievement.ACH_INTRO_STATUE_HEAD)
		{
			if (NetGame.isServer)
			{
				NetGame.instance.NotifyUnlockAchievementServer(null, achievement);
			}
			if (NetGame.isClient)
			{
				return;
			}
		}
		UnlockAchievementInternal(achievement, incAchievement, localPlayerIndex);
	}

	private static void UnlockAchievementInternal(Achievement achievement, bool incAchievement, int localPlayerIndex = -1)
	{
		Debug.Log("Unlocking achievement: " + achievement.ToString());
		if (!CheatCodes.cheatMode && SteamManager.Initialized && !unlocked.Contains(achievement))
		{
			unlocked.Add(achievement);
			SteamUserStats.SetAchievement(SteamAchievementMap.AchievementToSteam(achievement));
			if (!incAchievement)
			{
				Save();
			}
		}
	}

	public static void OnNotifyUnlockAchievement(Achievement achievement, float amount)
	{
		switch (achievement)
		{
		case Achievement.ACH_TRAVEL_1KM:
			AddTravel(null, amount);
			break;
		case Achievement.ACH_FALL_1:
			IncreaseFallCount(null);
			break;
		case Achievement.ACH_JUMP_1000:
			IncreaseJumpCount(null);
			break;
		case Achievement.ACH_CLIMB_100M:
			AddClimb(null, amount);
			break;
		case Achievement.ACH_CARRY_1000M:
			AddCarry(null, amount);
			break;
		case Achievement.ACH_DROWN_10:
			IncreaseDrownCount(null);
			break;
		case Achievement.ACH_SHIP_1000M:
			AddShip(amount);
			break;
		case Achievement.ACH_DRIVE_1000M:
			AddDrive(amount);
			break;
		case Achievement.ACH_DUMPSTER_50M:
			AddDumpster(amount);
			break;
		case Achievement.ACH_SINGLE_RUN:
			if (Game.instance.singleRun)
			{
				UnlockAchievementInternal(Achievement.ACH_SINGLE_RUN, incAchievement: false);
			}
			break;
		case Achievement.ACH_TRAVEL_10KM:
		case Achievement.ACH_TRAVEL_100KM:
		case Achievement.ACH_FALL_1000:
			UnlockAchievementInternal(achievement, incAchievement: true);
			break;
		default:
			UnlockAchievementInternal(achievement, incAchievement: false);
			break;
		}
	}

	public static int GetLocalPlayerIndex(Human human)
	{
		return -1;
	}

	public static bool IsStatsPlayer(int localPlayerIndex)
	{
		return true;
	}

	public static void IncreaseFallCount(Human human)
	{
		if (NetGame.isServer && human != null && human.player != null && !human.player.isLocalPlayer)
		{
			if (IsStatsPlayer((int)human.player.localCoopIndex))
			{
				NetGame.instance.NotifyUnlockAchievementServer(human.player, Achievement.ACH_FALL_1);
			}
			return;
		}
		int localPlayerIndex = GetLocalPlayerIndex(human);
		if (IsStatsPlayer(localPlayerIndex))
		{
			fall++;
			if (fall >= 1)
			{
				UnlockAchievement(Achievement.ACH_FALL_1, incAchievement: false, localPlayerIndex);
			}
			Save(triggerSave: false);
		}
	}

	public static void IncreaseJumpCount(Human human)
	{
		if (NetGame.isServer && human != null && human.player != null && !human.player.isLocalPlayer)
		{
			if (IsStatsPlayer((int)human.player.localCoopIndex))
			{
				NetGame.instance.NotifyUnlockAchievementServer(human.player, Achievement.ACH_JUMP_1000);
			}
			return;
		}
		int localPlayerIndex = GetLocalPlayerIndex(human);
		if (IsStatsPlayer(localPlayerIndex))
		{
			jump++;
			if (jump % 100 == 0)
			{
				Save(triggerSave: false);
			}
		}
	}

	public static void IncreaseDrownCount(Human human)
	{
		if (NetGame.isServer && human != null && human.player != null && !human.player.isLocalPlayer)
		{
			if (IsStatsPlayer((int)human.player.localCoopIndex))
			{
				NetGame.instance.NotifyUnlockAchievementServer(human.player, Achievement.ACH_DROWN_10);
			}
			return;
		}
		int localPlayerIndex = GetLocalPlayerIndex(human);
		if (IsStatsPlayer(localPlayerIndex))
		{
			drown++;
			Save(triggerSave: false);
		}
	}

	public static void AddTravel(Human human, float m)
	{
		if (NetGame.isServer && human != null && human.player != null && !human.player.isLocalPlayer)
		{
			if (IsStatsPlayer((int)human.player.localCoopIndex))
			{
				NetGame.instance.NotifyUnlockAchievementServer(human.player, Achievement.ACH_TRAVEL_1KM, m);
			}
			return;
		}
		int localPlayerIndex = GetLocalPlayerIndex(human);
		if (IsStatsPlayer(localPlayerIndex))
		{
			travelM += m * 1f;
			if ((int)travelM / 100 > savedTravelM / 100)
			{
				Save(triggerSave: false);
			}
		}
	}

	public static void AddClimb(Human human, float m)
	{
		if (BlockClimb)
		{
			return;
		}
		if ((NetGame.isServer || NetGame.isLocal) && human != null && human.player != null && !human.player.isLocalPlayer)
		{
			if (IsStatsPlayer((int)human.player.localCoopIndex))
			{
				NetGame.instance.NotifyUnlockAchievementServer(human.player, Achievement.ACH_CLIMB_100M, m);
			}
			return;
		}
		int localPlayerIndex = GetLocalPlayerIndex(human);
		if (IsStatsPlayer(localPlayerIndex))
		{
			climbM += m * 1f;
			if ((int)climbM / 10 > savedClimbM / 10)
			{
				Save(triggerSave: false);
			}
		}
	}

	public static void AddCarry(Human human, float m)
	{
		if ((NetGame.isServer || NetGame.isLocal) && human != null && human.player != null && !human.player.isLocalPlayer)
		{
			if (IsStatsPlayer((int)human.player.localCoopIndex))
			{
				NetGame.instance.NotifyUnlockAchievementServer(human.player, Achievement.ACH_CARRY_1000M, m);
			}
			return;
		}
		int localPlayerIndex = GetLocalPlayerIndex(human);
		if (IsStatsPlayer(localPlayerIndex))
		{
			carryM += m * 1f;
			if ((int)carryM / 20 > savedCarryM / 20)
			{
				Save(triggerSave: false);
			}
		}
	}

	public static void AddShip(float m)
	{
		int num = -1;
		if (NetGame.isServer)
		{
			NetGame.instance.NotifyUnlockAchievementServer(null, Achievement.ACH_SHIP_1000M, m);
		}
		shipM += m * 1f;
		if ((int)shipM / 50 > savedShipM / 50)
		{
			Save(triggerSave: false);
		}
	}

	public static void AddDrive(float m)
	{
		int num = -1;
		if (NetGame.isServer)
		{
			NetGame.instance.NotifyUnlockAchievementServer(null, Achievement.ACH_DRIVE_1000M, m);
		}
		driveM += m * 1f;
		if ((int)driveM / 50 > savedDriveM / 50)
		{
			Save(triggerSave: false);
		}
	}

	public static void AddDumpster(float m)
	{
		int num = -1;
		if (NetGame.isServer)
		{
			NetGame.instance.NotifyUnlockAchievementServer(null, Achievement.ACH_DUMPSTER_50M, m);
		}
		dumpsterM += m * 1f;
		if ((int)dumpsterM / 5 > savedDumpsterM / 5)
		{
			Save(triggerSave: false);
		}
	}

	public static void Save(bool triggerSave = true)
	{
		instance.m_bStoreStats = true;
	}

	internal static void PassLevel(string currentLevelName, Human human)
	{
		switch (currentLevelName)
		{
		case "River":
			break;
		case "Ice":
		case "Ice_merged":
			break;
		case "Intro":
			UnlockAchievement(Achievement.ACH_LVL_INTRO);
			break;
		case "Push":
			UnlockAchievement(Achievement.ACH_LVL_PUSH);
			break;
		case "Carry":
			UnlockAchievement(Achievement.ACH_LVL_CARRY);
			break;
		case "Climb":
			UnlockAchievement(Achievement.ACH_LVL_CLIMB);
			break;
		case "Break":
			UnlockAchievement(Achievement.ACH_LVL_BREAK);
			break;
		case "Siege":
			UnlockAchievement(Achievement.ACH_LVL_SIEGE);
			break;
		case "Power":
			UnlockAchievement(Achievement.ACH_LVL_POWER);
			break;
		case "Aztec":
			UnlockAchievement(Achievement.ACH_LVL_AZTEC);
			break;
		case "Halloween":
			if (PlanksNoThanksAchievement.AchievementValid)
			{
				UnlockAchievement(Achievement.ACH_HALLOWEEN_PLANKS_NO_THANKS);
			}
			UnlockAchievement(Achievement.ACH_LVL_HALLOWEEN);
			break;
		case "Steam":
		case "Steam_merged":
			UnlockAchievement(Achievement.ACH_LVL_STEAM);
			break;
		case "Intro_Reprise":
			UnlockAchievement(Achievement.ACH_LVL_ICE);
			break;
		default:
			throw new Exception("Unknown level to award achievement " + currentLevelName);
		}
	}
}
