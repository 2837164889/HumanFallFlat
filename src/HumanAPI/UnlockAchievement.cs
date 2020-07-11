namespace HumanAPI
{
	public class UnlockAchievement : Node
	{
		public NodeInput input;

		public Achievement achievementToUnlock;

		public override void Process()
		{
			base.Process();
			if (input.value > 0.5f)
			{
				StatsAndAchievements.UnlockAchievement(achievementToUnlock);
			}
		}
	}
}
