using UnityEngine;

public class SkinOverrideBox : MonoBehaviour
{
	public float weight = 1f;

	public Vector3 scale = new Vector3(0.25f, 0.25f, 0.25f);

	public Color colour = new Color(1f, 0f, 0f, 0.5f);

	public Matrix4x4 GetBindPose(Transform ragdoll)
	{
		return base.transform.worldToLocalMatrix;
	}

	public float GetWeight(Vector3 pos)
	{
		return 1f;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = colour;
		Gizmos.DrawCube(base.transform.position, scale);
	}
}
