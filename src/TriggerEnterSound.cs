using HumanAPI;
using UnityEngine;

public class TriggerEnterSound : MonoBehaviour
{
	public float volume = 1f;

	public Collider[] acceptedColliders;

	public float minDelay = 0.2f;

	private float lastSoundTime;

	public void OnTriggerEnter(Collider other)
	{
		if (acceptedColliders.Length > 0)
		{
			bool flag = false;
			for (int i = 0; i < acceptedColliders.Length; i++)
			{
				if (acceptedColliders[i] == other)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		float time = Time.time;
		if (!(lastSoundTime + minDelay > time))
		{
			GetComponent<Sound2>().PlayOneShot();
			lastSoundTime = time;
		}
	}
}
