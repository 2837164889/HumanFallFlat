using UnityEngine;

public class FixFixedJoint : MonoBehaviour
{
	public void ReAddJoint()
	{
		FixedJoint component = GetComponent<FixedJoint>();
		if (component == null)
		{
			component = base.gameObject.AddComponent<FixedJoint>();
			component.breakForce = float.PositiveInfinity;
			component.breakTorque = float.PositiveInfinity;
			component.enableCollision = false;
			component.massScale = 1f;
			component.connectedMassScale = 1f;
			component.enablePreprocessing = true;
		}
	}

	public void MakeJointBreakable()
	{
		FixedJoint component = GetComponent<FixedJoint>();
		if (component != null)
		{
			component.breakForce = 0f;
			component.breakTorque = 0f;
		}
	}
}
