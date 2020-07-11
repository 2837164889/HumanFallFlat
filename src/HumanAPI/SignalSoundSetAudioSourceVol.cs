using UnityEngine;

namespace HumanAPI
{
	public class SignalSoundSetAudioSourceVol : Node
	{
		public NodeInput input;

		[SerializeField]
		private AudioSource target;

		[SerializeField]
		[Range(0f, 1f)]
		private float targetVolume;

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
				if (flag && !SignalManager.skipTransitions && target != null)
				{
					target.volume = targetVolume;
				}
			}
		}
	}
}
