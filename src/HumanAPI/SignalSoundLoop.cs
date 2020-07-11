using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Sound/Signal Loop", 10)]
	public class SignalSoundLoop : Node
	{
		public NodeInput input;

		public Sound2 sound2;

		private bool knownState;

		protected override void OnEnable()
		{
			base.OnEnable();
			if (sound2 == null)
			{
				sound2 = GetComponent<Sound2>();
			}
		}

		public override void Process()
		{
			base.Process();
			bool flag = Mathf.Abs(input.value) >= 0.5f;
			if (flag != knownState)
			{
				knownState = flag;
				if (flag)
				{
					sound2.Play();
				}
				else
				{
					sound2.Stop();
				}
			}
		}
	}
}
