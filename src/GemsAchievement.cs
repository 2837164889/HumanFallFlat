using UnityEngine;

public class GemsAchievement : MonoBehaviour
{
	public Transform gemsParent;

	private Transform[] gems;

	public float limit = 5f;

	private bool currentlyStacked;

	private void Awake()
	{
		gems = new Transform[gemsParent.childCount];
		for (int i = 0; i < gemsParent.childCount; i++)
		{
			gems[i] = gemsParent.GetChild(i);
		}
	}

	private void Update()
	{
		Bounds bounds = new Bounds(gems[0].position, Vector3.zero);
		for (int i = 1; i < gems.Length; i++)
		{
			bounds.Encapsulate(gems[i].position);
		}
		if (bounds.size.sqrMagnitude <= limit * limit)
		{
			if (!currentlyStacked)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_CLIMB_GEMS);
			}
			currentlyStacked = true;
		}
		else
		{
			currentlyStacked = false;
		}
	}
}
