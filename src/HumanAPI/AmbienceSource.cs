using System;
using UnityEngine;

namespace HumanAPI
{
	public class AmbienceSource : Sound2
	{
		private float transitionSpeed = 10000000f;

		private float transitionPhase;

		public float transitionFrom;

		public float transitionTo;

		private void Start()
		{
			SetVolume(0f);
			Play(forceLoop: true);
		}

		protected override void Update()
		{
			if (transitionPhase < 1f)
			{
				transitionPhase = Mathf.Clamp01(transitionPhase + Time.deltaTime * transitionSpeed);
				SetVolume(Mathf.Lerp(transitionFrom, transitionTo, Mathf.Sqrt(transitionPhase)));
			}
			base.Update();
		}

		internal void FadeVolume(float volume, float duration)
		{
			if (duration == 0f)
			{
				throw new ArgumentException("duration can't be 0", "duration");
			}
			transitionFrom = rtVolume;
			transitionTo = volume;
			transitionSpeed = 1f / duration;
			transitionPhase = 0f;
		}
	}
}
