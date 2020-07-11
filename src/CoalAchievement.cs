using System.Collections.Generic;
using UnityEngine;

public class CoalAchievement : MonoBehaviour
{
	public int piecesToDeliver = 10;

	public List<Collider> delivered = new List<Collider>();

	public Transform validCoalParent;

	public void OnTriggerEnter(Collider other)
	{
		if (!delivered.Contains(other) && other.transform.IsChildOf(validCoalParent))
		{
			delivered.Add(other);
			if (delivered.Count == piecesToDeliver)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_POWER_COAL_DELIVER);
			}
		}
	}
}
