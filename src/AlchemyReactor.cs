using HumanAPI;
using UnityEngine;

public class AlchemyReactor : Node
{
	public NodeInput input;

	public NodeOutput output;

	public ReactorDoor door;

	public float outputPressureScale = 1.1f;

	public float pressureDropPerSecond;

	private float pressure;

	private void FixedUpdate()
	{
		pressure += input.value;
	}

	private void Update()
	{
		if (door != null && !door.IsClosed())
		{
			pressure = 0f;
		}
		pressure -= pressureDropPerSecond * Time.deltaTime;
		pressure = Mathf.Clamp(pressure, 0f, 1f);
		output.SetValue(pressure * outputPressureScale);
	}
}
