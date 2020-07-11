using UnityEngine;

public class LightProbeVolume : MonoBehaviour
{
	public RaycastHit hitInfo;

	public Vector3 offset;

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(hitInfo.point, hitInfo.point + offset);
		Gizmos.DrawSphere(hitInfo.point + offset, 0.1f);
	}
}
