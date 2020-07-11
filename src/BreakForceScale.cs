using UnityEngine;

public class BreakForceScale : MonoBehaviour
{
	public Joint joint;

	public float scaleFactor = 0.1f;

	public Rigidbody contactBody;

	private int contacts;

	private float initialBreakForce;

	private void Start()
	{
		initialBreakForce = joint.breakForce;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(joint == null) && collision.rigidbody == contactBody)
		{
			contacts++;
			if (contacts == 1)
			{
				joint.breakForce = initialBreakForce * scaleFactor;
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (!(joint == null) && collision.rigidbody == contactBody)
		{
			contacts--;
			if (contacts == 0)
			{
				joint.breakForce = initialBreakForce;
			}
		}
	}
}
