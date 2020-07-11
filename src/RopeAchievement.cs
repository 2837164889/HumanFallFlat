public class RopeAchievement : PathAchievementBase
{
	public override void UnlockAchievement()
	{
		StatsAndAchievements.UnlockAchievement(Achievement.ACH_CLIMB_ROPE);
	}
}
