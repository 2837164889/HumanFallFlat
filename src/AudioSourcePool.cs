using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
	public AudioSourcePoolId poolId;

	public GameObject template;

	public int minItems;

	public int maxItems = 50;

	private List<AudioSource> active = new List<AudioSource>();

	private Queue<GameObject> available = new Queue<GameObject>();

	private static Dictionary<AudioSourcePoolId, AudioSourcePool> pools = new Dictionary<AudioSourcePoolId, AudioSourcePool>();

	private void OnEnable()
	{
		pools.Add(poolId, this);
		for (int i = 1; i < minItems; i++)
		{
			GameObject gameObject = Object.Instantiate(template);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			available.Enqueue(gameObject);
		}
	}

	private void OnDisable()
	{
		pools.Remove(poolId);
	}

	public static AudioSource Allocate(AudioSourcePoolId id, GameObject parent, Vector3 pos)
	{
		if (pools.TryGetValue(id, out AudioSourcePool value))
		{
			return value.Allocate(parent, pos);
		}
		Debug.LogError("No audio pool for " + id.ToString());
		return null;
	}

	public AudioSource Allocate(GameObject parent, Vector3 pos)
	{
		GameObject gameObject = null;
		AudioSource audioSource = null;
		if (available.Count > 0)
		{
			gameObject = available.Dequeue();
			audioSource = gameObject.GetComponent<AudioSource>();
		}
		else
		{
			for (int i = 0; i < active.Count; i++)
			{
				audioSource = active[i];
				if (audioSource == null)
				{
					active.RemoveAt(i);
					i--;
				}
				else if (!audioSource.isPlaying)
				{
					gameObject = audioSource.gameObject;
					active.RemoveAt(i);
					break;
				}
			}
			if (gameObject == null)
			{
				if (active.Count >= maxItems)
				{
					Debug.LogError("Allocating too much audiosources!" + base.name);
				}
				gameObject = Object.Instantiate(template);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				audioSource = gameObject.GetComponent<AudioSource>();
			}
		}
		gameObject.transform.position = pos;
		active.Add(audioSource);
		gameObject.SetActive(value: true);
		return audioSource;
	}

	public void Free(AudioSource audiosource)
	{
		if (!active.Contains(audiosource))
		{
			Debug.LogError("Freeing non allocated audiosource!");
			return;
		}
		active.Remove(audiosource);
		GameObject gameObject = audiosource.gameObject;
		available.Enqueue(gameObject);
		gameObject.SetActive(value: false);
	}

	public void FreeAll()
	{
		for (int num = active.Count - 1; num >= 0; num--)
		{
			Free(active[num]);
		}
	}
}
