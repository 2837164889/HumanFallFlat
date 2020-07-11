using UnityEngine;

public class StandingRagdoll : MonoBehaviour
{
	public float standForce = 1000f;

	private Ragdoll ragdoll;

	private void OnEnable()
	{
		ragdoll = GetComponent<Ragdoll>();
	}

	private void FixedUpdate()
	{
		if (Human.instance.grabManager.hasGrabbed)
		{
			if (Human.instance.controls.walkSpeed > 0f)
			{
				standForce -= Time.fixedDeltaTime * 300f;
			}
		}
		else if (standForce < 1000f)
		{
			standForce = 0f;
		}
		if (standForce != 0f)
		{
			ragdoll.partHead.rigidbody.SafeAddForce(Vector3.up * standForce);
			if ((Human.instance.transform.position - base.transform.position).magnitude < 3f)
			{
				ragdoll.partRightHand.rigidbody.SafeAddForce(new Vector3(-0.2f, 1f, 0f) * standForce * 0.05f);
			}
			ragdoll.partLeftFoot.rigidbody.SafeAddForce(-Vector3.up * standForce / 2f);
			ragdoll.partRightFoot.rigidbody.SafeAddForce(-Vector3.up * standForce / 2f);
		}
	}
}
