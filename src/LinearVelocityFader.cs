using System.Collections;
using UnityEngine;

public class LinearVelocityFader : MonoBehaviour
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

	private float previousVelocity;

	public bool debug;

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
			if (debug)
			{
				Debug.Log("<color=green> previousVelocity=</color>" + previousVelocity + " currentVelocity=" + rigidbody.velocity.magnitude);
			}
			if (previousVelocity > magnitudeThresholdForTail && rigidbody.velocity.magnitude < 0.0001f)
			{
				if (debug)
				{
					Debug.Log("<color=green> Triggered Tail Sound </color>");
				}
				TriggerTailSound();
				previousVelocity = 0f;
			}
			loopAudioSource.volume = Mathf.Clamp(rigidbody.velocity.magnitude * volumeMultiplier, 0f, 1f);
		}
		previousVelocity = rigidbody.velocity.magnitude;
	}

	private void TriggerTailSound()
	{
		if (tailAudioSource != null)
		{
			tailAudioSource.Play();
		}
	}
}
