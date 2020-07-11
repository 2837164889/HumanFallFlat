using UnityEngine;

public class AchievementLadderBreak : MonoBehaviour
{
	private void OnJointBreak(float breakForce)
	{
		StatsAndAchievements.UnlockAchievement(Achievement.ACH_STEAM_WHOOPS);
	}
}
