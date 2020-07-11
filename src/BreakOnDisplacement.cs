using HumanAPI;
using UnityEngine;

public class BreakOnDisplacement : Node
{
	public float breakDiscplacement = 0.5f;

	public NodeInput startChecking;

	public NodeOutput broken;

	private bool areWeBroken;

	[Tooltip("Use ths in order to print debug info to the Log")]
	public bool showDebug;

	private Vector3 startingPos;

	private void Start()
	{
		startingPos = GetComponent<Rigidbody>().position;
	}

	private void Update()
	{
		if (!areWeBroken)
		{
			return;
		}
		Vector3 position = GetComponent<Rigidbody>().position;
		if ((position - startingPos).magnitude > breakDiscplacement)
		{
			Joint component = GetComponent<Joint>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			broken.SetValue(1f);
		}
	}

	public override void Process()
	{
		if (startChecking.value > 0.5f)
		{
			areWeBroken = true;
		}
	}
}
