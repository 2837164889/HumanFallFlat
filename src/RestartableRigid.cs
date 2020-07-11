using UnityEngine;

public class RestartableRigid : MonoBehaviour, IReset
{
	private struct RigidState
	{
		public Rigidbody rigid;

		public Vector3 position;

		public Quaternion rotation;

		public bool recorded;

		public Vector3 recordedPosition;

		public Quaternion recordedRotation;
	}

	private struct JointState
	{
		private bool valid;

		private bool isHinge;

		public Joint joint;

		public Rigidbody rigid;

		public Vector3 anchor;

		public Vector3 axis;

		public float breakForce;

		public float breakTorque;

		public Vector3 connectedAnchor;

		public Rigidbody connectedBody;

		public bool enableCollision;

		public bool enablePreprocessing;

		public JointLimits limits;

		public JointMotor motor;

		public JointSpring spring;

		public bool useLimits;

		public bool useMotor;

		public bool useSpring;

		public static JointState FromJoint(Joint joint)
		{
			JointState jointState = default(JointState);
			jointState.joint = joint;
			jointState.rigid = joint.GetComponent<Rigidbody>();
			jointState.anchor = joint.anchor;
			jointState.axis = joint.axis;
			jointState.breakForce = joint.breakForce;
			jointState.breakTorque = joint.breakTorque;
			jointState.connectedAnchor = joint.connectedAnchor;
			jointState.connectedBody = joint.connectedBody;
			jointState.enableCollision = joint.enableCollision;
			jointState.enablePreprocessing = joint.enablePreprocessing;
			JointState result = jointState;
			HingeJoint hingeJoint = joint as HingeJoint;
			if (hingeJoint != null)
			{
				result.isHinge = true;
				result.limits = hingeJoint.limits;
				result.motor = hingeJoint.motor;
				result.spring = hingeJoint.spring;
				result.useLimits = hingeJoint.useLimits;
				result.useMotor = hingeJoint.useMotor;
				result.useSpring = hingeJoint.useSpring;
				result.valid = true;
			}
			else if (joint is FixedJoint)
			{
				result.isHinge = false;
				result.valid = true;
			}
			return result;
		}

		public Joint RecreateJoint()
		{
			if (!valid)
			{
				return null;
			}
			if (joint == null)
			{
				HingeJoint hingeJoint = null;
				if (isHinge)
				{
					hingeJoint = (HingeJoint)(joint = rigid.gameObject.AddComponent<HingeJoint>());
				}
				else
				{
					joint = rigid.gameObject.AddComponent<FixedJoint>();
				}
				joint.autoConfigureConnectedAnchor = false;
				joint.connectedBody = connectedBody;
				joint.anchor = anchor;
				joint.axis = axis;
				joint.breakForce = breakForce;
				joint.breakTorque = breakTorque;
				joint.connectedAnchor = connectedAnchor;
				joint.enableCollision = enableCollision;
				joint.enablePreprocessing = enablePreprocessing;
				if (hingeJoint != null)
				{
					hingeJoint.limits = limits;
					hingeJoint.motor = motor;
					hingeJoint.spring = spring;
					hingeJoint.useLimits = useLimits;
					hingeJoint.useMotor = useMotor;
					hingeJoint.useSpring = useSpring;
				}
			}
			return joint;
		}
	}

	private RigidState[] initialState;

	private JointState[] jointState;

	private void OnEnable()
	{
	}

	public void Reset(Vector3 offset)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		Reset(Vector3.zero);
	}

	private string GetGameObjectPath(Transform transform)
	{
		string text = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return text;
	}

	public void SetRecordedInfo(string rigidBodyName, Vector3 pos, Quaternion rot)
	{
		if (initialState == null)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < initialState.Length)
			{
				string gameObjectPath = GetGameObjectPath(initialState[num].rigid.transform);
				if (gameObjectPath == rigidBodyName)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		RigidState rigidState = initialState[num];
		rigidState.recorded = true;
		rigidState.recordedPosition = pos;
		rigidState.recordedRotation = rot;
		initialState[num] = rigidState;
		rigidState.rigid.MovePosition(pos);
		rigidState.rigid.MoveRotation(rot);
		rigidState.rigid.transform.position = pos;
		rigidState.rigid.transform.rotation = rot;
		Rigidbody rigid = rigidState.rigid;
		Vector3 zero = Vector3.zero;
		rigidState.rigid.velocity = zero;
		rigid.angularVelocity = zero;
		rigidState.rigid.Sleep();
	}
}
