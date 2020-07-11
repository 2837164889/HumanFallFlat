using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class SoundSourcePool : MonoBehaviour
	{
		public static SoundSourcePool instance;

		private Queue<AudioSource> pool = new Queue<AudioSource>();

		private List<AudioSource> playingSources = new List<AudioSource>();

		private void Awake()
		{
			instance = this;
		}

		public AudioSource GetAudioSource()
		{
			while (pool.Count > 0)
			{
				AudioSource audioSource = pool.Dequeue();
				if (audioSource != null)
				{
					return audioSource;
				}
			}
			for (int i = 0; i < playingSources.Count; i++)
			{
				AudioSource audioSource2 = playingSources[i];
				if (audioSource2 == null)
				{
					playingSources.RemoveAt(i);
					i--;
				}
				else if (!audioSource2.isPlaying)
				{
					playingSources.RemoveAt(i);
					return audioSource2;
				}
			}
			GameObject gameObject = new GameObject("pooled audio");
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			AudioSource result = gameObject.AddComponent<AudioSource>();
			AudioLowPassFilter audioLowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
			return result;
		}

		public void ReleaseAudioSource(AudioSource source)
		{
			pool.Enqueue(source);
			source.Stop();
			source.clip = null;
			source.loop = false;
			source.transform.SetParent(base.transform, worldPositionStays: false);
		}

		public void ReleaseAudioSourceOnComplete(AudioSource source)
		{
			playingSources.Add(source);
		}
	}
}
