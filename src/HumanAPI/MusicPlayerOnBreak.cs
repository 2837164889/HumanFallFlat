using Multiplayer;

namespace HumanAPI
{
	public class MusicPlayerOnBreak : Music
	{
		public VoronoiShatter wall;

		private uint evtCollision;

		private NetIdentity identity;

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

		protected override void Update()
		{
			if (ReplayRecorder.isPlaying || NetGame.isClient)
			{
				return;
			}
			if (wall.shattered)
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
				base.enabled = false;
			}
			base.Update();
		}
	}
}
