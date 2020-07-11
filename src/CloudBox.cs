using System.Collections.Generic;
using UnityEngine;

public class CloudBox : MonoBehaviour
{
	public float fadeInDuration;

	public float fadeInTime;

	public float fade = 1f;

	public Vector3 innerSize = new Vector3(100f, 50f, 100f);

	public Vector3 outerSize = new Vector3(120f, 70f, 120f);

	public static List<CloudBox> all = new List<CloudBox>();

	public static object cloudLock = new object();

	private Vector3 transformPosition;

	private void OnEnable()
	{
		lock (cloudLock)
		{
			all.Add(this);
		}
	}

	private void OnDisable()
	{
		lock (cloudLock)
		{
			all.Remove(this);
		}
	}

	public void ReadPos()
	{
		transformPosition = base.transform.position;
	}

	public float GetAlpha(Vector3 pos)
	{
		float num = 0f;
		Vector3 vector = pos - transformPosition;
		float num2 = 1f - (Mathf.Abs(vector.x * 2f) - innerSize.x) / (outerSize.x - innerSize.x);
		if (num2 > 0f)
		{
			float num3 = 1f - (Mathf.Abs(vector.y * 2f) - innerSize.y) / (outerSize.y - innerSize.y);
			if (num3 > 0f)
			{
				float num4 = 1f - (Mathf.Abs(vector.z * 2f) - innerSize.z) / (outerSize.z - innerSize.z);
				if (num4 > 0f)
				{
					num = Mathf.Clamp01(num2) * Mathf.Clamp01(num3) * Mathf.Clamp01(num4);
				}
			}
		}
		num = 1f - num;
		return fade * num + (1f - fade);
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(base.transform.position, innerSize);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(base.transform.position, outerSize);
	}

	private void Update()
	{
		if (fadeInDuration == 0f)
		{
			fade = 1f;
			return;
		}
		fadeInTime += Time.deltaTime;
		fade = Mathf.Clamp01(fadeInTime / fadeInDuration);
	}

	public void FadeIn(float duration)
	{
		fadeInTime = 0f;
		fadeInDuration = duration;
	}
}
