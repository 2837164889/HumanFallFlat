using UnityEngine;

namespace HumanAPI
{
	public class SignalSoundVolumeMap : Node
	{
		public NodeInput input;

		[SerializeField]
		private string soundGameObjectName = string.Empty;

		[SerializeField]
		private float multiplyFactor = 1f;

		private bool knownState;

		private Sound2 sound2;

		protected void Awake()
		{
			priority = NodePriority.Update;
		}

		public override void Process()
		{
			base.Process();
			float value = input.value;
			if (sound2 != null)
			{
				sound2.SetBaseVolume(input.value * multiplyFactor);
			}
			else
			{
				FindSound();
			}
		}

		private void FindSound()
		{
			GameObject gameObject = GameObject.Find(soundGameObjectName);
			if (gameObject != null)
			{
				sound2 = gameObject.GetComponent<Sound2>();
			}
		}
	}
}
