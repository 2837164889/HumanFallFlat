using UnityEngine;

public class ConstantForceAtPoint : MonoBehaviour
{
	private Rigidbody body;

	public Vector3 force;

	private void Start()
	{
		body = GetComponentInParent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (!body.IsSleeping())
		{
			body.AddForceAtPosition(force, base.transform.position);
		}
	}
}
