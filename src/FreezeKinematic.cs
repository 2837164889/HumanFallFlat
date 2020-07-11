using UnityEngine;

public class FreezeKinematic : MonoBehaviour
{
	private Rigidbody body;

	private Vector3 initialPosition;

	private Quaternion initialRotation;

	private void Start()
	{
		initialPosition = base.transform.localPosition;
		initialRotation = base.transform.localRotation;
		body = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (body != null && body.isKinematic)
		{
			base.transform.localPosition = initialPosition;
			base.transform.localRotation = initialRotation;
		}
	}
}
