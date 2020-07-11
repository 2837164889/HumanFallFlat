using UnityEngine;

public class AngularImpulse : MonoBehaviour
{
	private enum axisImpulse
	{
		x,
		y,
		z
	}

	[SerializeField]
	private Rigidbody rigidbody;

	[SerializeField]
	private float angularVelocityThreshold = 0.1f;

	[SerializeField]
	private float yMax = 2f;

	[SerializeField]
	private float multiplyFactor = 10f;

	[SerializeField]
	private Vector3 constantForceVector;

	[SerializeField]
	private Vector3 targetLocalPosition;

	[SerializeField]
	private float speed = 1f;

	[SerializeField]
	private bool debug;

	private bool rotatingClockWise;

	private bool rotatingCounterClockWise;

	private void Update()
	{
		if (rigidbody.angularVelocity.magnitude > angularVelocityThreshold)
		{
			rigidbody.isKinematic = true;
		}
	}
}
