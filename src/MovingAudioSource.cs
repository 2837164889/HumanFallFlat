using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MovingAudioSource : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float maxVol = 1f;

	private Vector3 lastPositionFrame;

	private float distanceSinceLastFrame;

	private float targetVol;

	private void Start()
	{
		audioSource.pitch = Random.Range(0.95f, 1.05f);
		audioSource.time = Random.Range(0f, 1f);
		lastPositionFrame = base.transform.position;
	}

	private void Update()
	{
		distanceSinceLastFrame = Vector3.Distance(base.transform.position, lastPositionFrame);
		if (distanceSinceLastFrame > 0.1f)
		{
			targetVol = Mathf.Clamp(distanceSinceLastFrame * 2f, 0f, maxVol);
		}
		else
		{
			targetVol = 0f;
		}
		lastPositionFrame = base.transform.position;
		if (audioSource.volume > targetVol)
		{
			audioSource.volume -= (audioSource.volume - targetVol) * 0.2f;
		}
		else
		{
			audioSource.volume += (targetVol - audioSource.volume) * 0.2f;
		}
	}
}
