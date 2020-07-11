using UnityEngine;

namespace HumanAPI
{
	public class SignalSoundLoopExternal : Node
	{
		public NodeInput input;

		[SerializeField]
		private string soundGameObjectName = string.Empty;

		private Sound2 sound2;

		private bool knownState;

		protected override void OnEnable()
		{
			base.OnEnable();
			if (sound2 == null)
			{
				GameObject gameObject = GameObject.Find(soundGameObjectName);
				if (gameObject != null)
				{
					sound2 = gameObject.GetComponent<Sound2>();
				}
			}
		}

		public override void Process()
		{
			base.Process();
			bool flag = Mathf.Abs(input.value) >= 0.1f;
			if (flag != knownState && sound2 != null)
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
