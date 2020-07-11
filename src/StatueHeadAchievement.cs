using UnityEngine;

public class StatueHeadAchievement : MonoBehaviour
{
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_INTRO_STATUE_HEAD);
		}
	}
}
