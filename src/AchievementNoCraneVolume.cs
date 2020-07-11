using HumanAPI;
using Multiplayer;

public class AchievementNoCraneVolume : Node
{
	public NodeInput input;

	private const int kSteamWalkThePlankCheckpoint = 11;

	public override void Process()
	{
		if (input.value >= 0.5f && !AchievementNoCrane.usedCrane && App.instance.startedCheckpoint != 11)
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_STEAM_WALK_THE_PLANK);
		}
	}
}
