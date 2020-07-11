using System;
using System.Collections.Generic;
using UnityEngine;

public static class SteamAchievementMap
{
	private static Dictionary<Stat, string> statsMapInv = new Dictionary<Stat, string>();

	private static Dictionary<string, Stat> statsMap = new Dictionary<string, Stat>
	{
		{
			"Inc000MetresTraveled",
			Stat.STAT_TRAVEL_M
		},
		{
			"Inc001Respawns",
			Stat.STAT_FALL
		},
		{
			"Inc002Jumps",
			Stat.STAT_JUMP
		},
		{
			"Inc003MetresClimbed",
			Stat.STAT_CLIMB_M
		},
		{
			"Inc004MetresCarried",
			Stat.STAT_CARRY_M
		},
		{
			"Inc005Drownings",
			Stat.STAT_DROWN
		},
		{
			"Inc006MetresBoat",
			Stat.STAT_SHIP_M
		},
		{
			"Inc007MetresGroundVehicle",
			Stat.STAT_DRIVE_M
		},
		{
			"Inc008MetresDumpster",
			Stat.STAT_DUMPSTER_M
		}
	};

	private static Dictionary<Achievement, string> achievementMapInv = new Dictionary<Achievement, string>();

	private static Dictionary<string, Achievement> achievementMap = new Dictionary<string, Achievement>
	{
		{
			"Achievement000",
			Achievement.ACH_LVL_INTRO
		},
		{
			"Achievement001",
			Achievement.ACH_LVL_PUSH
		},
		{
			"Achievement002",
			Achievement.ACH_LVL_CARRY
		},
		{
			"Achievement003",
			Achievement.ACH_LVL_CLIMB
		},
		{
			"Achievement004",
			Achievement.ACH_LVL_BREAK
		},
		{
			"Achievement005",
			Achievement.ACH_LVL_SIEGE
		},
		{
			"Achievement006",
			Achievement.ACH_LVL_RIVER_FEET
		},
		{
			"Achievement007",
			Achievement.ACH_LVL_RIVER_HEAD
		},
		{
			"Achievement008",
			Achievement.ACH_LVL_POWER
		},
		{
			"Achievement1008",
			Achievement.ACH_LVL_AZTEC
		},
		{
			"Achievement1009",
			Achievement.ACH_LVL_HALLOWEEN
		},
		{
			"Achievement1010",
			Achievement.ACH_LVL_STEAM
		},
		{
			"Achievement1011",
			Achievement.ACH_LVL_ICE
		},
		{
			"Achievement009",
			Achievement.ACH_SINGLE_RUN
		},
		{
			"Achievement010",
			Achievement.ACH_FALL_1
		},
		{
			"Achievement011",
			Achievement.ACH_INTRO_STATUE_HEAD
		},
		{
			"Achievement012",
			Achievement.ACH_INTRO_JUMP_GAP
		},
		{
			"Achievement013",
			Achievement.ACH_PUSH_CLEAN_DEBRIS
		},
		{
			"Achievement014",
			Achievement.ACH_PUSH_BENCH_ALIGN
		},
		{
			"Achievement015",
			Achievement.ACH_CARRY_JAM_DOOR
		},
		{
			"Achievement016",
			Achievement.ACH_CARRY_BIG_STACK
		},
		{
			"Achievement017",
			Achievement.ACH_CLIMB_ROPE
		},
		{
			"Achievement018",
			Achievement.ACH_CLIMB_SPEAKERS
		},
		{
			"Achievement019",
			Achievement.ACH_CLIMB_GEMS
		},
		{
			"Achievement020",
			Achievement.ACH_BREAK_WINDOW_SHORTCUT
		},
		{
			"Achievement021",
			Achievement.ACH_BREAK_SURPRISE
		},
		{
			"Achievement022",
			Achievement.ACH_BREAK_BARE_HANDS
		},
		{
			"Achievement023",
			Achievement.ACH_SIEGE_HUMAN_CANNON
		},
		{
			"Achievement024",
			Achievement.ACH_SIEGE_BELL
		},
		{
			"Achievement025",
			Achievement.ACH_SIEGE_ZIPLINE
		},
		{
			"Achievement026",
			Achievement.ACH_SIEGE_ASSASIN
		},
		{
			"Achievement027",
			Achievement.ACH_WATER_ROW_BOAT
		},
		{
			"Achievement028",
			Achievement.ACH_WATER_REVERSE_GEAR
		},
		{
			"Achievement029",
			Achievement.ACH_WATER_LIGHTHOUSE
		},
		{
			"Achievement030",
			Achievement.ACH_WATER_ALMOST_DROWN
		},
		{
			"Achievement031",
			Achievement.ACH_WATER_SURF
		},
		{
			"Achievement032",
			Achievement.ACH_POWER_SHORT_CIRCUIT
		},
		{
			"Achievement033",
			Achievement.ACH_POWER_3VOLTS
		},
		{
			"Achievement034",
			Achievement.ACH_POWER_COAL_DELIVER
		},
		{
			"Achievement035",
			Achievement.ACH_POWER_STATUE_BATTERY
		},
		{
			"Achievement1111",
			Achievement.ACH_AZTEC_OVERLOOK
		},
		{
			"Achievement1112",
			Achievement.ACH_AZTEC_INDIANA
		},
		{
			"Achievement1113",
			Achievement.ACH_AZTEC_CLOCWORK
		},
		{
			"Achievement1121",
			Achievement.ACH_HALLOWEEN_LIKE_CLOCKWORK
		},
		{
			"Achievement1122",
			Achievement.ACH_HALLOWEEN_FRY_ME_TO_THE_MOON
		},
		{
			"Achievement1123",
			Achievement.ACH_HALLOWEEN_PLANKS_NO_THANKS
		},
		{
			"Achievement1132",
			Achievement.ACH_STEAM_WALK_THE_PLANK
		},
		{
			"Achievement1133",
			Achievement.ACH_STEAM_GET_DIZZY
		},
		{
			"Achievement1134",
			Achievement.ACH_STEAM_WHOOPS
		},
		{
			"Achievement1142",
			Achievement.ACH_ICE_TRICKY
		},
		{
			"Achievement1143",
			Achievement.ACH_ICE_NO_ICE_BABY
		},
		{
			"Achievement1144",
			Achievement.ACH_ICE_TAKING_THE_PISTE
		},
		{
			"Achievement1145",
			Achievement.ACH_XMAS_SLIDE
		},
		{
			"Achievement1146",
			Achievement.ACH_XMAS_SKI_LAND
		},
		{
			"Achievement1147",
			Achievement.ACH_XMAS_ANGEL_FALL
		},
		{
			"Achievement1148",
			Achievement.ACH_XMAS_SNOWMAN_HAT
		},
		{
			"Achievement1149",
			Achievement.ACH_THERMAL_COMPLETE
		},
		{
			"Achievement1150",
			Achievement.ACH_THERMAL_PAYDAY
		},
		{
			"Achievement1151",
			Achievement.ACH_THERMAL_SHORTCUT
		},
		{
			"Achievement1152",
			Achievement.ACH_THERMAL_WIRES_TIME_TRIAL
		},
		{
			"Achievement1153",
			Achievement.ACH_FACTORY_COMPLETE
		},
		{
			"Achievement1154",
			Achievement.ACH_FACTORY_RADIOS
		},
		{
			"Achievement1155",
			Achievement.ACH_FACTORY_DARK
		},
		{
			"Achievement1156",
			Achievement.ACH_FACTORY_FIRED
		},
		{
			"IncAchievement000",
			Achievement.ACH_TRAVEL_1KM
		},
		{
			"IncAchievement001",
			Achievement.ACH_TRAVEL_10KM
		},
		{
			"IncAchievement002",
			Achievement.ACH_TRAVEL_100KM
		},
		{
			"IncAchievement003",
			Achievement.ACH_FALL_1000
		},
		{
			"IncAchievement004",
			Achievement.ACH_JUMP_1000
		},
		{
			"IncAchievement005",
			Achievement.ACH_CLIMB_100M
		},
		{
			"IncAchievement006",
			Achievement.ACH_CARRY_1000M
		},
		{
			"IncAchievement007",
			Achievement.ACH_DROWN_10
		},
		{
			"IncAchievement008",
			Achievement.ACH_SHIP_1000M
		},
		{
			"IncAchievement009",
			Achievement.ACH_DRIVE_1000M
		},
		{
			"IncAchievement010",
			Achievement.ACH_DUMPSTER_50M
		}
	};

