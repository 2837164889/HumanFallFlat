using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
	public Rigidbody body;

	private void Start()
	{
		if (body == null)
		{
			body = GetComponentInParent<Rigidbody>();
		}
		if (body != null)
		{
			body.centerOfMass = body.transform.InverseTransformPoint(base.transform.position);
		}
	}
}
