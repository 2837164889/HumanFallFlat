using UnityEngine;
using UnityEngine.Audio;

public class GridSnapshot : MonoBehaviour
{
	public AudioMixer mixer;

	public Texture2D map;

	public AudioMixerSnapshot[] snapshots;

	private float[] weights;

	public Vector3 size = new Vector3(100f, 10f, 100f);

	private void Update()
	{
		if (weights == null)
		{
			weights = new float[snapshots.Length];
		}
		Vector3 vector = Vector3.Scale(base.transform.InverseTransformPoint(Listener.instance.transform.position), new Vector3(1f / size.x, 1f / size.y, 1f / size.z));
		Color pixelBilinear = map.GetPixelBilinear(vector.x, vector.z);
		weights[0] = Mathf.Clamp01(2f * pixelBilinear.r - 1f);
		weights[1] = Mathf.Clamp01(1f - 2f * pixelBilinear.r);
		weights[2] = Mathf.Clamp01(2f * pixelBilinear.g - 1f);
		weights[3] = Mathf.Clamp01(1f - 2f * pixelBilinear.g);
		weights[4] = Mathf.Clamp01(2f * pixelBilinear.b - 1f);
		weights[5] = Mathf.Clamp01(1f - 2f * pixelBilinear.b);
		mixer.TransitionToSnapshots(snapshots, weights, Time.deltaTime);
	}

	public void OnDrawGizmosSelected()
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled)
		{
			return;
		}
		float num = 32f;
		for (float num2 = 0f; num2 <= 1f; num2 += 1f / num)
		{
			for (float num3 = 0f; num3 <= 1f; num3 += 1f / num)
			{
				Gizmos.color = map.GetPixelBilinear(num2, num3);
				Gizmos.DrawCube(base.transform.TransformPoint(num2 * size.x, 0f, num3 * size.z), size / num);
			}
		}
		if (Listener.instance != null)
		{
			Vector3 vector = Vector3.Scale(base.transform.InverseTransformPoint(Listener.instance.transform.position), new Vector3(1f / size.x, 1f / size.y, 1f / size.z));
			Color color = Gizmos.color = map.GetPixelBilinear(vector.x, vector.z);
			Gizmos.DrawSphere(base.transform.TransformPoint(vector.x * size.x, 0f, vector.z * size.z), size.x / num);
		}
	}
}
