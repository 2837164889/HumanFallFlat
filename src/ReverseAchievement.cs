using UnityEngine;

public class ReverseAchievement : MonoBehaviour
{
	public Collider ship;

	public void OnTriggerEnter(Collider other)
	{
		if (other == ship && Vector3.Dot(other.transform.up, base.transform.forward) > 0f)
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_WATER_REVERSE_GEAR);
		}
	}
}
