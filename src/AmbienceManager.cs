using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
	public static AmbienceManager instance;

	private List<AmbienceZoneTrigger> activeZones = new List<AmbienceZoneTrigger>();

	private AmbienceMix playingMix;

	public List<AmbienceSource> sources = new List<AmbienceSource>();

	public List<AmbienceSource> transitionToSources = new List<AmbienceSource>();

	private void OnEnable()
	{
		instance = this;
	}

	public void RegisterSource(AmbienceSource source)
	{
		if (!transitionToSources.Contains(source))
		{
			transitionToSources.Add(source);
		}
		if (sources.Contains(source))
		{
			sources.Remove(source);
		}
	}

	private void TransitionToMix(AmbienceMix mix, float duration)
	{
		mix.TransitionTo(duration);
		for (int i = 0; i < sources.Count; i++)
		{
			sources[i].FadeVolume(0f, duration);
		}
		sources.Clear();
		sources.AddRange(transitionToSources);
		transitionToSources.Clear();
	}

	public void EnterZone(AmbienceZoneTrigger trigger)
	{
		if (!activeZones.Contains(trigger))
		{
			activeZones.Add(trigger);
			CalculateActiveZone();
		}
	}

	public void LeaveZone(AmbienceZoneTrigger trigger)
	{
		if (activeZones.Contains(trigger))
		{
			activeZones.Remove(trigger);
			CalculateActiveZone();
		}
	}

	private void CalculateActiveZone()
	{
		int num = int.MinValue;
		AmbienceZoneTrigger ambienceZoneTrigger = null;
		for (int i = 0; i < activeZones.Count; i++)
		{
			if (activeZones[i].priority > num)
			{
				ambienceZoneTrigger = activeZones[i];
				num = activeZones[i].priority;
			}
		}
		if (!(ambienceZoneTrigger == null) && (!(playingMix != null) || !(playingMix == ambienceZoneTrigger.mix)))
		{
			playingMix = ambienceZoneTrigger.mix;
			TransitionToMix(playingMix, ambienceZoneTrigger.transitionDuration);
		}
	}
}
