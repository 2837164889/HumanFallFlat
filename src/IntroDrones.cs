using HumanAPI;
using System.Collections;
using UnityEngine;

public class IntroDrones : MonoBehaviour
{
	public static IntroDrones instance;

	public Sound2 sound;

	private float dronesStartTime;

	public float dronesTime
	{
		get
		{
			AudioSource activeSource = sound.GetActiveSource();
			float num = (!(activeSource != null)) ? 0f : activeSource.time;
			if (num == 0f)
			{
				return Time.timeSinceLevelLoad - dronesStartTime;
			}
			return num;
		}
	}

	private void Start()
	{
		instance = this;
		base.transform.SetParent(null, worldPositionStays: false);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Play()
	{
		sound.Play();
		dronesStartTime = Time.timeSinceLevelLoad;
		StartCoroutine(DestroyWhenDone());
	}

	private IEnumerator DestroyWhenDone()
	{
		AudioClip clip = sound.soundSample.clips[0].clip;
		while (dronesTime < clip.length)
		{
			yield return null;
		}
		instance = null;
		Object.Destroy(base.gameObject);
	}
}
