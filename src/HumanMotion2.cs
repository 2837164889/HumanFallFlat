using UnityEngine;

public class HumanMotion2 : MonoBehaviour
{
	private Human human;

	private Ragdoll ragdoll;

	public LayerMask grabLayers;

	public TorsoMuscles torso;

	public LegMuscles legs;

	public HandMuscles hands;

	public void Initialize()
	{
		if (!(human != null))
		{
			human = GetComponent<Human>();
			ragdoll = human.ragdoll;
			torso = new TorsoMuscles(human, ragdoll, this);
			legs = new LegMuscles(human, ragdoll, this);
			hands = new HandMuscles(human, ragdoll, this);
		}
	}

	public void OnFixedUpdate()
	{
		hands.OnFixedUpdate();
		torso.OnFixedUpdate();
		legs.OnFixedUpdate(torso.feedbackForce);
	}

	public static void AlignLook(HumanSegment segment, Quaternion targetRotation, float accelerationSpring, float damping)
	{
		(targetRotation * Quaternion.Inverse(segment.transform.rotation)).ToAngleAxis(out float angle, out Vector3 axis);
		if (angle > 180f)
		{
			angle -= 360f;
		}
		if (angle < -180f)
		{
			angle += 360f;
		}
		segment.rigidbody.SafeAddTorque(axis * angle * accelerationSpring - segment.rigidbody.angularVelocity * damping, ForceMode.Acceleration);
	}

	public static void AlignToVector(Rigidbody body, Vector3 alignmentVector, Vector3 targetVector, float spring)
	{
		AlignToVector(body, alignmentVector, targetVector, spring, spring);
	}

	public static void AlignToVector(Rigidbody body, Vector3 alignmentVector, Vector3 targetVector, float spring, float maxTorque)
	{
		float num = 0.1f;
		Vector3 vector = Vector3.Cross((Quaternion.AngleAxis(body.angularVelocity.magnitude * 57.29578f * num, body.angularVelocity) * alignmentVector.normalized).normalized, targetVector.normalized);
		Vector3 a = vector.normalized * Mathf.Asin(Mathf.Clamp01(vector.magnitude));
		Vector3 vector2 = spring * a;
		body.SafeAddTorque(Vector3.ClampMagnitude(vector2, maxTorque));
	}

	public static void AlignToVector(HumanSegment part, Vector3 alignmentVector, Vector3 targetVector, float spring)
	{
		AlignToVector(part.rigidbody, alignmentVector, targetVector, spring);
	}
}
