using System;
using UnityEngine;

public static class Math2d
{
	public static float SignedAngle(Vector2 from, Vector2 to)
	{
		float num = Vector2.Angle(from, to);
		Vector3 vector = Vector3.Cross(from, to);
		if (vector.z < 0f)
		{
			num = 0f - num;
		}
		return num;
	}

	public static float NormalizeAngle(float angle)
	{
		while (angle < -(float)Math.PI)
		{
			angle += (float)Math.PI * 2f;
		}
		while (angle > (float)Math.PI)
		{
			angle -= (float)Math.PI * 2f;
		}
		return angle;
	}

	public static float NormalizeAngleDeg(float angle)
	{
		while (angle < -180f)
		{
			angle += 360f;
		}
		while (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public static float NormalizeAnglePositive(float angle)
	{
		while (angle > (float)Math.PI * 2f)
		{
			angle -= (float)Math.PI * 2f;
		}
		while (angle < 0f)
		{
			angle += (float)Math.PI * 2f;
		}
		return angle;
	}

	public static float CalculateDistToBisector(Vector2 edgeVector, Vector2 normal, Vector2 startBisector, Vector2 endBisector)
	{
		float num = startBisector.x * endBisector.y - endBisector.x * startBisector.y;
		if (num != 0f)
		{
			float num2 = (edgeVector.x * endBisector.y - endBisector.x * edgeVector.y) / num;
			return (startBisector.x * normal.x + startBisector.y * normal.y) * num2;
		}
		return -10000f;
	}

	public static float CalculateTimeToBisector(Vector2 edgeVector, Vector2 normal, Vector2 startBisector, Vector2 endBisector)
	{
		if (startBisector == Vector2.zero)
		{
			if (endBisector == Vector2.zero)
			{
				return -10000f;
			}
			return Vector2.Dot(endBisector, -edgeVector) / endBisector.sqrMagnitude;
		}
		if (endBisector == Vector2.zero)
		{
			return Vector2.Dot(startBisector, edgeVector) / startBisector.sqrMagnitude;
		}
		float num = startBisector.x * endBisector.y - endBisector.x * startBisector.y;
		if (num != 0f)
		{
			return (edgeVector.x * endBisector.y - endBisector.x * edgeVector.y) / num;
		}
		return -10000f;
	}

	public static Vector2 CalculateUnitBisector(Vector2 a, Vector2 b)
	{
		Vector2 a2 = a + b;
		float num = a2.x * b.x + a2.y * b.y;
		if (num != 0f)
		{
			return a2 / num;
		}
		return a.RotateCW90();
	}

	public static Vector2 CalculateOffsetBisector(Vector2 a, Vector2 b)
	{
		float num = a.x * b.y - b.x * a.y;
		if (num != 0f)
		{
			Vector2 vector = a - b;
			float num2 = vector.x * b.x + vector.y * b.y;
			return a + num2 / num * a.RotateCW90();
		}
		return a.RotateCW90();
	}

	public static bool LineLineIntersection(out Vector2 intersection, Vector2 linePoint1, Vector2 lineVec1, Vector2 linePoint2, Vector2 lineVec2, float parallel_cross_epsilon = 1E-06f)
	{
		if (linePoint1 == linePoint2)
		{
			intersection = linePoint1;
			return true;
		}
		float y = lineVec1.y;
		float num = 0f - lineVec1.x;
		float num2 = y * linePoint1.x + num * linePoint1.y;
		float y2 = lineVec2.y;
		float num3 = 0f - lineVec2.x;
		float num4 = y2 * linePoint2.x + num3 * linePoint2.y;
		float num5 = y * num3 - y2 * num;
		if (Mathf.Abs(num5) < parallel_cross_epsilon)
		{
			intersection = Vector3.zero;
			return false;
		}
		intersection = new Vector2((num3 * num2 - num * num4) / num5, (y * num4 - y2 * num2) / num5);
		return true;
	}

	public static bool LineLineIntersection(out Vector2 intersection, Vector2 n1, float C1, Vector2 n2, float C2)
	{
		float x = n1.x;
		float y = n1.y;
		float x2 = n2.x;
		float y2 = n2.y;
		float num = x * y2 - x2 * y;
		if (Mathf.Abs(num) < 1E-06f)
		{
			intersection = Vector3.zero;
			return false;
		}
		intersection = new Vector2((y2 * C1 - y * C2) / num, (x * C2 - x2 * C1) / num);
		return true;
	}

	public static float Cross(Vector2 v1, Vector2 v2)
	{
		return v1.x * v2.y - v1.y * v2.x;
	}

	public static bool LineLineIntersection(out float t1, out float t2, Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2)
	{
		Vector2 vector = p2 - p1;
		float num = v1.x * v2.y - v2.x * v1.y;
		if (num == 0f)
		{
			float magnitude = (v1 - v2).magnitude;
			float num2 = (magnitude == 0f) ? 0f : (vector.magnitude / magnitude);
			t1 = (t2 = num2);
			return false;
		}
		t1 = (vector.x * v2.y - v2.x * vector.y) / num;
		t2 = (vector.x * v1.y - v1.x * vector.y) / num;
		return true;
	}

	public static int PointOnWhichSideOfLineSegment(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
	{
		Vector2 rhs = linePoint2 - linePoint1;
		Vector2 lhs = point - linePoint1;
		float num = Vector2.Dot(lhs, rhs);
		if (num > 0f)
		{
			if (lhs.sqrMagnitude <= rhs.sqrMagnitude)
			{
				return 0;
			}
			return 2;
		}
		return 1;
	}

	public static Vector2 ProjectPointOnLine(Vector2 linePoint, Vector2 lineVec, Vector2 point, out float t)
	{
		Vector2 lhs = point - linePoint;
		t = Vector2.Dot(lhs, lineVec);
		return linePoint + lineVec * t;
	}

	public static Vector2 ProjectPointOnLine(Vector2 linePoint, Vector2 lineVec, Vector2 point)
	{
		Vector2 lhs = point - linePoint;
		float d = Vector2.Dot(lhs, lineVec);
		return linePoint + lineVec * d;
	}

	public static Vector2 ProjectPointOnLineSegment(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
	{
		Vector2 vector = ProjectPointOnLine(linePoint1, (linePoint2 - linePoint1).normalized, point);
		switch (PointOnWhichSideOfLineSegment(linePoint1, linePoint2, vector))
		{
		case 0:
			return vector;
		case 1:
			return linePoint1;
		case 2:
			return linePoint2;
		default:
			return Vector2.zero;
		}
	}

	public static Vector2 ProjectPointOnLineSegment(Vector2 linePoint1, Vector2 linePoint2, Vector2 point, out float t)
	{
		Vector2 vector = linePoint2 - linePoint1;
		Vector2 result = ProjectPointOnLine(linePoint1, vector.normalized, point, out t);
		t /= vector.magnitude;
		if (t < 0f)
		{
			return linePoint1;
		}
		if (t > 1f)
		{
			return linePoint2;
		}
		return result;
	}

	public static float PointDistanceToSegment(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
	{
		Vector2 a = ProjectPointOnLineSegment(linePoint1, linePoint2, point);
		return ((Vector3)(a - point)).magnitude;
	}

	public static float LerpAngle(float from, float to, float t)
	{
		float num = NormalizeAngle(to - from);
		return NormalizeAngle(Mathf.Lerp(from, from + num, t));
	}
}
