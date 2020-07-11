using UnityEngine;

namespace HumanAPI
{
	public class SignalLerpVolume : Node
	{
		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private float minThreshold = 0.05f;

		[SerializeField]
		private float multiplyFactor = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float maxVolume = 1f;

		private float desiredVolume;

		public NodeInput input;

		private float targetVolume;

		public void Awake()
		{
			if (audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
			}
		}

		public override void Process()
		{
			base.Process();
			float num = input.value * multiplyFactor;
			if (num < minThreshold)
			{
				desiredVolume = 0f;
			}
			else
			{
				desiredVolume = Mathf.Clamp(num, 0f, maxVolume);
			}
		}

		private void Update()
		{
			if (!(audioSource.volume < 0.001f) || !(desiredVolume < 0.001f))
			{
				if (audioSource.volume > desiredVolume)
				{
					audioSource.volume -= (audioSource.volume - desiredVolume) * 0.2f;
				}
				else
				{
					audioSource.volume += (desiredVolume - audioSource.volume) * 0.2f;
				}
			}
		}
	}
}
