using UnityEngine;

public class ControlPositionSpline : MonoBehaviour, IControllable
{
	public SplineComponent spline;

	private Rigidbody body;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
	}

	public void SetControlValue(float v)
	{
		body.MovePosition(spline.GetPoint(v));
	}
}
