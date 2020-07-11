using UnityEngine;

public class ContainerWheel : MonoBehaviour
{
	public float staticFriction = 0.2f;

	public float rollingFriction = 0.2f;

	public float connectedMass;

	public float treshold = 1f;

	public Transform mesh;

	public Vector3 upAxis;

	public Vector3 forwardAxis;

	public Rigidbody body;

	private void OnEnable()
	{
		body = GetComponentInParent<Rigidbody>();
		body.maxAngularVelocity = 50f;
	}

	public void FixedUpdate()
	{
		if (rollingFriction != 0f || staticFriction != 0f)
		{
			Vector3 pointVelocity = body.GetPointVelocity(base.transform.position);
			float magnitude = pointVelocity.magnitude;
			if (!(magnitude < 0.01f))
			{
				float num = (!(magnitude > treshold)) ? staticFriction : rollingFriction;
				float a = magnitude * connectedMass / Time.fixedDeltaTime;
				body.SafeAddForceAtPosition(-pointVelocity.normalized * Mathf.Min(a, num * connectedMass), base.transform.position);
				Debug.DrawRay(base.transform.position, -pointVelocity.normalized * Mathf.Min(a, num * connectedMass) / 100f, Color.red);
			}
		}
	}

	private void LateUpdate()
	{
		if (!(mesh == null) && !body.IsSleeping())
		{
			Vector3 vector = mesh.parent.InverseTransformVector(body.GetPointVelocity(base.transform.position));
			vector -= Vector3.Project(vector, upAxis);
			Quaternion b = Quaternion.FromToRotation(forwardAxis, vector);
			Quaternion localRotation = Quaternion.Lerp(mesh.localRotation, b, vector.magnitude / 10f);
			mesh.localRotation = localRotation;
		}
	}
}
