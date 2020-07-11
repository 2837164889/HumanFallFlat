using HumanAPI;
using System.Collections;
using UnityEngine;

public class ExitLatch : Node, IReset
{
	public NodeInput input;

	public Rigidbody left;

	public Rigidbody right;

	public Rigidbody portcullis;

	public AudioSource doorReleaseSample;

	public Sound2 doorReleaseSound;

	public override void Process()
	{
		base.Process();
		if (input.value > 0.5f)
		{
			StartCoroutine(TheEnd());
		}
	}

	private IEnumerator TheEnd()
	{
		portcullis.isKinematic = false;
		portcullis.WakeUp();
		yield return new WaitForSeconds(2f);
		Object.Destroy(left.GetComponent<FixedJoint>());
		Object.Destroy(right.GetComponent<FixedJoint>());
		if (doorReleaseSound != null)
		{
			doorReleaseSound.Play();
		}
		else
		{
			doorReleaseSample.Play();
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		StopAllCoroutines();
		portcullis.isKinematic = true;
	}
}
