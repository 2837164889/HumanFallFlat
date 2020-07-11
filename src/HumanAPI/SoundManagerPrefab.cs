using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class SoundManagerPrefab : MonoBehaviour
	{
		public SoundManager.SoundManagerState sounds;

		public Ambience.AmbienceState ambience;

		public Reverb.ReverbState reverb;

		private Dictionary<string, SoundLibrarySample> samples;

		public void Initialize()
		{
			if (sounds != null)
			{
				sounds.Populate();
			}
			samples = new Dictionary<string, SoundLibrarySample>();
			SoundLibrarySample[] componentsInChildren = GetComponentsInChildren<SoundLibrarySample>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				samples[componentsInChildren[i].name] = componentsInChildren[i];
			}
		}

		public SoundLibrarySample GetSample(string sample)
		{
			samples.TryGetValue(sample, out SoundLibrarySample value);
			return value;
		}

		public SoundManager.SoundState GetSoundState(string name)
		{
			return sounds.GetSoundState(name);
		}

		public SoundManager.GrainState GetGrainState(string name)
		{
			return sounds.GetGrainState(name);
		}
	}
}
