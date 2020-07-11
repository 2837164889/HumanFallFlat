using HumanAPI;
using UnityEngine;

public class ResetChainLink : MonoBehaviour, IPostReset
{
	[SerializeField]
	private GameObject connectedBelow;

	private Rigidbody connectedBody;

	private ConfigurableJointMotion xMotion;

	private ConfigurableJointMotion yMotion;

	private ConfigurableJointMotion zMotion;

	private SoftJointLimit linearLimit;

	private SoftJointLimit lowAngularXLimit;

	private SoftJointLimit highAngularXLimit;

	private SoftJointLimit angularYLimit;

	private SoftJointLimit angularZLimit;

	private JointDrive angularXDrive;

	private JointDrive angularYZDrive;

	private JointDrive slerpDrive;

	private float breakForce;

	private float breakTorque;

	private bool noJoint;

	private void Awake()
	{
		ConfigurableJoint component = GetComponent<ConfigurableJoint>();
		if ((bool)component)
		{
			connectedBody = component.connectedBody;
			xMotion = component.xMotion;
			yMotion = component.yMotion;
			zMotion = component.zMotion;
			lowAngularXLimit = component.lowAngularXLimit;
			highAngularXLimit = component.highAngularXLimit;
			angularYLimit = component.angularYLimit;
			angularZLimit = component.angularZLimit;
			angularXDrive = component.angularXDrive;
			angularYZDrive = component.angularYZDrive;
			slerpDrive = component.slerpDrive;
			linearLimit = component.linearLimit;
			breakForce = component.breakForce;
			breakTorque = component.breakTorque;
			if (connectedBelow != null)
			{
				component = connectedBelow.GetComponent<ConfigurableJoint>();
				if (component != null)
				{
					Rigidbody rigidbody = component.connectedBody = GetComponent<Rigidbody>();
				}
			}
		}
		else
		{
			noJoint = true;
		}
	}

	void IPostReset.PostResetState(int checkpoint)
	{
		if (!noJoint)
		{
			ConfigurableJoint component = GetComponent<ConfigurableJoint>();
			if (component == null)
			{
				component = base.gameObject.AddComponent<ConfigurableJoint>();
				component.connectedBody = connectedBody;
				component.anchor = Vector3.zero;
				component.xMotion = xMotion;
				component.yMotion = yMotion;
				component.zMotion = zMotion;
				component.linearLimit = linearLimit;
				component.lowAngularXLimit = lowAngularXLimit;
				component.highAngularXLimit = highAngularXLimit;
				component.angularYLimit = angularYLimit;
				component.angularZLimit = angularZLimit;
				component.angularXDrive = angularXDrive;
				component.angularYZDrive = angularYZDrive;
				component.slerpDrive = slerpDrive;
				component.breakForce = breakForce;
				component.breakTorque = breakTorque;
			}
		}
	}
}
