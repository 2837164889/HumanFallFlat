using UnityEngine;

public class RotateWhenStepped : MonoBehaviour
{
	public Vector3 rotationNotStepped;

	public Vector3 rotationStepped;

	private ConfigurableJoint joint;

	private void OnEnable()
	{
		joint = GetComponent<ConfigurableJoint>();
	}

	private void Update()
	{
		if (GroundManager.IsStandingAny(base.gameObject))
		{
			joint.targetRotation = Quaternion.Euler(rotationStepped);
		}
		else
		{
			joint.targetRotation = Quaternion.Euler(rotationNotStepped);
		}
	}
}
