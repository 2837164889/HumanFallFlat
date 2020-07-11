using UnityEngine;

public class SlidingAnchor : MonoBehaviour
{
	public Transform anchor;

	public Vector3 direction;

	private Vector3 connectedDirection;

	private Joint joint;

	private Vector3 offset;

	private void Start()
	{
		joint = GetComponent<Joint>();
		offset = joint.connectedBody.transform.InverseTransformPoint(anchor.position) - Vector3.up * (base.transform.position - anchor.position).magnitude;
		connectedDirection = joint.connectedBody.transform.InverseTransformDirection(Vector3.up);
	}

	private void FixedUpdate()
	{
		float magnitude = (base.transform.position - anchor.position).magnitude;
		joint.autoConfigureConnectedAnchor = false;
		joint.anchor = direction * magnitude;
		joint.connectedAnchor = offset + connectedDirection * magnitude;
	}
}
