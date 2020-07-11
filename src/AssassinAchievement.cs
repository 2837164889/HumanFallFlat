using UnityEngine;

public class AssassinAchievement : PathAchievementBase
{
	public float maxTime = 10f;

	public float currentTime;

	private float startTime;

	public override void UnlockAchievement()
	{
		StatsAndAchievements.UnlockAchievement(Achievement.ACH_SIEGE_ASSASIN);
	}

	protected override bool IsValid(Collider trackedPlayer)
	{
		currentTime = Time.time - startTime;
		return currentTime < maxTime;
	}

	protected override void OnEntry(Collider trackedPlayer)
	{
		base.OnEntry(trackedPlayer);
		startTime = Time.time;
		currentTime = 0f;
	}
}
