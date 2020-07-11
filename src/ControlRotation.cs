using UnityEngine;

public class ControlRotation : MonoBehaviour, IControllable
{
	public Vector3 axis;

	public float min;

	public float max;

	private Rigidbody body;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
	}

	public void SetControlValue(float v)
	{
		body.MoveRotation(base.transform.parent.rotation * Quaternion.AngleAxis(Mathf.Lerp(min, max, v), axis));
	}
}
