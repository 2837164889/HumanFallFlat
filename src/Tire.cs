using UnityEngine;

public class Tire : MonoBehaviour
{
	private HingeJoint joint;

	private Rigidbody body;

	private void OnEnable()
	{
		joint = GetComponent<HingeJoint>();
		body = GetComponent<Rigidbody>();
	}

	public void OnCollisionEnter(Collision collision)
	{
		OnCollisionStay(collision);
	}

	public void OnCollisionStay(Collision collision)
	{
		if (collision.contacts.Length != 0)
		{
			float num = Mathf.Abs(Vector3.Dot(collision.GetImpulse(), Vector3.up));
			Vector3 vector = base.transform.TransformDirection(joint.axis.normalized);
			Vector3 point = collision.GetPoint();
			Vector3 pointVelocity = body.GetPointVelocity(point);
			float num2 = Vector3.Dot(pointVelocity, vector);
			if (!(Mathf.Abs(num2) < 0.01f))
			{
				Vector3 a = num2 * vector;
				Vector3 force = (0f - num) * a;
				body.AddForceAtPosition(force, point, ForceMode.Impulse);
			}
		}
	}
}
