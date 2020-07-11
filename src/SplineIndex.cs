using System.Collections.Generic;
using UnityEngine;

public class SplineIndex
{
	public Vector3[] linearPoints;

	private SplineComponent spline;

	public int ControlPointCount => spline.ControlPointCount;

	public SplineIndex(SplineComponent spline)
	{
		this.spline = spline;
		ReIndex();
	}

	public void ReIndex()
	{
		float num = 1E-05f;
		float length = spline.GetLength(num);
		int capacity = Mathf.FloorToInt(length * 2f);
		List<Vector3> list = new List<Vector3>(capacity);
		float num2 = 0f;
		float f = length / 1024f;
		float num3 = Mathf.Pow(f, 2f);
		Vector3 vector = spline.GetNonUniformPoint(0f);
		list.Add(vector);
		while (num2 <= 1f)
		{
			Vector3 nonUniformPoint = spline.GetNonUniformPoint(num2);
			while ((nonUniformPoint - vector).sqrMagnitude <= num3)
			{
				num2 += num;
				nonUniformPoint = spline.GetNonUniformPoint(num2);
			}
			vector = nonUniformPoint;
			list.Add(nonUniformPoint);
		}
		linearPoints = list.ToArray();
	}

	public Vector3 GetPoint(float t)
	{
		int num = linearPoints.Length - ((!spline.closed) ? 3 : 0);
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		int num3 = linearPoints.Length;
		if (num2 < 0)
		{
			num2 += num3;
		}
		float u = t * (float)num - (float)num2;
		Vector3 a = linearPoints[num2 % num3];
		Vector3 b = linearPoints[(num2 + 1) % num3];
		Vector3 c = linearPoints[(num2 + 2) % num3];
		Vector3 d = linearPoints[(num2 + 3) % num3];
		return SplineComponent.Interpolate(a, b, c, d, u);
	}
}
