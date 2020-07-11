using UnityEngine;

namespace HumanAPI
{
	public class SignalSoundPlayRandom : Node
	{
		public NodeInput input;

		[SerializeField]
		private AudioSource target;

		[SerializeField]
		private AudioClip[] randomSounds;

		private bool knownState;

		protected void Awake()
		{
			priority = NodePriority.Update;
		}

		public override void Process()
		{
			base.Process();
			float value = input.value;
			bool flag = value >= 0.5f;
			if (flag != knownState)
			{
				knownState = flag;
				if (flag && !SignalManager.skipTransitions && target != null && !target.isPlaying)
				{
					target.clip = randomSounds[Random.Range(0, randomSounds.Length)];
					target.Play();
				}
			}
		}
	}
}
