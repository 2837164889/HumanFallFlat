using UnityEngine;

public class XmasAngelAchievement : MonoBehaviour
{
	public bool achieved;

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "Angel" && !achieved)
		{
			achieved = true;
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_XMAS_ANGEL_FALL);
		}
	}
}