	public static List<Achievement> achievementList = new List<Achievement>();

	public static void ValidateMaps()
	{
		foreach (KeyValuePair<string, Stat> item in statsMap)
		{
			statsMapInv[item.Value] = item.Key;
		}
		foreach (KeyValuePair<string, Achievement> item2 in achievementMap)
		{
			achievementMapInv[item2.Value] = item2.Key;
		}
		Array values = Enum.GetValues(typeof(Stat));
		for (int i = 0; i < values.Length; i++)
		{
			Stat stat = (Stat)values.GetValue(i);
			if (!statsMapInv.ContainsKey(stat))
			{
				Debug.LogErrorFormat("Mapping for stat {0} not defined", stat);
			}
		}
		Array values2 = Enum.GetValues(typeof(Achievement));
		for (int j = 0; j < values2.Length; j++)
		{
			Achievement achievement = (Achievement)values2.GetValue(j);
			if (!achievementMapInv.ContainsKey(achievement))
			{
				Debug.LogErrorFormat("Mapping for achievement {0} not defined", achievement);
			}
			else
			{
				achievementList.Add(achievement);
			}
		}
	}

	public static string AchievementToSteam(Achievement achievement)
	{
		return achievementMapInv[achievement];
	}

	public static string TryAchievementToSteam(Achievement achievement)
	{
		if (achievementMapInv.TryGetValue(achievement, out string value))
		{
			return value;
		}
		return null;
	}

	public static Achievement AchievementFromSteam(string id)
	{
		if (achievementMap.TryGetValue(id, out Achievement value))
		{
			return value;
		}
		throw new InvalidOperationException("Unknown achievement:" + id);
	}

	public static string StatToSteam(Stat stat)
	{
		return statsMapInv[stat];
	}

	public static Stat StatFromSteam(string id)
	{
		if (statsMap.TryGetValue(id, out Stat value))
		{
			return value;
		}
		throw new InvalidOperationException("Unknown stat:" + id);
	}
}
