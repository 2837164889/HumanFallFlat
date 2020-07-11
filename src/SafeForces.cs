using UnityEngine;

public static class SafeForces
{
	public static void SafeAddForce(this Rigidbody body, Vector3 force, ForceMode mode = ForceMode.Force)
	{
		if (float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z) || force.sqrMagnitude > 2.5E+11f)
		{
			Debug.Log("invalid force " + force + " for " + body.name);
		}
		else
		{
			body.AddForce(force, mode);
		}
	}

	public static void SafeAddForceAtPosition(this Rigidbody body, Vector3 force, Vector3 position, ForceMode mode = ForceMode.Force)
	{
		if (float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z) || force.sqrMagnitude > 2.5E+11f)
		{
			Debug.Log("invalid force " + force + " for " + body.name);
		}
		else if (position.x == float.NaN || position.y == float.NaN || position.z == float.NaN || position.sqrMagnitude > 2.5E+11f)
		{
			Debug.Log("invalid position " + position + " for " + body.name);
		}
		else
		{
			body.AddForceAtPosition(force, position, mode);
		}
	}

	public static void SafeAddTorque(this Rigidbody body, Vector3 torque, ForceMode mode = ForceMode.Force)
	{
		if (float.IsNaN(torque.x) || float.IsNaN(torque.y) || float.IsNaN(torque.z) || torque.sqrMagnitude > 1E+10f)
		{
			Debug.Log("invalid torque " + torque + " for " + body.name);
		}
		else
		{
			body.AddTorque(torque, mode);
		}
	}
}
