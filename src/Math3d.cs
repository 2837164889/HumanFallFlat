using System;
using UnityEngine;

public static class Math3d
{
	public static Vector3 AddVectorLength(Vector3 vector, float size)
	{
		float num = Vector3.Magnitude(vector);
		num += size;
		Vector3 a = Vector3.Normalize(vector);
		return Vector3.Scale(a, new Vector3(num, num, num));
	}

	public static Vector3 SetVectorLength(Vector3 vector, float size)
	{
		Vector3 vector2 = Vector3.Normalize(vector);
		return vector2 *= size;
	}

	public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
	{
		return Quaternion.Inverse(A) * B;
	}

	public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
	{
		linePoint = Vector3.zero;
		lineVec = Vector3.zero;
		lineVec = Vector3.Cross(plane1Normal, plane2Normal);
		Vector3 vector = Vector3.Cross(plane2Normal, lineVec);
		float num = Vector3.Dot(plane1Normal, vector);
		if (Mathf.Abs(num) > 0.006f)
		{
			Vector3 rhs = plane1Position - plane2Position;
			float d = Vector3.Dot(plane1Normal, rhs) / num;
			linePoint = plane2Position + d * vector;
			return true;
		}
		return false;
	}

	public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
	{
		intersection = Vector3.zero;
		float num = Vector3.Dot(planePoint - linePoint, planeNormal);
		float num2 = Vector3.Dot(lineVec, planeNormal);
		if (num2 != 0f)
		{
			float size = num / num2;
			Vector3 b = SetVectorLength(lineVec, size);
			intersection = linePoint + b;
			return true;
		}
		return false;
	}

	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		intersection = Vector3.zero;
		Vector3 lhs = linePoint2 - linePoint1;
		Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
		Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
		float num = Vector3.Dot(lhs, rhs);
		if (num >= 1E-05f || num <= -1E-05f)
		{
			return false;
		}
		float num2 = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
		if (num2 >= 0f && num2 <= 1f)
		{
			intersection = linePoint1 + lineVec1 * num2;
			return true;
		}
		return false;
	}

	public static bool LineLineIntersection(out float tLine1, out float tLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		tLine1 = float.PositiveInfinity;
		tLine2 = float.PositiveInfinity;
		Vector3 lhs = linePoint2 - linePoint1;
		Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
		Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
		Vector3 lhs3 = Vector3.Cross(lhs, lineVec1);
		float num = Vector3.Dot(lhs, rhs);
		if (num >= 1E-05f || num <= -1E-05f)
		{
			return false;
		}
		tLine1 = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
		tLine2 = Vector3.Dot(lhs3, rhs) / rhs.sqrMagnitude;
		return true;
	}

	public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		closestPointLine1 = Vector3.zero;
		closestPointLine2 = Vector3.zero;
		float num = Vector3.Dot(lineVec1, lineVec1);
		float num2 = Vector3.Dot(lineVec1, lineVec2);
		float num3 = Vector3.Dot(lineVec2, lineVec2);
		float num4 = num * num3 - num2 * num2;
		if (num4 != 0f)
		{
			Vector3 rhs = linePoint1 - linePoint2;
			float num5 = Vector3.Dot(lineVec1, rhs);
			float num6 = Vector3.Dot(lineVec2, rhs);
			float d = (num2 * num6 - num5 * num3) / num4;
			float d2 = (num * num6 - num5 * num2) / num4;
			closestPointLine1 = linePoint1 + lineVec1 * d;
			closestPointLine2 = linePoint2 + lineVec2 * d2;
			return true;
		}
		return false;
	}

	public static bool ClosestPointsOnTwoLines(out float s, out float t, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		t = (s = 0f);
		float num = Vector3.Dot(lineVec1, lineVec1);
		float num2 = Vector3.Dot(lineVec1, lineVec2);
		float num3 = Vector3.Dot(lineVec2, lineVec2);
		float num4 = num * num3 - num2 * num2;
		if (num4 != 0f)
		{
			Vector3 rhs = linePoint1 - linePoint2;
			float num5 = Vector3.Dot(lineVec1, rhs);
			float num6 = Vector3.Dot(lineVec2, rhs);
			s = (num2 * num6 - num5 * num3) / num4;
			t = (num * num6 - num5 * num2) / num4;
			return true;
		}
		return false;
	}

	public static bool ClosestPointsOnLineSegment(out Vector3 closestPointLine, out Vector3 closestPointSegment, out float lineT, out float segmentT, Vector3 linePoint, Vector3 lineVec, Vector3 segmentPoint1, Vector3 segmentPoint2)
	{
		Vector3 vector = segmentPoint2 - segmentPoint1;
		closestPointLine = Vector3.zero;
		closestPointSegment = Vector3.zero;
		segmentT = 0f;
		lineT = 0f;
		float num = Vector3.Dot(lineVec, lineVec);
		float num2 = Vector3.Dot(lineVec, vector);
		float num3 = Vector3.Dot(vector, vector);
		float num4 = num * num3 - num2 * num2;
		if (num4 != 0f)
		{
			Vector3 rhs = linePoint - segmentPoint1;
			float num5 = Vector3.Dot(lineVec, rhs);
			float num6 = Vector3.Dot(vector, rhs);
			float num7 = (num2 * num6 - num5 * num3) / num4;
			float value = (num * num6 - num5 * num2) / num4;
			lineT = num7;
			segmentT = Mathf.Clamp01(value);
			closestPointLine = linePoint + lineVec * num7;
			closestPointSegment = segmentPoint1 + vector * segmentT;
			return true;
		}
		return false;
	}

	public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
	{
		Vector3 lhs = point - linePoint;
		float d = Vector3.Dot(lhs, lineVec);
		return linePoint + lineVec * d;
	}

	public static Vector3 ProjectPointOnLine(out float t, Vector3 linePoint, Vector3 lineVec, Vector3 point)
	{
		Vector3 lhs = point - linePoint;
		t = Vector3.Dot(lhs, lineVec);
		return linePoint + lineVec * t;
	}

	public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
	{
		Vector3 vector = ProjectPointOnLine(linePoint1, (linePoint2 - linePoint1).normalized, point);
		switch (PointOnWhichSideOfLineSegment(linePoint1, linePoint2, vector))
		{
		case 0:
			return vector;
		case 1:
			return linePoint1;
		case 2:
			return linePoint2;
		default:
			return Vector3.zero;
		}
	}

	public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
	{
		float num = SignedDistancePlanePoint(planeNormal, planePoint, point);
		num *= -1f;
		Vector3 b = SetVectorLength(planeNormal, num);
		return point + b;
	}

	public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
	{
		return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
	}

	public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
	{
		return Vector3.Dot(planeNormal, point - planePoint);
	}

	public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
	{
		Vector3 lhs = Vector3.Cross(normal, vectorA);
		return Vector3.Dot(lhs, vectorB);
	}

	public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
	{
		Vector3 lhs = Vector3.Cross(normal, referenceVector);
		float num = Vector3.Angle(referenceVector, otherVector);
		return num * Mathf.Sign(Vector3.Dot(lhs, otherVector));
	}

	public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
	{
		float num = Vector3.Dot(vector, normal);
		float num2 = (float)Math.Acos(num);
		return (float)Math.PI / 2f - num2;
	}

	public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
	{
		double num = Vector3.Dot(vec1, vec2);
		if (num < -1.0)
		{
			num = -1.0;
		}
		if (num > 1.0)
		{
			num = 1.0;
		}
		double num2 = Math.Acos(num);
		return (float)num2;
	}

	public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC)
	{
		planeNormal = Vector3.zero;
		planePoint = Vector3.zero;
		Vector3 vector = pointB - pointA;
		Vector3 vector2 = pointC - pointA;
		planeNormal = Vector3.Normalize(Vector3.Cross(vector, vector2));
		Vector3 vector3 = pointA + vector / 2f;
		Vector3 vector4 = pointA + vector2 / 2f;
		Vector3 lineVec = pointC - vector3;
		Vector3 lineVec2 = pointB - vector4;
		ClosestPointsOnTwoLines(out planePoint, out Vector3 _, vector3, lineVec, vector4, lineVec2);
	}

	public static Vector3 GetForwardVector(Quaternion q)
	{
		return q * Vector3.forward;
	}

	public static Vector3 GetUpVector(Quaternion q)
	{
		return q * Vector3.up;
	}

	public static Vector3 GetRightVector(Quaternion q)
	{
		return q * Vector3.right;
	}

	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	public static Vector3 PositionFromMatrix(Matrix4x4 m)
	{
		Vector4 column = m.GetColumn(3);
		return new Vector3(column.x, column.y, column.z);
	}

	public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
	{
		Quaternion lhs = Quaternion.LookRotation(alignWithVector, alignWithNormal);
		Quaternion rotation = Quaternion.LookRotation(customForward, customUp);
		gameObjectInOut.transform.rotation = lhs * Quaternion.Inverse(rotation);
	}

	public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal, Vector3 trianglePosition)
	{
		LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward, triangleNormal);
		Vector3 b = gameObjectInOut.transform.TransformPoint(trianglePosition);
		Vector3 translation = alignWithPosition - b;
		gameObjectInOut.transform.Translate(translation, Space.World);
	}

	private static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector, Vector3 directionVector, Vector3 normalVector)
	{
		gameObjectInOut.transform.position = positionVector;
		gameObjectInOut.transform.rotation = Quaternion.LookRotation(directionVector, normalVector);
	}

	public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
	{
		Vector3 rhs = linePoint2 - linePoint1;
		Vector3 lhs = point - linePoint1;
		float num = Vector3.Dot(lhs, rhs);
		if (num > 0f)
		{
			if (lhs.magnitude <= rhs.magnitude)
			{
				return 0;
			}
			return 2;
		}
		return 1;
	}

	public static float MouseDistanceToLine(Vector3 linePoint1, Vector3 linePoint2)
	{
		Camera main = Camera.main;
		Vector3 mousePosition = Input.mousePosition;
		Vector3 linePoint3 = main.WorldToScreenPoint(linePoint1);
		Vector3 linePoint4 = main.WorldToScreenPoint(linePoint2);
		Vector3 vector = ProjectPointOnLineSegment(linePoint3, linePoint4, mousePosition);
		vector = new Vector3(vector.x, vector.y, 0f);
		return (vector - mousePosition).magnitude;
	}

	public static float MouseDistanceToCircle(Vector3 point, float radius)
	{
		Camera main = Camera.main;
		Vector3 mousePosition = Input.mousePosition;
		Vector3 vector = main.WorldToScreenPoint(point);
		vector = new Vector3(vector.x, vector.y, 0f);
		float magnitude = (vector - mousePosition).magnitude;
		return magnitude - radius;
	}

	public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
	{
		bool flag = false;
		bool flag2 = false;
		flag = IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);
		if (!flag)
		{
			flag2 = IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
		}
		if (!flag && !flag2)
		{
			bool flag3 = AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
			bool flag4 = AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
			bool flag5 = AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
			bool flag6 = AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);
			if (flag3 || flag4 || flag5 || flag6)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
	{
		Vector3 vector = rectC - rectA;
		float size = 0f - vector.magnitude / 2f;
		vector = AddVectorLength(vector, size);
		Vector3 linePoint = rectA + vector;
		Vector3 vector2 = rectB - rectA;
		float num = vector2.magnitude / 2f;
		Vector3 vector3 = rectD - rectA;
		float num2 = vector3.magnitude / 2f;
		Vector3 a = ProjectPointOnLine(linePoint, vector2.normalized, point);
		float magnitude = (a - point).magnitude;
		a = ProjectPointOnLine(linePoint, vector3.normalized, point);
		float magnitude2 = (a - point).magnitude;
		if (magnitude2 <= num && magnitude <= num2)
		{
			return true;
		}
		return false;
	}

	public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
	{
		Vector3 vector = pointA2 - pointA1;
		if (ClosestPointsOnTwoLines(lineVec2: (pointB2 - pointB1).normalized, closestPointLine1: out Vector3 closestPointLine, closestPointLine2: out Vector3 closestPointLine2, linePoint1: pointA1, lineVec1: vector.normalized, linePoint2: pointB1))
		{
			int num = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointLine);
			int num2 = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointLine2);
			if (num == 0 && num2 == 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
