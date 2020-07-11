using UnityEngine;

public class WindowShortcutAchievement : MonoBehaviour
{
	private Collider trackedCollider;

	private float entryX;

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Vector3 vector = base.transform.InverseTransformPoint(other.transform.position);
			entryX = vector.x;
			if (entryX < 0f)
			{
				trackedCollider = other;
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other == trackedCollider)
		{
			Vector3 vector = base.transform.InverseTransformPoint(other.transform.position);
			if (vector.x * entryX < 0f)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_BREAK_WINDOW_SHORTCUT);
			}
			trackedCollider = null;
		}
	}
}
