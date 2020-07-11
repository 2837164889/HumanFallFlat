using UnityEngine;

public class BigStackAchievement : MonoBehaviour
{
	public Collider[] boxes;

	public float limit = 2f;

	private bool currentlyStacked;

	private void Update()
	{
		Bounds bounds = boxes[0].bounds;
		for (int i = 1; i < boxes.Length; i++)
		{
			bounds.Encapsulate(boxes[i].bounds);
		}
		Vector3 size = bounds.size;
		if (size.x <= limit)
		{
			Vector3 size2 = bounds.size;
			if (size2.z <= limit)
			{
				if (!currentlyStacked)
				{
					StatsAndAchievements.UnlockAchievement(Achievement.ACH_CARRY_BIG_STACK);
				}
				currentlyStacked = true;
				return;
			}
		}
		currentlyStacked = false;
	}
}
