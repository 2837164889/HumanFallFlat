using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class Music : MonoBehaviour
	{
		public string song;

		public Music mainPlayer;

		public float restartInMinutes = 10f;

		public bool overrideShuffle;

		private bool playOnLoad;

		protected float lastPlayTime = float.MinValue;

		protected static Music currentMusic;

		public void Trigger()
		{
			if ((App.state == AppSate.PlayLevel || App.state == AppSate.ServerPlayLevel || App.state == AppSate.ClientPlayLevel) && Time.time - lastPlayTime > restartInMinutes * 60f)
			{
				lastPlayTime = Time.time;
				PlayMusic();
			}
		}

		public void PlayMusic()
		{
			if ((!MusicManager.instance.shuffle || overrideShuffle) && (!MusicManager.instance.shuffle || !(currentMusic == this)))
			{
				currentMusic = this;
				MusicManager.instance.PlayTriggeredMusic(song);
			}
		}

		protected virtual void Update()
		{
			if ((App.state == AppSate.PlayLevel || App.state == AppSate.ServerPlayLevel || App.state == AppSate.ClientPlayLevel) && !MusicManager.instance.shuffle && currentMusic == this && Time.time - lastPlayTime > restartInMinutes * 60f)
			{
				lastPlayTime = Time.time;
				PlayMusic();
			}
		}
	}
}
