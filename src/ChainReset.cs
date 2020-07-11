using HumanAPI;
using UnityEngine;

public class ChainReset : MonoBehaviour, IPostReset
{
	public Rigidbody connectedTo;

	private ConfigurableJoint currentJoint;

	private Vector3 originalAnchor;

	private SoftJointLimit originalZLimit;

	private void Awake()
	{
		currentJoint = GetComponent<ConfigurableJoint>();
		originalAnchor = currentJoint.connectedAnchor;
		originalZLimit = currentJoint.angularZLimit;
	}

	void IPostReset.PostResetState(int checkpoint)
	{
		Object.DestroyImmediate(currentJoint);
		currentJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		currentJoint.autoConfigureConnectedAnchor = false;
		currentJoint.connectedAnchor = originalAnchor;
		currentJoint.connectedBody = connectedTo;
		currentJoint.xMotion = ConfigurableJointMotion.Locked;
		currentJoint.yMotion = ConfigurableJointMotion.Locked;
		currentJoint.zMotion = ConfigurableJointMotion.Locked;
		currentJoint.angularXMotion = ConfigurableJointMotion.Free;
		currentJoint.angularYMotion = ConfigurableJointMotion.Free;
		currentJoint.angularZMotion = ConfigurableJointMotion.Limited;
		currentJoint.angularZLimit = originalZLimit;
	}
}
