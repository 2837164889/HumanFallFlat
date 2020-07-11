using UnityEngine;

public class CaveAmbientSource : MonoBehaviour
{
	public float radius = 2f;

	public float minRadius = 1f;

	public Color color;

	public float intensity = 0.1f;

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, minRadius);
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
