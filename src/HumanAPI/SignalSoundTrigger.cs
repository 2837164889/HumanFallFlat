using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Sound/Signal Trigger", 10)]
	public class SignalSoundTrigger : Node
	{
		public NodeInput input;

		private bool knownState;

		private Sound2 sound2;

		protected void Awake()
		{
			priority = NodePriority.Update;
			sound2 = GetComponent<Sound2>();
			if (sound2 == null)
			{
				Debug.LogError("SignalSoundTrigger requires a sound", this);
			}
		}

		public override void Process()
		{
			base.Process();
			float value = input.value;
			bool flag = value >= 0.5f;
			if (flag != knownState)
			{
				knownState = flag;
				if (flag && !SignalManager.skipTransitions)
				{
					sound2.PlayOneShot();
				}
			}
		}
	}
}
