using HumanAPI;
using System;
using UnityEngine;

public class AudioGrid : MonoBehaviour
{
	public static AudioGrid instance;

	public Texture2D map;

	public Vector3 size = new Vector3(100f, 10f, 100f);

	public AudioSource fallbackSource;

	public float fallbackVolume;

	public AudioSource[] sources;

	public Sound2[] sounds;

	public float[] maxVolumes;

	public float[] lowPass;

	public float[] currentVolumes;

	public float volumeTreshold = 0.01f;

	public bool interactive;

	public Color currentColor;

	public bool drawgrid;

	public float subdiv = 32f;

	public float fadeDuration = 2f;

	private AudioLowPassFilter[] lp;

	private void OnEnable()
	{
		instance = this;
	}

	private void Awake()
	{
		currentVolumes = new float[sounds.Length];
		lp = new AudioLowPassFilter[sounds.Length];
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i] != null)
			{
				lp[i] = sounds[i].GetComponent<AudioLowPassFilter>();
			}
		}
	}

	private void Update()
	{
		if (!interactive)
		{
			Vector3 vector = Vector3.Scale(base.transform.InverseTransformPoint(Listener.instance.transform.position), new Vector3(1f / size.x, 1f / size.y, 1f / size.z));
			currentColor = map.GetPixelBilinear(vector.x, vector.z);
		}
		Color color = currentColor;
		float num = 0f;
		if (AudioGridOverride.all.Count > 0)
		{
			num = AudioGridOverride.all[0].ApplyVolume(0, Listener.instance.transform.position);
		}
		float num2 = 1f - num;
		ApplyVolume(0, Mathf.Clamp01(2f * color.r - 1f) * color.a * num2);
		ApplyVolume(1, Mathf.Clamp01(1f - 2f * color.r) * color.a * num2);
		ApplyVolume(2, Mathf.Clamp01(2f * color.g - 1f) * color.a * num2);
		ApplyVolume(3, Mathf.Clamp01(1f - 2f * color.g) * color.a * num2);
		ApplyVolume(4, Mathf.Clamp01(2f * color.b - 1f) * color.a * num2);
		ApplyVolume(5, Mathf.Clamp01(1f - 2f * color.b) * color.a * num2);
		if (sounds.Length > 6 && sounds[6] != null)
		{
			ApplyVolume(6, (1f - color.a) * num2);
		}
	}

	internal void ZoneEntered(AudioGridOverride audioGridOverride)
	{
		throw new NotImplementedException();
	}

	private void ApplyVolume(int idx, float volume)
	{
		if (sounds.Length < idx || sounds[idx] == null)
		{
			return;
		}
		if (volume < volumeTreshold)
		{
			if (sounds[idx].isPlaying)
			{
				sounds[idx].Stop();
			}
		}
		else if (!sounds[idx].isPlaying)
		{
			sounds[idx].Play();
		}
		currentVolumes[idx] = Mathf.MoveTowards(currentVolumes[idx], volume, Time.deltaTime / fadeDuration);
		sounds[idx].SetVolume(currentVolumes[idx]);
		if (lowPass.Length > idx)
		{
			sounds[idx].SetLowPass(Mathf.Lerp(lowPass[idx], 22000f, Mathf.Sqrt(volume)));
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (!base.enabled || !base.gameObject.activeInHierarchy || !drawgrid)
		{
			return;
		}
		for (float num = 0f; num <= 1f; num += 1f / subdiv)
		{
			for (float num2 = 0f; num2 <= 1f; num2 += 1f / subdiv)
			{
				Gizmos.color = map.GetPixelBilinear(num, num2);
				Gizmos.DrawCube(base.transform.TransformPoint(num * size.x, 0f, num2 * size.z), size / subdiv);
			}
		}
		if (Listener.instance != null)
		{
			Vector3 vector = Vector3.Scale(base.transform.InverseTransformPoint(Listener.instance.transform.position), new Vector3(1f / size.x, 1f / size.y, 1f / size.z));
			Color color = Gizmos.color = map.GetPixelBilinear(vector.x, vector.z);
			Gizmos.DrawSphere(base.transform.TransformPoint(vector.x * size.x, 0f, vector.z * size.z), size.x / subdiv);
		}
	}
}
