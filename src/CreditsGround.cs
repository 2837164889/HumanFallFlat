using Multiplayer;
using UnityEngine;

public class CreditsGround : MonoBehaviour, IReset
{
	public Rigidbody left;

	public Rigidbody right;

	public AudioSource doorReleaseSample;

	private Vector3 oldPosL;

	private Vector3 oldPosR;

	private Quaternion oldRotL;

	private Quaternion oldRotR;

	public bool isOpen
	{
		get;
		protected set;
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!NetGame.isClient)
		{
			ChangeState(newState: true);
		}
	}

	public void ResetState(int cp, int subObjective)
	{
		ChangeState(newState: false);
	}

	public void ChangeState(bool newState)
	{
		if (isOpen != newState)
		{
			isOpen = newState;
			if (newState)
			{
				oldPosL = left.position;
				oldPosR = right.position;
				oldRotL = left.rotation;
				oldRotR = right.rotation;
				left.isKinematic = false;
				right.isKinematic = false;
				left.GetComponent<NetBody>().isKinematic = false;
				right.GetComponent<NetBody>().isKinematic = false;
				doorReleaseSample.Play();
			}
			else
			{
				left.isKinematic = true;
				right.isKinematic = true;
				left.GetComponent<NetBody>().isKinematic = true;
				right.GetComponent<NetBody>().isKinematic = true;
			}
		}
	}
}
