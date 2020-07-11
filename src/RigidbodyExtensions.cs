using UnityEngine;

public static class RigidbodyExtensions
{
	public static void ResetDynamics(this Rigidbody body)
	{
		Vector3 vector2 = body.velocity = (body.angularVelocity = Vector3.zero);
	}
}
