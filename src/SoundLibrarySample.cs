using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrarySample : MonoBehaviour
{
	public string category;

	public float vB = 1f;

	public float vR;

	public float pB = 1f;

	public float pR;

	public float crossFade = 0.1f;

	public AudioClip[] clips;

	private SoundLibrary.SerializedSample serialized;

	public void Load(SoundLibrary.SerializedSample serialized)
	{
		base.name = serialized.name;
		category = serialized.category;
		crossFade = serialized.crossFade;
		vB = serialized.vB;
		vR = serialized.vR;
		pB = serialized.pB;
		pR = serialized.pR;
	}

	public SoundLibrary.SerializedSample GetSerialized()
	{
		if (serialized != null)
		{
			return serialized;
		}
		serialized = new SoundLibrary.SerializedSample
		{
			category = category,
			crossFade = crossFade,
			vB = vB,
			vR = vR,
			pB = pB,
			pR = pR,
			name = base.name,
			builtIn = true,
			clips = new List<SoundLibrary.SerializedClip>()
		};
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i] != null)
			{
				serialized.AddClip(category + "/" + clips[i].name, clips[i].name, null, clips[i]);
			}
			else
			{
				Debug.LogError("Empty clip in sample " + serialized.name);
			}
		}
		serialized.loaded = (SoundSourcePool.instance != null);
		return serialized;
	}
}
