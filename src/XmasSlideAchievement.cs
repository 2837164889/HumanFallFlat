using UnityEngine;

public class XmasSlideAchievement : MonoBehaviour
{
	public bool achieved;

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !achieved)
		{
			achieved = true;
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_XMAS_SLIDE);
		}
	}
}
