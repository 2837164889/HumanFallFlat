using System;
using UnityEngine;

public static class ConstantAccelerationControl
{
	private static float aOff;

	private static float aOn;

	private const float BigTime = 10000f;

	public static Vector3 Solve(Vector3 offset, Vector3 velocity, float maxAcceleration, float deadZone)
	{
		if (offset == Vector3.zero)
		{
			return Vector3.ClampMagnitude(-velocity / Time.fixedDeltaTime, maxAcceleration);
		}
		if (deadZone == 0f)
		{
			throw new ArgumentException("deadZone can't be 0", "deadZone");
		}
		if (maxAcceleration == 0f)
		{
			throw new ArgumentException("maxAcceleration can't be 0", "maxAcceleration");
		}
		float magnitude = offset.magnitude;
		float num = Vector3.Dot(offset.normalized, velocity);
		Vector3 vector = velocity - num * offset.normalized;
		float magnitude2 = vector.magnitude;
		maxAcceleration *= Mathf.Lerp(0f, 1f, offset.magnitude / deadZone);
		float num2 = velocity.magnitude / maxAcceleration;
		for (int i = 0; i < 5; i++)
		{
			if (num2 == 0f || magnitude2 == 0f)
			{
				aOff = 0f;
			}
			else if (magnitude2 < maxAcceleration * num2)
			{
				aOff = (0f - magnitude2) / num2;
			}
			else
			{
				aOff = AccelerationFromTime(magnitude2, 0f, num2);
			}
			aOff = Mathf.Clamp(aOff, (0f - maxAcceleration) * 0.99f, maxAcceleration * 0.99f);
			aOn = Mathf.Sqrt(maxAcceleration * maxAcceleration - aOff * aOff);
			float num3 = TimeFromAcceleration(num, magnitude, aOn);
			float num4 = TimeFromAcceleration(num, magnitude, 0f - aOn);
			if (num3 >= 0f && num3 < num4)
			{
				num2 = num3;
				continue;
			}
			num2 = num4;
			aOn = 0f - aOn;
		}
		if (num2 < Time.fixedDeltaTime)
		{
			aOn *= num2 / Time.fixedDeltaTime;
			aOff *= num2 / Time.fixedDeltaTime;
		}
		return offset.normalized * aOn + vector.normalized * aOff;
	}

	public static float TimeFromAcceleration(float v, float x, float a)
	{
		if (a == 0f)
		{
			if (x == 0f)
			{
				return 0f;
			}
			if (x * v <= 0f)
			{
				return 10000f;
			}
			return x / v;
		}
		float num = 2f * v * v + 4f * a * x;
		if (num < 0f)
		{
			return 10000f;
		}
		float num2 = Mathf.Sqrt(num);
		float num3 = (0f - v + num2) / a;
		float num4 = (0f - v - num2) / a;
		float num5 = num3 / 2f - v / 2f / a;
		float num6 = num4 / 2f - v / 2f / a;
		if (num5 >= 0f && num6 >= 0f && num3 >= 0f && num4 >= 0f)
		{
			return Mathf.Min(num3, num4);
		}
		if (num5 >= 0f && num3 >= 0f)
		{
			return num3;
		}
		if (num6 >= 0f && num4 >= 0f)
		{
			return num4;
		}
		return 10000f;
	}

	public static float AccelerationFromTime(float v, float x, float t)
	{
		if (t == 0f)
		{
			throw new ArgumentException("t can't be 0", "t");
		}
		float f = (t * v - 2f * x) * (t * v - 2f * x) + t * t * v * v;
		float num = Mathf.Sqrt(f);
		float num2 = (0f - (t * v - 2f * x) + num) / t / t;
		float num3 = (0f - (t * v - 2f * x) - num) / t / t;
		float num4 = t / 2f - v / 2f / num2;
		float num5 = t / 2f - v / 2f / num3;
		if (num4 >= 0f && num5 >= 0f)
		{
			if (Mathf.Abs(num2) < Mathf.Abs(num3))
			{
				return num2;
			}
			return num3;
		}
		if (num4 >= 0f)
		{
			return num2;
		}
		if (num5 >= 0f)
		{
			return num3;
		}
		Debug.LogError($"did not find possitive solutions AccelerationFromTime({v} {x} {t})");
		return 10000f;
	}
}
