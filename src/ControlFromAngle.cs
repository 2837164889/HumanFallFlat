using HumanAPI;
using System;
using UnityEngine;

public class ControlFromAngle : Node, IReset
{
	public enum Axis
	{
		X,
		Y,
		Z
	}

	public NodeOutput output;

	public GameObject target;

	[Tooltip("A Flag to say whether to use the information for location coming from a parent gameobject")]
	public bool useParentLocation;

	private Vector3 newPosition;

	public Axis axis;

	public float rangeRotations = 1f;

	public float currentAngle;

	public float currentValue;

	private float originalValue;

	private float originalAngle;

	private Rigidbody body;

	public bool allowNegative;

	public bool clampFullRotation = true;

	public bool CalculateAngleCorrectly;

	public bool ResetToZeroOnRestart;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private bool justReset;

	protected override void OnEnable()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Enabled ");
		}
		base.OnEnable();
		body = GetComponent<Rigidbody>();
		originalAngle = (currentAngle = FromEuler(base.transform));
		originalValue = currentValue;
	}

	private void FixedUpdate()
	{
		float num = FromEuler(base.transform);
		if (justReset)
		{
			output.SetValue(0f);
			currentAngle = num;
			justReset = false;
			return;
		}
		float num2 = num - currentAngle;
		currentAngle = num;
		for (; num2 < -180f; num2 += 360f)
		{
		}
		while (num2 > 180f)
		{
			num2 -= 360f;
		}
		if (num2 == 0f)
		{
			return;
		}
		currentValue += num2 / 360f / rangeRotations;
		float num3 = (!allowNegative) ? 0f : (-1f);
		if (currentValue < num3)
		{
			if (clampFullRotation)
			{
				if (body.angularVelocity.magnitude > 0.01f)
				{
					body.angularVelocity = Vector3.zero;
					body.MoveRotation(base.transform.parent.rotation * Quaternion.Euler(ToEuler((0f - currentValue) * 360f * rangeRotations)) * base.transform.localRotation);
				}
				currentValue = num3;
			}
			else
			{
				currentValue = 1f - (num3 - currentValue);
			}
		}
		if (currentValue > 1f)
		{
			if (clampFullRotation)
			{
				if (body.angularVelocity.magnitude > 0.01f)
				{
					body.angularVelocity = Vector3.zero;
					body.MoveRotation(base.transform.parent.rotation * Quaternion.Euler(ToEuler((1f - currentValue) * 360f * rangeRotations)) * base.transform.localRotation);
				}
				currentValue = 1f;
			}
			else
			{
				currentValue = ((!allowNegative) ? 0f : (-1f)) + (currentValue - 1f);
			}
		}
		if (target != null)
		{
			target.GetComponent<IControllable>()?.SetControlValue(currentValue);
		}
		output.SetValue(currentValue);
	}

	private float FromEuler(Transform transform)
	{
		if (CalculateAngleCorrectly)
		{
			switch (axis)
			{
			case Axis.X:
			{
				Vector3 forward = transform.forward;
				float y = forward.y;
				Vector3 forward2 = transform.forward;
				return Mathf.Atan2(y, forward2.z) * 57.29578f;
			}
			case Axis.Y:
			case Axis.Z:
				throw new InvalidOperationException();
			}
		}
		else
		{
			Vector3 eulerAngles = transform.localRotation.eulerAngles;
			switch (axis)
			{
			case Axis.X:
				return eulerAngles.x;
			case Axis.Y:
				return eulerAngles.y;
			case Axis.Z:
				return eulerAngles.z;
			}
		}
		throw new InvalidOperationException();
	}

	private Vector3 ToEuler(float angle)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " ToEuler ");
		}
		switch (axis)
		{
		case Axis.X:
			return new Vector3(angle, 0f, 0f);
		case Axis.Y:
			return new Vector3(0f, angle);
		case Axis.Z:
			return new Vector3(0f, 0f, angle);
		default:
			throw new InvalidOperationException();
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " ResetState ");
		}
		currentAngle = originalAngle;
		currentValue = originalValue;
		if (ResetToZeroOnRestart && checkpoint == 0)
		{
			justReset = true;
		}
	}
}
