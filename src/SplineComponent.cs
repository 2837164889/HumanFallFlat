using System.Collections.Generic;
using UnityEngine;

public class SplineComponent : MonoBehaviour, ISpline
{
	public bool closed;

	public List<Vector3> points = new List<Vector3>();

	public float? length;

	private SplineIndex uniformIndex;

	public int ControlPointCount => points.Count;

	private SplineIndex Index
	{
		get
		{
			if (uniformIndex == null)
			{
				uniformIndex = new SplineIndex(this);
			}
			return uniformIndex;
		}
	}

	public Vector3 GetNonUniformPoint(float t)
	{
		switch (points.Count)
		{
		case 0:
			return Vector3.zero;
		case 1:
			return base.transform.TransformPoint(points[0]);
		case 2:
			return base.transform.TransformPoint(Vector3.Lerp(points[0], points[1], t));
		case 3:
			return base.transform.TransformPoint(points[1]);
		default:
			return Hermite(t);
		}
	}

	public void InsertControlPoint(int index, Vector3 position)
	{
		ResetIndex();
		if (index >= points.Count)
		{
			points.Add(position);
		}
		else
		{
			points.Insert(index, position);
		}
	}

	public void RemoveControlPoint(int index)
	{
		ResetIndex();
		points.RemoveAt(index);
	}

	public Vector3 GetControlPoint(int index)
	{
		return points[index];
	}

	public void SetControlPoint(int index, Vector3 position)
	{
		ResetIndex();
		points[index] = position;
	}

	private Vector3 Hermite(float t)
	{
		int num = points.Count - ((!closed) ? 3 : 0);
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float u = t * (float)num - (float)num2;
		Vector3 pointByIndex = GetPointByIndex(num2);
		Vector3 pointByIndex2 = GetPointByIndex(num2 + 1);
		Vector3 pointByIndex3 = GetPointByIndex(num2 + 2);
		Vector3 pointByIndex4 = GetPointByIndex(num2 + 3);
		return base.transform.TransformPoint(Interpolate(pointByIndex, pointByIndex2, pointByIndex3, pointByIndex4, u));
	}

	private Vector3 GetPointByIndex(int i)
	{
		if (i < 0)
		{
			i += points.Count;
		}
		return points[i % points.Count];
	}

	public void ResetIndex()
	{
		uniformIndex = null;
		length = null;
	}

	public Vector3 GetRight(float t)
	{
		Vector3 point = GetPoint(t - 0.001f);
		Vector3 point2 = GetPoint(t + 0.001f);
		Vector3 vector = point2 - point;
		return new Vector3(0f - vector.z, 0f, vector.x).normalized;
	}

	public Vector3 GetForward(float t)
	{
		Vector3 point = GetPoint(t - 0.001f);
		Vector3 point2 = GetPoint(t + 0.001f);
		return (point2 - point).normalized;
	}

	public Vector3 GetUp(float t)
	{
		Vector3 point = GetPoint(t - 0.001f);
		Vector3 point2 = GetPoint(t + 0.001f);
		Vector3 normalized = (point2 - point).normalized;
		return Vector3.Cross(normalized, GetRight(t));
	}

	public Vector3 GetPoint(float t)
	{
		return Index.GetPoint(t);
	}

	public Vector3 GetLeft(float t)
	{
		return -GetRight(t);
	}

	public Vector3 GetDown(float t)
	{
		return -GetUp(t);
	}

	public Vector3 GetBackward(float t)
	{
		return -GetForward(t);
	}

	public float GetLength(float step = 0.001f)
	{
		float num = 0f;
		Vector3 b = GetNonUniformPoint(0f);
		for (float num2 = 0f; num2 < 1f; num2 += step)
		{
			Vector3 nonUniformPoint = GetNonUniformPoint(num2);
			num += (nonUniformPoint - b).magnitude;
			b = nonUniformPoint;
		}
		return num;
	}

	public Vector3 GetDistance(float distance)
	{
		float? num = length;
		if (!num.HasValue)
		{
			length = GetLength();
		}
		return Index.GetPoint(distance / length.Value);
	}

	public Vector3 FindClosest(Vector3 worldPoint)
	{
		float num = float.MaxValue;
		float num2 = 0.0009765625f;
		Vector3 result = Vector3.zero;
		for (int i = 0; i <= 1024; i++)
		{
			Vector3 point = GetPoint((float)i * num2);
			float sqrMagnitude = (worldPoint - point).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				result = point;
				num = sqrMagnitude;
			}
		}
		return result;
	}

	private void Reset()
	{
		points = new List<Vector3>
		{
			Vector3.forward * 3f,
			Vector3.forward * 6f,
			Vector3.forward * 9f,
			Vector3.forward * 12f
		};
	}

	private void OnValidate()
	{
		if (uniformIndex != null)
		{
			uniformIndex.ReIndex();
		}
	}

	internal static Vector3 Interpolate(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float u)
	{
		return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
	}

	public void Flatten(List<Vector3> points)
	{
		for (int i = 0; i < points.Count; i++)
		{
			points[i] = Vector3.Scale(points[i], new Vector3(1f, 0f, 1f));
		}
	}

	public void CenterAroundOrigin(List<Vector3> points)
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < points.Count; i++)
		{
			zero += points[i];
		}
		zero /= (float)points.Count;
		for (int j = 0; j < points.Count; j++)
		{
			points[j] -= zero;
		}
	}

	public void Reverse(List<Vector3> points)
	{
		points.Reverse();
	}
}
