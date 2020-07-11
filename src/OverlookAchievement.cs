using UnityEngine;

public class OverlookAchievement : MonoBehaviour
{
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_AZTEC_OVERLOOK);
		}
	}
}
