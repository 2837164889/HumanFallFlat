using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AudioGridOverride : MonoBehaviour
{
	public float layer;

	public float innerZoneOffset;

	public float maxVolume = 1f;

	public float baseOpacity = 1f;

	private BoxCollider collider;

	private Sound2 sound;

	private AudioGrid grid;

	private float currentVolume;

	public static List<AudioGridOverride> all = new List<AudioGridOverride>();

	private void OnEnable()
	{
		sound = GetComponent<Sound2>();
		collider = GetComponent<BoxCollider>();
		grid = GetComponentInParent<AudioGrid>();
		int i;
		for (i = 0; i < all.Count && !(all[i].layer > layer); i++)
		{
		}
		all.Insert(i, this);
	}

	private void OnDisable()
	{
		all.Remove(this);
	}

	public void OnDrawGizmosSelected()
	{
		collider = GetComponent<BoxCollider>();
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0f, 0f, 0.8f);
		Gizmos.DrawCube(collider.center, collider.size - Vector3.one * innerZoneOffset * 2f);
		Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
		Gizmos.DrawCube(collider.center, collider.size);
		Gizmos.matrix = matrix;
	}

	public float ApplyVolume(int idx, Vector3 pos)
	{
		float num = 0f;
		if (idx + 1 < all.Count)
		{
			num = all[idx + 1].ApplyVolume(idx + 1, pos);
		}
		float num2 = GetOpacity(pos) * baseOpacity;
		if (num2 < grid.volumeTreshold)
		{
			if (sound.isPlaying)
			{
				sound.Stop();
			}
		}
		else if (!sound.isPlaying)
		{
			sound.Play();
		}
		currentVolume = Mathf.MoveTowards(currentVolume, num2 * (1f - num), Time.deltaTime / grid.fadeDuration);
		sound.SetVolume(currentVolume);
		return num + (1f - num) * num2;
	}

	public float GetOpacity(Vector3 pos)
	{
		Vector3 vector = base.transform.InverseTransformPoint(pos) - collider.center;
		float b = innerZoneOffset;
		Vector3 size = collider.size;
		float a = Mathf.InverseLerp(0f, b, size.x / 2f - Mathf.Abs(vector.x));
		float b2 = innerZoneOffset;
		Vector3 size2 = collider.size;
		float b3 = Mathf.InverseLerp(0f, b2, size2.y / 2f - Mathf.Abs(vector.y));
		float b4 = innerZoneOffset;
		Vector3 size3 = collider.size;
		float b5 = Mathf.InverseLerp(0f, b4, size3.z / 2f - Mathf.Abs(vector.z));
		return Mathf.Min(Mathf.Min(a, b3), b5);
	}
}
