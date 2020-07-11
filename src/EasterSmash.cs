using UnityEngine;

public class EasterSmash : MonoBehaviour
{
	public VoronoiShatter[] glasses;

	private bool completed;

	private void Update()
	{
		if (completed)
		{
			if (!glasses[0].shattered)
			{
				completed = false;
			}
			return;
		}
		for (int i = 0; i < glasses.Length; i++)
		{
			if (!glasses[i].shattered)
			{
				return;
			}
		}
		GetComponent<AudioSource>().Play();
		completed = true;
	}
}
