using UnityEngine;

public class LowPassRide : MonoBehaviour
{
	public float durationLow = 1f;

	public float durationHigh = 3f;

	public float min = 200f;

	public float max = 22000f;

	private AudioLowPassFilter filter;

	private float from;

	private float to;

	private float duration;

	private float time;

	private void Start()
	{
		filter = GetComponent<AudioLowPassFilter>();
		from = Random.Range(Mathf.Log(min, 2f), Mathf.Log(max, 2f));
		to = Random.Range(Mathf.Log(min, 2f), Mathf.Log(max, 2f));
		time = 0f;
		duration = Random.Range(durationLow, durationHigh);
	}

	private void Update()
	{
		time += Time.deltaTime;
		if (time >= duration)
		{
			from = to;
			to = Random.Range(Mathf.Log(min, 2f), Mathf.Log(max, 2f));
			time = 0f;
			duration = Random.Range(durationLow, durationHigh);
		}
		filter.cutoffFrequency = Mathf.Pow(2f, Mathf.Lerp(from, to, time / duration));
	}
}
