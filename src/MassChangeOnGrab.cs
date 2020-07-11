using Multiplayer;
using UnityEngine;

public class MassChangeOnGrab : MonoBehaviour, IGrabbable
{
	public float massMultiplyOnGrab = 1f;

	public Rigidbody rigid;

	private float mass;

	private void OnEnable()
	{
		if (!NetGame.isClient)
		{
			if (rigid == null)
			{
				rigid = GetComponent<Rigidbody>();
			}
			mass = rigid.mass;
		}
	}

	public void OnGrab()
	{
		if (!NetGame.isClient)
		{
			rigid.mass = mass * massMultiplyOnGrab;
		}
	}

	public void OnRelease()
	{
		if (!NetGame.isClient)
		{
			rigid.mass = mass;
		}
	}
}
