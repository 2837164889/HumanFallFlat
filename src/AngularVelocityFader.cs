using System.Collections;
using UnityEngine;

public class AngularVelocityFader : MonoBehaviour
{
	[SerializeField]
	private AudioSource loopAudioSource;

	[SerializeField]
	private AudioSource tailAudioSource;

	[SerializeField]
	private Rigidbody rigidbody;

	[SerializeField]
	private float volumeMultiplier = 0.6f;

	[SerializeField]
	private float magnitudeThresholdForTail = 0.1f;

	[SerializeField]
	private float enableDelay = 3f;

	private bool soundEnabled;

	private float angularVelocityMagnitude;

	private void Awake()
	{
		loopAudioSource.loop = true;
		loopAudioSource.volume = 0f;
		loopAudioSource.playOnAwake = true;
	}

	private void Start()
	{
		StartCoroutine(EnableVelocityBasedSound());
	}

	private IEnumerator EnableVelocityBasedSound()
	{
		yield return new WaitForSeconds(enableDelay);
		soundEnabled = true;
	}

	private void Update()
	{
		if (soundEnabled)
		{
			if (angularVelocityMagnitude > magnitudeThresholdForTail && rigidbody.angularVelocity.magnitude < 0.01f)
			{
				TriggerTailSound();
				angularVelocityMagnitude = 0f;
			}
			loopAudioSource.volume = Mathf.Clamp(rigidbody.angularVelocity.magnitude, 0f, 1f) * volumeMultiplier;
		}
		angularVelocityMagnitude = rigidbody.angularVelocity.magnitude;
	}

	private void TriggerTailSound()
	{
		if (tailAudioSource != null)
		{
			tailAudioSource.Play();
		}
	}
}
