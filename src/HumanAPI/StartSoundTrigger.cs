using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Sound/Start Trigger", 10)]
	public class StartSoundTrigger : MonoBehaviour
	{
		public Sound2 sound2;

		public bool loop = true;

		private void Start()
		{
			sound2.Play(loop);
		}
	}
}
