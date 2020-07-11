using UnityEngine;

namespace HumanAPI
{
	public class LerpParticles : LerpBase
	{
		public Color colorOff1 = Color.white;

		public Color colorOff2 = Color.white;

		public Color colorOn1 = Color.white;

		public Color colorOn2 = Color.white;

		private float timer;

		private float rate;

		private float sinceLastEmit;

		public float rateOff;

		public float rateOn = 100f;

		private ParticleSystem particles;

		private float storedValue;

		protected override void OnEnable()
		{
			base.OnEnable();
			particles = GetComponent<ParticleSystem>();
		}

		public void Update()
		{
			if (rate != 0f)
			{
				sinceLastEmit += Time.deltaTime;
				while (sinceLastEmit > 1f / rate)
				{
					sinceLastEmit -= 1f / rate;
					Emit(storedValue);
				}
			}
		}

		protected override void ApplyValue(float value)
		{
			storedValue = value;
			rate = Mathf.Lerp(rateOff, rateOn, value);
			if (rate != 0f && sinceLastEmit > 1f / rate)
			{
				Emit(storedValue);
				sinceLastEmit = 0f;
			}
		}

		public void Emit(float storedValue)
		{
			particles.Emit(new ParticleSystem.EmitParams
			{
				startColor = Color.Lerp(Color.Lerp(colorOff1, colorOn2, storedValue), Color.Lerp(colorOff1, colorOn2, storedValue), Random.value)
			}, 1);
		}
	}
}
