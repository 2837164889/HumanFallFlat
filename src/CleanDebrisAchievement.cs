using System.Collections.Generic;
using UnityEngine;

public class CleanDebrisAchievement : MonoBehaviour
{
	public int piecesToUnlock = 5;

	private List<Collider> stonesInside = new List<Collider>();

	public void OnTriggerEnter(Collider other)
	{
		if (other.name.Contains("Debris") && !stonesInside.Contains(other))
		{
			stonesInside.Add(other);
			if (stonesInside.Count == piecesToUnlock)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_PUSH_CLEAN_DEBRIS);
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (stonesInside.Contains(other))
		{
			stonesInside.Remove(other);
		}
	}
}
