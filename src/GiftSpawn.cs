using UnityEngine;

public class GiftSpawn : MonoBehaviour
{
	public float probability = 1f;

	public Vector3 GetPosition()
	{
		return base.transform.TransformPoint(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}
}
