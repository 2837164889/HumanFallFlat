using UnityEngine;

public class ControlJointLimit : MonoBehaviour, IControllable
{
	public float min;

	public float max = 1f;

	private ConfigurableJoint joint;

	private Rigidbody body;

	private void OnEnable()
	{
		joint = GetComponent<ConfigurableJoint>();
		body = GetComponent<Rigidbody>();
	}

	public void SetControlValue(float v)
	{
		if (body.IsSleeping())
		{
			body.WakeUp();
		}
		if (joint != null)
		{
			joint.linearLimit = new SoftJointLimit
			{
				limit = Mathf.Lerp(min, max, v)
			};
		}
	}
}
