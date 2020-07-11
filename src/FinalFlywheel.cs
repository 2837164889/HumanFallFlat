using HumanAPI;
using UnityEngine;

public class FinalFlywheel : Node, IReset
{
	public NodeInput input;

	public NodeOutput currentVelocity;

	public NodeOutput disconnected;

	public float disconnectForce;

	public float accMultiplier = 0.1f;

	public float deAccMultiplier = 0.1f;

	private ConfigurableJoint joint;

	private Rigidbody rb;

	private float targetVelocity;

	public Vector3 rotateDirection = Vector3.right;

	private void Awake()
	{
		joint = GetComponent<ConfigurableJoint>();
		rb = GetComponent<Rigidbody>();
		Rigidbody rigidbody = rb;
		Vector3 zero = Vector3.zero;
		rb.velocity = zero;
		rigidbody.angularVelocity = zero;
	}

	private void FixedUpdate()
	{
		if (disconnected.value > 0f)
		{
			targetVelocity = Mathf.Lerp(targetVelocity, 0f, Time.fixedDeltaTime * deAccMultiplier);
		}
		else
		{
			targetVelocity = Mathf.Lerp(targetVelocity, input.value, Time.fixedDeltaTime * accMultiplier);
		}
		if (targetVelocity != currentVelocity.value)
		{
			currentVelocity.SetValue(targetVelocity);
		}
		rb.angularVelocity = rotateDirection * targetVelocity;
		if (targetVelocity > disconnectForce)
		{
			DisconnectJoint();
		}
	}

	private void DisconnectJoint()
	{
		if (!(disconnected.value > 0f))
		{
			disconnected.SetValue(1f);
			ConfigurableJoint configurableJoint = joint;
			ConfigurableJointMotion configurableJointMotion = ConfigurableJointMotion.Free;
			joint.xMotion = configurableJointMotion;
			configurableJointMotion = configurableJointMotion;
			joint.yMotion = configurableJointMotion;
			configurableJoint.zMotion = configurableJointMotion;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Debug.DrawLine(base.transform.position, base.transform.position + rotateDirection);
	}

	void IReset.ResetState(int checkpoint, int subObjectives)
	{
		ConfigurableJoint configurableJoint = joint;
		ConfigurableJointMotion configurableJointMotion = ConfigurableJointMotion.Locked;
		joint.xMotion = configurableJointMotion;
		configurableJointMotion = configurableJointMotion;
		joint.yMotion = configurableJointMotion;
		configurableJoint.zMotion = configurableJointMotion;
		disconnected.SetValue(0f);
		targetVelocity = 0f;
	}
}
