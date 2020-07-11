using UnityEngine;

public class FeetHeadAchievement : MonoBehaviour
{
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Human componentInParent = other.GetComponentInParent<Human>();
			Vector3 position = componentInParent.ragdoll.partHead.transform.position;
			float y = position.y;
			Vector3 position2 = componentInParent.ragdoll.partHips.transform.position;
			if (y > position2.y)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_LVL_RIVER_FEET);
			}
			else
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_LVL_RIVER_HEAD);
			}
		}
	}
}
