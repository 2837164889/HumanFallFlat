using UnityEngine;

public class SkinOverrideVolume : MonoBehaviour
{
	public float weight = 1f;

	public bool isBox;

	public float innerRadius = 0.49f;

	public float outerRadius = 0.5f;

	public Vector3 boxScale = new Vector3(0.25f, 0.25f, 0.25f);

	public Color colour = new Color(1f, 0f, 0f, 0.5f);

	private bool initedBox;

	private Bounds boxBounds;

	public Matrix4x4 GetBindPose(Transform ragdoll)
	{
		return base.transform.worldToLocalMatrix;
	}

	public float GetWeight(Vector3 pos)
	{
		if (isBox)
		{
			if (!initedBox)
			{
				boxBounds = new Bounds(base.transform.position, boxScale);
				initedBox = true;
			}
			if (boxBounds.Contains(pos))
			{
				return weight;
			}
			return 0f;
		}
		return Mathf.InverseLerp(outerRadius, innerRadius, base.transform.InverseTransformPoint(pos).magnitude);
	}

	private void OnDrawGizmosSelected()
	{
		if (isBox)
		{
			Gizmos.color = colour;
			Gizmos.DrawCube(base.transform.position, boxScale);
			return;
		}
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.DrawSphere(base.transform.position, innerRadius);
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawSphere(base.transform.position, outerRadius);
	}
}
