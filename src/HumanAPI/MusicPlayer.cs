using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Sound/Music Player", 10)]
	public class MusicPlayer : Music, IPostEndReset
	{
		private MusicManager musicManager;

		private uint evtCollision;

		private NetIdentity identity;

		private void Awake()
		{
			musicManager = Object.FindObjectOfType<MusicManager>();
		}

		public void Start()
		{
			identity = GetComponent<NetIdentity>();
			if (identity != null)
			{
				evtCollision = identity.RegisterEvent(OnTriggerMusic);
			}
		}

		private void OnTriggerMusic(NetStream stream)
		{
			if (mainPlayer != null)
			{
				mainPlayer.Trigger();
			}
			else
			{
				Trigger();
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (!ReplayRecorder.isPlaying && !NetGame.isClient && other.gameObject.tag == "Player")
			{
				if (mainPlayer != null)
				{
					mainPlayer.Trigger();
				}
				else
				{
					Trigger();
				}
				if ((bool)identity)
				{
					identity.BeginEvent(evtCollision);
					identity.EndEvent();
				}
			}
		}

		void IPostEndReset.PostEndResetState(int checkpoint)
		{
			if (checkpoint == 0)
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
}
