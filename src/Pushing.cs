using UnityEngine;

public class Pushing : MonoBehaviour
{
	public float multiplier = 5f;

	public Vector3 direction;

	public float tresholdLow = 400f;

	public float maxForce = 1000f;

	public Rigidbody negativeBody;

	public Rigidbody positiveBody;

	public float negativeForce;

	public float positiveForce;

	public Vector3 negativePoint;

	public Vector3 positivePoint;

	public void OnCollisionEnter(Collision collision)
	{
		HandleCollision(collision);
	}

	public void OnCollisionStay(Collision collision)
	{
		HandleCollision(collision);
	}

	private void HandleCollision(Collision collision)
	{
		if (collision.contacts.Length != 0 && collision.gameObject.layer == 0)
		{
			Vector3 impulse = collision.GetImpulse();
			float num = Vector3.Dot(impulse, direction) / Time.fixedDeltaTime;
			if (Vector3.Dot(collision.contacts[0].normal, impulse) > 0f)
			{
				num *= -1f;
			}
			num *= Mathf.InverseLerp(0.4f, 0.7f, Mathf.Abs(Vector3.Dot(impulse.normalized, direction)));
			if (num < negativeForce)
			{
				negativeForce = num;
				negativeBody = collision.rigidbody;
				negativePoint = collision.GetPoint();
			}
			else if (num > positiveForce)
			{
				positiveForce = num;
				positiveBody = collision.rigidbody;
				positivePoint = collision.GetPoint();
			}
		}
	}

	public void FixedUpdate()
	{
		if (negativeForce < 0f - tresholdLow && positiveForce > tresholdLow)
		{
			if (negativeBody != null && !negativeBody.isKinematic)
			{
				float d = Mathf.InverseLerp(2f, 0f, negativeBody.velocity.magnitude);
				negativeBody.AddForceAtPosition(d * direction * Mathf.Clamp(negativeForce * multiplier, 0f - maxForce, 0f), negativePoint, ForceMode.Force);
			}
			if (positiveBody != null && !positiveBody.isKinematic)
			{
				float d2 = Mathf.InverseLerp(2f, 0f, positiveBody.velocity.magnitude);
				positiveBody.AddForceAtPosition(d2 * direction * Mathf.Clamp(positiveForce * multiplier, 0f, maxForce), positivePoint, ForceMode.Force);
			}
		}
		positiveForce = (negativeForce = 0f);
	}
}
