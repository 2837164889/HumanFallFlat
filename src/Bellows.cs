using HumanAPI;
using UnityEngine;

public class Bellows : Node
{
	public NodeOutput output;

	public float velocityOutputScale;

	public Rigidbody topBellow;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		NodeOutput nodeOutput = output;
		Vector3 angularVelocity = topBellow.angularVelocity;
		nodeOutput.SetValue(Mathf.Max(angularVelocity.x * velocityOutputScale, 0f));
	}
}
