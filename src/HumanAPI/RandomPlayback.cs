using System.Collections;
using UnityEngine;

namespace HumanAPI
{
	public class RandomPlayback : MonoBehaviour
	{
		public float minDelay = 1f;

		public float maxDelay = 3f;

		private IEnumerator Start()
		{
			Sound2 sound = GetComponent<Sound2>();
			yield return new WaitForSeconds(Random.Range(0f, maxDelay));
			while (true)
			{
				if (sound.isPlaying)
				{
					yield return null;
					continue;
				}
				sound.Play();
				yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
			}
		}
	}
}
