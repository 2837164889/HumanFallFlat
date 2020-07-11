using UnityEngine;

public class SignalTweenPosition : SignalTweenBase
{
	public Vector3 offPos;

	public Vector3 onPos;

	public bool relative;

	private Vector3 initialPos;

	private Rigidbody body;

	protected override void OnEnable()
	{
		initialPos = base.transform.localPosition;
		body = GetComponent<Rigidbody>();
		base.OnEnable();
	}

	public override void OnValueChanged(float value)
	{
		base.OnValueChanged(value);
		Vector3 vector = Vector3.Lerp(offPos, onPos, value);
		if (relative)
		{
			vector += initialPos;
		}
		if (body != null)
		{
			body.MovePosition(base.transform.parent.TransformPoint(vector));
		}
		else
		{
			base.transform.localPosition = vector;
		}
	}
}
