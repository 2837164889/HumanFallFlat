using UnityEngine;

public class ZiplineAchievement : MonoBehaviour
{
	public bool achieved;

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !achieved)
		{
			achieved = true;
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_SIEGE_ZIPLINE);
		}
	}
}
