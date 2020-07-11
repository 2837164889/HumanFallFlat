using System.Collections.Generic;
using UnityEngine;

public class SpeakersAchievement : MonoBehaviour
{
	private List<Collider> speakersInside = new List<Collider>();

	public void OnTriggerEnter(Collider other)
	{
		if (other.name.Contains("Speaker") && !speakersInside.Contains(other))
		{
			speakersInside.Add(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (speakersInside.Contains(other))
		{
			speakersInside.Remove(other);
			if (speakersInside.Count == 0)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_CLIMB_SPEAKERS);
			}
		}
	}
}
