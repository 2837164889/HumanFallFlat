using UnityEngine;

public class OfflineShatter : ShatterBase
{
	public GameObject shardRoot;

	public float minExplodeImpulse;

	public float maxExplodeImpulse = float.PositiveInfinity;

	public float perShardImpulseFraction = 0.25f;

	public float maxShardVelocity = float.PositiveInfinity;

	protected override void Shatter(Vector3 contactPoint, Vector3 adjustedImpulse, float impactMagnitude, uint seed, uint netId)
	{
		base.Shatter(contactPoint, adjustedImpulse, impactMagnitude, seed, netId);
		shardRoot.SetActive(value: true);
		Vector3 normalized = adjustedImpulse.normalized;
		float value = Mathf.Clamp(adjustedImpulse.magnitude, minExplodeImpulse, maxExplodeImpulse) * perShardImpulseFraction;
		for (int i = 0; i < shardRoot.transform.childCount; i++)
		{
			Rigidbody component = shardRoot.transform.GetChild(i).GetComponent<Rigidbody>();
			Vector3 vector = (component.position - contactPoint) * 10f;
			float d = Mathf.Clamp(value, 0f, maxShardVelocity * component.mass);
			component.SafeAddForceAtPosition(-normalized * d, (3f * contactPoint + component.position) / 4f, ForceMode.Impulse);
		}
	}

	public override void ResetState(int checkpoint, int subObjectives)
	{
		base.ResetState(checkpoint, subObjectives);
		shardRoot.SetActive(value: false);
	}
}
