using HumanAPI;
using System.Collections;
using UnityEngine;

public class WaterwheelCheckpoint : Checkpoint
{
	public Breakable damLog;

	public Transform fenceSupport;

	public override void OnTriggerEnter(Collider other)
	{
		if (damLog.broken.value > 0.5f)
		{
			base.OnTriggerEnter(other);
		}
	}

	public override void LoadHere()
	{
		base.LoadHere();
		StartCoroutine(BreakFence());
	}

	private IEnumerator BreakFence()
	{
		yield return null;
		damLog.Shatter();
		Vector3 pos = fenceSupport.position + new Vector3(0f, 0f, -0.5f);
		while ((fenceSupport.position - pos).magnitude < 0.2f)
		{
			fenceSupport.GetComponent<Rigidbody>().MovePosition(pos);
			yield return null;
		}
	}
}
