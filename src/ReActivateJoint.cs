using UnityEngine;

public class ReActivateJoint : MonoBehaviour, IReset
{
	[SerializeField]
	private GameObject iceBlock;

	[SerializeField]
	private GameObject torch;

	void IReset.ResetState(int checkpoint, int subObjectives)
	{
		ConfigurableJoint component = GetComponent<ConfigurableJoint>();
		if ((bool)component)
		{
			Object.Destroy(component);
		}
		component = torch.AddComponent<ConfigurableJoint>();
		component.connectedBody = iceBlock.GetComponent<Rigidbody>();
		component.xMotion = ConfigurableJointMotion.Locked;
		component.yMotion = ConfigurableJointMotion.Locked;
		component.zMotion = ConfigurableJointMotion.Locked;
		component.angularXMotion = ConfigurableJointMotion.Locked;
		component.angularYMotion = ConfigurableJointMotion.Locked;
		component.angularZMotion = ConfigurableJointMotion.Locked;
		component.massScale = 1.4f;
	}
}
