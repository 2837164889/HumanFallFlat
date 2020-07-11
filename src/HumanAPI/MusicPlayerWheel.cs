using UnityEngine;

namespace HumanAPI
{
	public class MusicPlayerWheel : Music, IReset
	{
		[SerializeField]
		private Rigidbody rigidbody;

		private MusicManager musicManager;

		private void Awake()
		{
			musicManager = Object.FindObjectOfType<MusicManager>();
		}

		protected override void Update()
		{
			if (rigidbody.angularVelocity.magnitude > 1f)
			{
				if (mainPlayer != null)
				{
					mainPlayer.Trigger();
				}
				else
				{
					Trigger();
				}
				base.enabled = false;
			}
			base.Update();
		}

		void IReset.ResetState(int checkpoint, int subObjectives)
		{
			base.enabled = true;
			lastPlayTime = float.MinValue;
			Music.currentMusic = null;
			if ((bool)musicManager)
			{
				musicManager.currentSong = null;
			}
		}
	}
}
