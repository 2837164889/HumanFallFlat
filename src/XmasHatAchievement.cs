using UnityEngine;

public class XmasHatAchievement : MonoBehaviour
{
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_XMAS_SNOWMAN_HAT);
		}
	}
}
