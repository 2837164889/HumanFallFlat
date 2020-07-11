using HumanAPI;
using UnityEngine;

public class SignalToForce : Node, IReset
{
	public NodeInput input;

	public bool oneShot = true;

	private bool appliedForce;

	public Rigidbody body;

	public Vector3 forcePosition;

	public Vector3 forceToApply;

	private void Start()
	{
	}

	private void Update()
	{
		if (body != null && (double)Mathf.Abs(input.value) >= 0.5 && (!oneShot || !appliedForce))
		{
			body.AddForceAtPosition(forceToApply, forcePosition);
			appliedForce = true;
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		appliedForce = false;
	}
}
