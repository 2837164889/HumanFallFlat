using UnityEngine;

public class SignalOnDisplace : SignalBase
{
	public float breakDiscplacement = 0.5f;

	private Vector3 startingPos;

	private void Start()
	{
		startingPos = GetComponent<Rigidbody>().position;
	}

	private void Update()
	{
		Vector3 position = GetComponent<Rigidbody>().position;
		if ((position - startingPos).magnitude > breakDiscplacement)
		{
			SetValue(1f);
		}
	}
}
