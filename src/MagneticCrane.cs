using HumanAPI;
using UnityEngine;

public class MagneticCrane : Node, IReset
{
	public NodeInput activateMagnet;

	private bool magnetActive;

	private Joint fixJoint;

	public override void Process()
	{
		magnetActive = (Mathf.Abs(activateMagnet.value) >= 0.5f);
		if (!magnetActive && fixJoint != null)
		{
			Object.Destroy(fixJoint);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (magnetActive && fixJoint == null && collision.gameObject.GetComponent<MagneticCraneTarget>() != null)
		{
			collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			fixJoint = collision.gameObject.AddComponent<FixedJoint>();
			fixJoint.connectedBody = GetComponent<Rigidbody>();
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (fixJoint != null)
		{
			Object.Destroy(fixJoint);
		}
	}
}
