using HumanAPI;
using UnityEngine;

public sealed class SnowManReset : MonoBehaviour, IPostEndReset
{
	public sealed class JointStoreData
	{
		public Rigidbody connectedBody;

		public Vector3 scale;

		public float breakForce;

		public float breakTorque;

		public float myMass;
	}

	private JointStoreData[] jointStore;

	private void Awake()
	{
		int childCount = base.transform.childCount;
		jointStore = new JointStoreData[childCount];
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			FixedJoint component = gameObject.GetComponent<FixedJoint>();
			JointStoreData jointStoreData = new JointStoreData();
			if (component != null)
			{
				jointStoreData.connectedBody = component.connectedBody;
				jointStoreData.breakForce = component.breakForce;
				jointStoreData.breakTorque = component.breakTorque;
			}
			jointStoreData.scale = component.transform.localScale;
			Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
			jointStoreData.myMass = component2.mass;
			jointStore[i] = jointStoreData;
		}
	}

	void IPostEndReset.PostEndResetState(int checkpoint)
	{
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			FixedJoint component = gameObject.GetComponent<FixedJoint>();
			if (component == null)
			{
				FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
				fixedJoint.connectedBody = jointStore[i].connectedBody;
				fixedJoint.breakForce = jointStore[i].breakForce;
				fixedJoint.breakTorque = jointStore[i].breakTorque;
			}
			gameObject.transform.localScale = jointStore[i].scale;
			Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
			component2.mass = jointStore[i].myMass;
		}
	}
}
