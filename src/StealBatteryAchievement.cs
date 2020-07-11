using UnityEngine;

public class StealBatteryAchievement : MonoBehaviour
{
	public Collider battery;

	public void OnTriggerEnter(Collider other)
	{
		if (other == battery)
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_POWER_STATUE_BATTERY);
		}
	}
}
