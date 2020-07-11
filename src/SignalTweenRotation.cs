using UnityEngine;

public class SignalTweenRotation : SignalTweenBase
{
	public Vector3 offRot;

	public Vector3 onRot;

	public bool relative;

	public bool eulerInterpolate;

	private Quaternion initialRot;

	private Rigidbody body;

	protected override void OnEnable()
	{
		initialRot = base.transform.localRotation;
		body = GetComponent<Rigidbody>();
		base.OnEnable();
	}

	public override void OnValueChanged(float value)
	{
		base.OnValueChanged(value);
		Quaternion quaternion = (!eulerInterpolate) ? Quaternion.Lerp(Quaternion.Euler(offRot), Quaternion.Euler(onRot), value) : Quaternion.Euler(Vector3.Lerp(offRot, onRot, value));
		if (relative)
		{
			quaternion = initialRot * quaternion;
		}
		if (body != null)
		{
			body.MoveRotation(base.transform.parent.rotation * quaternion);
		}
		else
		{
			base.transform.localRotation = quaternion;
		}
	}
}
