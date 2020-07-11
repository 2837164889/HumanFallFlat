using I2.Loc;
using Multiplayer;
using System;
using UnityEngine;

public class CheatCodes : MonoBehaviour
{
	public static bool cheatMode;

	public static bool climbCheat;

	public static bool throwCheat;

	private void Start()
	{
		Shell.RegisterCommand("fps", ToggleFps, "fps\r\nToggle first person camera");
		Shell.RegisterCommand("progressunlock", ProgressUnlock);
		Shell.RegisterCommand("progressreset", ProgressReset);
		Shell.RegisterCommand("steamreset", SteamReset);
		Shell.RegisterCommand("l", LevelChange);
		Shell.RegisterCommand("level", LevelChange, "level <level> <checkpoint>\r\nLoad checkpoint at level\r\n\t<level> - 0 based level number, e.g. 0-Intro, 1-Train\r\n\t<checkpoint> - 0 based checkpoint number");
		Shell.RegisterCommand("c", CheckpointChange);
		Shell.RegisterCommand("cp", CheckpointChange, "cp <checkpoint>\r\nLoad checkpoint\r\n\t<checkpoint> - 0 based checkpoint number");
		Shell.RegisterCommand("climbcheat", CheatClimb);
		Shell.RegisterCommand("throwcheat", CheatThrow);
		climbCheat = (PlayerPrefs.GetInt("cheatClimb", 0) > 0);
		throwCheat = (PlayerPrefs.GetInt("cheatThrow", 0) > 0);
	}

	private void CheatClimb()
	{
		climbCheat = !climbCheat;
		if (climbCheat)
		{
			Shell.Print("climb cheats on");
		}
		else
		{
			Shell.Print("climb cheats off");
		}
		PlayerPrefs.SetInt("cheatClimb", climbCheat ? 1 : 0);
	}

	private void CheatThrow()
	{
		throwCheat = !throwCheat;
		if (throwCheat)
		{
			Shell.Print("throw cheats on");
		}
		else
		{
			Shell.Print("throw cheats off");
		}
		PlayerPrefs.SetInt("cheatThrow", throwCheat ? 1 : 0);
	}

	private void LevelChange(string txt)
	{
		if (string.IsNullOrEmpty(txt))
		{
			return;
		}
		string[] array = txt.Split(new char[1]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 2)
		{
			return;
		}
		int result = 0;
		int result2 = 0;
		if (int.TryParse(array[0], out result) && int.TryParse(array[1], out result2) && result >= 0 && result2 >= 0 && result < Game.instance.levels.Length && !NetGame.isClient)
		{
			if (NetGame.isServer && Game.instance.currentLevelNumber != result)
			{
				Shell.Print("Cannot change the level in multiplayer");
			}
			else
			{
				try
				{
					if (GameSave.ProgressMoreOrEqual(result, result2) || Game.instance.workshopLevel != null)
					{
						if (result == Game.instance.currentLevelNumber)
						{
							Game.instance.RestartCheckpoint(result2, 0);
						}
						else
						{
							App.instance.StartNextLevel((ulong)result, result2);
						}
						Shell.Print($"loading level {result} {result2}");
					}
					else
					{
						Shell.Print("still locked");
					}
				}
				catch
				{
				}
			}
		}
	}

	private void CheckpointChange(string txt)
	{
		if (string.IsNullOrEmpty(txt))
		{
			return;
		}
		string[] array = txt.Split(new char[1]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 1)
		{
			int result = 0;
			if (int.TryParse(array[0], out result) && !NetGame.isClient)
			{
				try
				{
					if (GameSave.ProgressMoreOrEqual(Game.instance.currentLevelNumber, result) || Game.instance.workshopLevel != null)
					{
						Game.instance.RestartCheckpoint(result, 0);
						Shell.Print($"loading checkpoint {result}");
					}
					else
					{
						Shell.Print("still locked");
					}
				}
				catch
				{
				}
			}
		}
	}

	private void ToggleFps()
	{
		for (int i = 0; i < NetGame.instance.local.players.Count; i++)
		{
			NetPlayer netPlayer = NetGame.instance.local.players[i];
			if (netPlayer.cameraController.mode != CameraMode.FirstPerson)
			{
				netPlayer.cameraController.mode = CameraMode.FirstPerson;
				Shell.Print("fps on");
			}
			else
			{
				netPlayer.cameraController.mode = CameraMode.Far;
				Shell.Print("fps off");
			}
		}
	}

	private void ProgressReset()
	{
		GameSave.StartNewGame();
		Shell.Print("progress reset");
	}

	private void ProgressUnlock()
	{
		GameSave.CompleteGame(Game.instance.levelCount);
		Shell.Print("progress unlocked");
	}

	private void SteamReset()
	{
		StatsAndAchievements.ResetAchievements();
		Shell.Print("achievements reset");
	}

	public static void NotifyCheat(string cheat)
	{
		if (!cheatMode)
		{
			SubtitleManager.instance.SetProgress(string.Format(ScriptLocalization.TUTORIAL.CHEAT, cheat), 5f, -1f);
			Game.instance.singleRun = false;
			cheatMode = true;
		}
	}

	public static void SetCheckpoint(int checkpoint)
	{
		Game.instance.currentCheckpointNumber = checkpoint;
		Game.instance.RestartCheckpoint();
	}
}
