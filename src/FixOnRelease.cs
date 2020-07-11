using UnityEngine;

public class FixOnRelease : MonoBehaviour, IGrabbable
{
	public Rigidbody lockParent;

	public FixedJoint joint;

	public void OnGrab()
	{
		if (joint != null)
		{
			Object.Destroy(joint);
		}
		joint = null;
	}

	public void OnRelease()
	{
		joint = base.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = lockParent;
	}

	private void Start()
	{
		joint = base.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = lockParent;
	}
}
