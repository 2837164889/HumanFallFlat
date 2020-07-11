using HumanAPI;
using UnityEngine;

public sealed class LadderReset : MonoBehaviour, IPostReset
{
	public class JointList
	{
		public GameObject jointObject;

		public Rigidbody connectedRigidBody;

		public float breakForce;

		public JointList(GameObject jointObject, Rigidbody connectedRigidBody, float breakForce)
		{
			this.jointObject = jointObject;
			this.connectedRigidBody = connectedRigidBody;
			this.breakForce = breakForce;
		}
	}

	private JointList[] jointLists;

	private void Awake()
	{
		FixedJoint[] componentsInChildren = GetComponentsInChildren<FixedJoint>();
		jointLists = new JointList[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			FixedJoint fixedJoint = componentsInChildren[i];
			jointLists[i] = new JointList(fixedJoint.gameObject, fixedJoint.connectedBody, fixedJoint.breakForce);
		}
	}

	void IPostReset.PostResetState(int checkpoint)
	{
		JointList[] array = jointLists;
		foreach (JointList jointList in array)
		{
			if (jointList.jointObject.GetComponent<FixedJoint>() == null)
			{
				FixedJoint fixedJoint = jointList.jointObject.AddComponent<FixedJoint>();
				fixedJoint.connectedBody = jointList.connectedRigidBody;
				fixedJoint.breakForce = jointList.breakForce;
			}
		}
	}
}
