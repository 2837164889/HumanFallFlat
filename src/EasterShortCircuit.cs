using System.Collections;
using UnityEngine;

public class EasterShortCircuit : MonoBehaviour
{
	public static EasterShortCircuit instance;

	public int countNeeded = 5;

	public float allowedIdle = 60f;

	private float lastTime;

	private int count;

	private void Awake()
	{
		instance = this;
	}

	public void ShorCircuit()
	{
		float time = Time.time;
		if (time - lastTime > allowedIdle)
		{
			count = 1;
		}
		else
		{
			count++;
		}
		lastTime = time;
		if (count >= countNeeded)
		{
			StartCoroutine(Play());
		}
	}

	private IEnumerator Play()
	{
		yield return new WaitForSeconds(3f);
		GetComponent<AudioSource>().Play();
		count = 0;
		countNeeded++;
	}
}
