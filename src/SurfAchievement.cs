using UnityEngine;

public class SurfAchievement : PathAchievementBase
{
	public Transform[] matressParts;

	public override void UnlockAchievement()
	{
		StatsAndAchievements.UnlockAchievement(Achievement.ACH_WATER_SURF);
	}

	protected override bool IsValid(Collider trackedPlayer)
	{
		Vector3 position = trackedPlayer.transform.position;
		bool flag = false;
		for (int i = 0; i < matressParts.Length; i++)
		{
			Vector3 vector = position - matressParts[i].position;
			flag |= ((vector.sqrMagnitude < 25f) | (vector.y < -1f));
		}
		return flag;
	}
}
