using System;
using System.Reflection;
using UnityEngine;

public static class DebugExtension
{
	public static void DebugPoint(Vector3 position, Color color, float scale = 1f, float duration = 0f, bool depthTest = true)
	{
		color = ((!(color == default(Color))) ? color : Color.white);
		Debug.DrawRay(position + Vector3.up * (scale * 0.5f), -Vector3.up * scale, color, duration, depthTest);
		Debug.DrawRay(position + Vector3.right * (scale * 0.5f), -Vector3.right * scale, color, duration, depthTest);
		Debug.DrawRay(position + Vector3.forward * (scale * 0.5f), -Vector3.forward * scale, color, duration, depthTest);
	}

	public static void DebugPoint(Vector3 position, float scale = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugPoint(position, Color.white, scale, duration, depthTest);
	}

	public static void DebugBounds(Bounds bounds, Color color, float duration = 0f, bool depthTest = true)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		float x = extents.x;
		Vector3 extents2 = bounds.extents;
		float y = extents2.y;
		Vector3 extents3 = bounds.extents;
		float z = extents3.z;
		Vector3 start = center + new Vector3(x, y, z);
		Vector3 vector = center + new Vector3(x, y, 0f - z);
		Vector3 vector2 = center + new Vector3(0f - x, y, z);
		Vector3 vector3 = center + new Vector3(0f - x, y, 0f - z);
		Vector3 vector4 = center + new Vector3(x, 0f - y, z);
		Vector3 end = center + new Vector3(x, 0f - y, 0f - z);
		Vector3 vector5 = center + new Vector3(0f - x, 0f - y, z);
		Vector3 vector6 = center + new Vector3(0f - x, 0f - y, 0f - z);
		Debug.DrawLine(start, vector2, color, duration, depthTest);
		Debug.DrawLine(start, vector, color, duration, depthTest);
		Debug.DrawLine(vector2, vector3, color, duration, depthTest);
		Debug.DrawLine(vector, vector3, color, duration, depthTest);
		Debug.DrawLine(start, vector4, color, duration, depthTest);
		Debug.DrawLine(vector, end, color, duration, depthTest);
		Debug.DrawLine(vector2, vector5, color, duration, depthTest);
		Debug.DrawLine(vector3, vector6, color, duration, depthTest);
		Debug.DrawLine(vector4, vector5, color, duration, depthTest);
		Debug.DrawLine(vector4, end, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector6, end, color, duration, depthTest);
	}

	public static void DebugBounds(Bounds bounds, float duration = 0f, bool depthTest = true)
	{
		DebugBounds(bounds, Color.white, duration, depthTest);
	}

	public static void DebugLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = transform.TransformPoint(center + -size * 0.5f);
		Vector3 vector2 = transform.TransformPoint(center + new Vector3(size.x, 0f - size.y, 0f - size.z) * 0.5f);
		Vector3 vector3 = transform.TransformPoint(center + new Vector3(size.x, 0f - size.y, size.z) * 0.5f);
		Vector3 vector4 = transform.TransformPoint(center + new Vector3(0f - size.x, 0f - size.y, size.z) * 0.5f);
		Vector3 vector5 = transform.TransformPoint(center + new Vector3(0f - size.x, size.y, 0f - size.z) * 0.5f);
		Vector3 vector6 = transform.TransformPoint(center + new Vector3(size.x, size.y, 0f - size.z) * 0.5f);
		Vector3 vector7 = transform.TransformPoint(center + size * 0.5f);
		Vector3 vector8 = transform.TransformPoint(center + new Vector3(0f - size.x, size.y, size.z) * 0.5f);
		Debug.DrawLine(vector, vector2, color, duration, depthTest);
		Debug.DrawLine(vector2, vector3, color, duration, depthTest);
		Debug.DrawLine(vector3, vector4, color, duration, depthTest);
		Debug.DrawLine(vector4, vector, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector6, vector7, color, duration, depthTest);
		Debug.DrawLine(vector7, vector8, color, duration, depthTest);
		Debug.DrawLine(vector8, vector5, color, duration, depthTest);
		Debug.DrawLine(vector, vector5, color, duration, depthTest);
		Debug.DrawLine(vector2, vector6, color, duration, depthTest);
		Debug.DrawLine(vector3, vector7, color, duration, depthTest);
		Debug.DrawLine(vector4, vector8, color, duration, depthTest);
	}

	public static void DebugLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		DebugLocalCube(transform, size, Color.white, center, duration, depthTest);
	}

	public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		color = ((!(color == default(Color))) ? color : Color.white);
		Vector3 vector = space.MultiplyPoint3x4(center + -size * 0.5f);
		Vector3 vector2 = space.MultiplyPoint3x4(center + new Vector3(size.x, 0f - size.y, 0f - size.z) * 0.5f);
		Vector3 vector3 = space.MultiplyPoint3x4(center + new Vector3(size.x, 0f - size.y, size.z) * 0.5f);
		Vector3 vector4 = space.MultiplyPoint3x4(center + new Vector3(0f - size.x, 0f - size.y, size.z) * 0.5f);
		Vector3 vector5 = space.MultiplyPoint3x4(center + new Vector3(0f - size.x, size.y, 0f - size.z) * 0.5f);
		Vector3 vector6 = space.MultiplyPoint3x4(center + new Vector3(size.x, size.y, 0f - size.z) * 0.5f);
		Vector3 vector7 = space.MultiplyPoint3x4(center + size * 0.5f);
		Vector3 vector8 = space.MultiplyPoint3x4(center + new Vector3(0f - size.x, size.y, size.z) * 0.5f);
		Debug.DrawLine(vector, vector2, color, duration, depthTest);
		Debug.DrawLine(vector2, vector3, color, duration, depthTest);
		Debug.DrawLine(vector3, vector4, color, duration, depthTest);
		Debug.DrawLine(vector4, vector, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector6, vector7, color, duration, depthTest);
		Debug.DrawLine(vector7, vector8, color, duration, depthTest);
		Debug.DrawLine(vector8, vector5, color, duration, depthTest);
		Debug.DrawLine(vector, vector5, color, duration, depthTest);
		Debug.DrawLine(vector2, vector6, color, duration, depthTest);
		Debug.DrawLine(vector3, vector7, color, duration, depthTest);
		Debug.DrawLine(vector4, vector8, color, duration, depthTest);
	}

	public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		DebugLocalCube(space, size, Color.white, center, duration, depthTest);
	}

	public static void DebugCircle(Vector3 position, Vector3 up, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = up.normalized * radius;
		Vector3 rhs = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector2 = Vector3.Cross(vector, rhs).normalized * radius;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x[0] = vector2.x;
		matrix4x[1] = vector2.y;
		matrix4x[2] = vector2.z;
		matrix4x[4] = vector.x;
		matrix4x[5] = vector.y;
		matrix4x[6] = vector.z;
		matrix4x[8] = rhs.x;
		matrix4x[9] = rhs.y;
		matrix4x[10] = rhs.z;
		Vector3 start = position + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
		Vector3 vector3 = Vector3.zero;
		color = ((!(color == default(Color))) ? color : Color.white);
		for (int i = 0; i < 91; i++)
		{
			vector3.x = Mathf.Cos((float)(i * 4) * ((float)Math.PI / 180f));
			vector3.z = Mathf.Sin((float)(i * 4) * ((float)Math.PI / 180f));
			vector3.y = 0f;
			vector3 = position + matrix4x.MultiplyPoint3x4(vector3);
			Debug.DrawLine(start, vector3, color, duration, depthTest);
			start = vector3;
		}
	}

	public static void DebugCircle(Vector3 position, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugCircle(position, Vector3.up, color, radius, duration, depthTest);
	}

	public static void DebugCircle(Vector3 position, Vector3 up, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugCircle(position, up, Color.white, radius, duration, depthTest);
	}

	public static void DebugCircle(Vector3 position, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugCircle(position, Vector3.up, Color.white, radius, duration, depthTest);
	}

	public static void DebugWireSphere(Vector3 position, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		float num = 10f;
		Vector3 start = new Vector3(position.x, position.y + radius * Mathf.Sin(0f), position.z + radius * Mathf.Cos(0f));
		Vector3 start2 = new Vector3(position.x + radius * Mathf.Cos(0f), position.y, position.z + radius * Mathf.Sin(0f));
		Vector3 start3 = new Vector3(position.x + radius * Mathf.Cos(0f), position.y + radius * Mathf.Sin(0f), position.z);
		for (int i = 1; i < 37; i++)
		{
			Vector3 vector = new Vector3(position.x, position.y + radius * Mathf.Sin(num * (float)i * ((float)Math.PI / 180f)), position.z + radius * Mathf.Cos(num * (float)i * ((float)Math.PI / 180f)));
			Vector3 vector2 = new Vector3(position.x + radius * Mathf.Cos(num * (float)i * ((float)Math.PI / 180f)), position.y, position.z + radius * Mathf.Sin(num * (float)i * ((float)Math.PI / 180f)));
			Vector3 vector3 = new Vector3(position.x + radius * Mathf.Cos(num * (float)i * ((float)Math.PI / 180f)), position.y + radius * Mathf.Sin(num * (float)i * ((float)Math.PI / 180f)), position.z);
			Debug.DrawLine(start, vector, color, duration, depthTest);
			Debug.DrawLine(start2, vector2, color, duration, depthTest);
			Debug.DrawLine(start3, vector3, color, duration, depthTest);
			start = vector;
			start2 = vector2;
			start3 = vector3;
		}
	}

	public static void DebugWireSphere(Vector3 position, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugWireSphere(position, Color.white, radius, duration, depthTest);
	}

	public static void DebugCylinder(Vector3 start, Vector3 end, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 b = Vector3.Cross(vector, vector2).normalized * radius;
		DebugCircle(start, vector, color, radius, duration, depthTest);
		DebugCircle(end, -vector, color, radius, duration, depthTest);
		DebugCircle((start + end) * 0.5f, vector, color, radius, duration, depthTest);
		Debug.DrawLine(start + b, end + b, color, duration, depthTest);
		Debug.DrawLine(start - b, end - b, color, duration, depthTest);
		Debug.DrawLine(start + vector2, end + vector2, color, duration, depthTest);
		Debug.DrawLine(start - vector2, end - vector2, color, duration, depthTest);
		Debug.DrawLine(start - b, start + b, color, duration, depthTest);
		Debug.DrawLine(start - vector2, start + vector2, color, duration, depthTest);
		Debug.DrawLine(end - b, end + b, color, duration, depthTest);
		Debug.DrawLine(end - vector2, end + vector2, color, duration, depthTest);
	}

	public static void DebugCylinder(Vector3 start, Vector3 end, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugCylinder(start, end, Color.white, radius, duration, depthTest);
	}

	public static void DebugCone(Vector3 position, Vector3 direction, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		float magnitude = direction.magnitude;
		Vector3 vector = direction;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * magnitude;
		direction = direction.normalized;
		Vector3 direction2 = Vector3.Slerp(vector, vector2, angle / 90f);
		Plane plane = new Plane(-direction, position + vector);
		Ray ray = new Ray(position, direction2);
		plane.Raycast(ray, out float enter);
		Debug.DrawRay(position, direction2.normalized * enter, color);
		Debug.DrawRay(position, Vector3.Slerp(vector, -vector2, angle / 90f).normalized * enter, color, duration, depthTest);
		Debug.DrawRay(position, Vector3.Slerp(vector, vector3, angle / 90f).normalized * enter, color, duration, depthTest);
		Debug.DrawRay(position, Vector3.Slerp(vector, -vector3, angle / 90f).normalized * enter, color, duration, depthTest);
		DebugCircle(position + vector, direction, color, (vector - direction2.normalized * enter).magnitude, duration, depthTest);
		DebugCircle(position + vector * 0.5f, direction, color, (vector * 0.5f - direction2.normalized * (enter * 0.5f)).magnitude, duration, depthTest);
	}

	public static void DebugCone(Vector3 position, Vector3 direction, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugCone(position, direction, Color.white, angle, duration, depthTest);
	}

	public static void DebugCone(Vector3 position, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugCone(position, Vector3.up, color, angle, duration, depthTest);
	}

	public static void DebugCone(Vector3 position, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugCone(position, Vector3.up, Color.white, angle, duration, depthTest);
	}

	public static void DebugArrow(Vector3 position, Vector3 direction, Color color, float duration = 0f, bool depthTest = true)
	{
		Debug.DrawRay(position, direction, color, duration, depthTest);
		DebugCone(position + direction, -direction * 0.333f, color, 15f, duration, depthTest);
	}

	public static void DebugArrow(Vector3 position, Vector3 direction, float duration = 0f, bool depthTest = true)
	{
		DebugArrow(position, direction, Color.white, duration, depthTest);
	}

	public static void DebugCapsule(Vector3 start, Vector3 end, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		float magnitude = (start - end).magnitude;
		float d = Mathf.Max(0f, magnitude * 0.5f - radius);
		Vector3 vector4 = (end + start) * 0.5f;
		start = vector4 + (start - vector4).normalized * d;
		end = vector4 + (end - vector4).normalized * d;
		DebugCircle(start, vector, color, radius, duration, depthTest);
		DebugCircle(end, -vector, color, radius, duration, depthTest);
		Debug.DrawLine(start + vector3, end + vector3, color, duration, depthTest);
		Debug.DrawLine(start - vector3, end - vector3, color, duration, depthTest);
		Debug.DrawLine(start + vector2, end + vector2, color, duration, depthTest);
		Debug.DrawLine(start - vector2, end - vector2, color, duration, depthTest);
		for (int i = 1; i < 26; i++)
		{
			Debug.DrawLine(Vector3.Slerp(vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(vector3, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector3, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(vector2, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector2, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector3, vector, (float)i / 25f) + end, Vector3.Slerp(vector3, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector3, vector, (float)i / 25f) + end, Vector3.Slerp(-vector3, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector2, vector, (float)i / 25f) + end, Vector3.Slerp(vector2, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector2, vector, (float)i / 25f) + end, Vector3.Slerp(-vector2, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
		}
	}

	public static void DebugCapsule(Vector3 start, Vector3 end, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugCapsule(start, end, Color.white, radius, duration, depthTest);
	}

	public static void DrawPoint(Vector3 position, Color color, float scale = 1f)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position + Vector3.up * (scale * 0.5f), -Vector3.up * scale);
		Gizmos.DrawRay(position + Vector3.right * (scale * 0.5f), -Vector3.right * scale);
		Gizmos.DrawRay(position + Vector3.forward * (scale * 0.5f), -Vector3.forward * scale);
		Gizmos.color = color2;
	}

	public static void DrawPoint(Vector3 position, float scale = 1f)
	{
		DrawPoint(position, Color.white, scale);
	}

	public static void DrawBounds(Bounds bounds, Color color)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		float x = extents.x;
		Vector3 extents2 = bounds.extents;
		float y = extents2.y;
		Vector3 extents3 = bounds.extents;
		float z = extents3.z;
		Vector3 from = center + new Vector3(x, y, z);
		Vector3 vector = center + new Vector3(x, y, 0f - z);
		Vector3 vector2 = center + new Vector3(0f - x, y, z);
		Vector3 vector3 = center + new Vector3(0f - x, y, 0f - z);
		Vector3 vector4 = center + new Vector3(x, 0f - y, z);
		Vector3 to = center + new Vector3(x, 0f - y, 0f - z);
		Vector3 vector5 = center + new Vector3(0f - x, 0f - y, z);
		Vector3 vector6 = center + new Vector3(0f - x, 0f - y, 0f - z);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(from, vector2);
		Gizmos.DrawLine(from, vector);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector, vector3);
		Gizmos.DrawLine(from, vector4);
		Gizmos.DrawLine(vector, to);
		Gizmos.DrawLine(vector2, vector5);
		Gizmos.DrawLine(vector3, vector6);
		Gizmos.DrawLine(vector4, vector5);
		Gizmos.DrawLine(vector4, to);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector6, to);
		Gizmos.color = color2;
	}

	public static void DrawBounds(Bounds bounds)
	{
		DrawBounds(bounds, Color.white);
	}

	public static void DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3))
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Vector3 vector = transform.TransformPoint(center + -size * 0.5f);
		Vector3 vector2 = transform.TransformPoint(center + new Vector3(size.x, 0f - size.y, 0f - size.z) * 0.5f);
		Vector3 vector3 = transform.TransformPoint(center + new Vector3(size.x, 0f - size.y, size.z) * 0.5f);
		Vector3 vector4 = transform.TransformPoint(center + new Vector3(0f - size.x, 0f - size.y, size.z) * 0.5f);
		Vector3 vector5 = transform.TransformPoint(center + new Vector3(0f - size.x, size.y, 0f - size.z) * 0.5f);
		Vector3 vector6 = transform.TransformPoint(center + new Vector3(size.x, size.y, 0f - size.z) * 0.5f);
		Vector3 vector7 = transform.TransformPoint(center + size * 0.5f);
		Vector3 vector8 = transform.TransformPoint(center + new Vector3(0f - size.x, size.y, size.z) * 0.5f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector6, vector7);
		Gizmos.DrawLine(vector7, vector8);
		Gizmos.DrawLine(vector8, vector5);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, vector8);
		Gizmos.color = color2;
	}

	public static void DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3))
	{
		DrawLocalCube(transform, size, Color.white, center);
	}

	public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3))
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Vector3 vector = space.MultiplyPoint3x4(center + -size * 0.5f);
		Vector3 vector2 = space.MultiplyPoint3x4(center + new Vector3(size.x, 0f - size.y, 0f - size.z) * 0.5f);
		Vector3 vector3 = space.MultiplyPoint3x4(center + new Vector3(size.x, 0f - size.y, size.z) * 0.5f);
		Vector3 vector4 = space.MultiplyPoint3x4(center + new Vector3(0f - size.x, 0f - size.y, size.z) * 0.5f);
		Vector3 vector5 = space.MultiplyPoint3x4(center + new Vector3(0f - size.x, size.y, 0f - size.z) * 0.5f);
		Vector3 vector6 = space.MultiplyPoint3x4(center + new Vector3(size.x, size.y, 0f - size.z) * 0.5f);
		Vector3 vector7 = space.MultiplyPoint3x4(center + size * 0.5f);
		Vector3 vector8 = space.MultiplyPoint3x4(center + new Vector3(0f - size.x, size.y, size.z) * 0.5f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector6, vector7);
		Gizmos.DrawLine(vector7, vector8);
		Gizmos.DrawLine(vector8, vector5);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, vector8);
		Gizmos.color = color2;
	}

	public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3))
	{
		DrawLocalCube(space, size, Color.white, center);
	}

	public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1f)
	{
		up = ((!(up == Vector3.zero)) ? up : Vector3.up).normalized * radius;
		Vector3 rhs = Vector3.Slerp(up, -up, 0.5f);
		Vector3 vector = Vector3.Cross(up, rhs).normalized * radius;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x[0] = vector.x;
		matrix4x[1] = vector.y;
		matrix4x[2] = vector.z;
		matrix4x[4] = up.x;
		matrix4x[5] = up.y;
		matrix4x[6] = up.z;
		matrix4x[8] = rhs.x;
		matrix4x[9] = rhs.y;
		matrix4x[10] = rhs.z;
		Vector3 from = position + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
		Vector3 vector2 = Vector3.zero;
		Color color2 = Gizmos.color;
		Gizmos.color = ((!(color == default(Color))) ? color : Color.white);
		for (int i = 0; i < 91; i++)
		{
			vector2.x = Mathf.Cos((float)(i * 4) * ((float)Math.PI / 180f));
			vector2.z = Mathf.Sin((float)(i * 4) * ((float)Math.PI / 180f));
			vector2.y = 0f;
			vector2 = position + matrix4x.MultiplyPoint3x4(vector2);
			Gizmos.DrawLine(from, vector2);
			from = vector2;
		}
		Gizmos.color = color2;
	}

	public static void DrawCircle(Vector3 position, Color color, float radius = 1f)
	{
		DrawCircle(position, Vector3.up, color, radius);
	}

	public static void DrawCircle(Vector3 position, Vector3 up, float radius = 1f)
	{
		DrawCircle(position, position, Color.white, radius);
	}

	public static void DrawCircle(Vector3 position, float radius = 1f)
	{
		DrawCircle(position, Vector3.up, Color.white, radius);
	}

	public static void DrawCylinder(Vector3 start, Vector3 end, Color color, float radius = 1f)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 b = Vector3.Cross(vector, vector2).normalized * radius;
		DrawCircle(start, vector, color, radius);
		DrawCircle(end, -vector, color, radius);
		DrawCircle((start + end) * 0.5f, vector, color, radius);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(start + b, end + b);
		Gizmos.DrawLine(start - b, end - b);
		Gizmos.DrawLine(start + vector2, end + vector2);
		Gizmos.DrawLine(start - vector2, end - vector2);
		Gizmos.DrawLine(start - b, start + b);
		Gizmos.DrawLine(start - vector2, start + vector2);
		Gizmos.DrawLine(end - b, end + b);
		Gizmos.DrawLine(end - vector2, end + vector2);
		Gizmos.color = color2;
	}

	public static void DrawCylinder(Vector3 start, Vector3 end, float radius = 1f)
	{
		DrawCylinder(start, end, Color.white, radius);
	}

	public static void DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45f)
	{
		float magnitude = direction.magnitude;
		Vector3 vector = direction;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * magnitude;
		direction = direction.normalized;
		Vector3 direction2 = Vector3.Slerp(vector, vector2, angle / 90f);
		Plane plane = new Plane(-direction, position + vector);
		Ray ray = new Ray(position, direction2);
		plane.Raycast(ray, out float enter);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position, direction2.normalized * enter);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, -vector2, angle / 90f).normalized * enter);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, vector3, angle / 90f).normalized * enter);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, -vector3, angle / 90f).normalized * enter);
		DrawCircle(position + vector, direction, color, (vector - direction2.normalized * enter).magnitude);
		DrawCircle(position + vector * 0.5f, direction, color, (vector * 0.5f - direction2.normalized * (enter * 0.5f)).magnitude);
		Gizmos.color = color2;
	}

	public static void DrawCone(Vector3 position, Vector3 direction, float angle = 45f)
	{
		DrawCone(position, direction, Color.white, angle);
	}

	public static void DrawCone(Vector3 position, Color color, float angle = 45f)
	{
		DrawCone(position, Vector3.up, color, angle);
	}

	public static void DrawCone(Vector3 position, float angle = 45f)
	{
		DrawCone(position, Vector3.up, Color.white, angle);
	}

	public static void DrawArrow(Vector3 position, Vector3 direction, Color color)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position, direction);
		DrawCone(position + direction, -direction * 0.333f, color, 15f);
		Gizmos.color = color2;
	}

	public static void DrawArrow(Vector3 position, Vector3 direction)
	{
		DrawArrow(position, direction, Color.white);
	}

	public static void DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1f)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		float magnitude = (start - end).magnitude;
		float d = Mathf.Max(0f, magnitude * 0.5f - radius);
		Vector3 vector4 = (end + start) * 0.5f;
		start = vector4 + (start - vector4).normalized * d;
		end = vector4 + (end - vector4).normalized * d;
		DrawCircle(start, vector, color, radius);
		DrawCircle(end, -vector, color, radius);
		Gizmos.DrawLine(start + vector3, end + vector3);
		Gizmos.DrawLine(start - vector3, end - vector3);
		Gizmos.DrawLine(start + vector2, end + vector2);
		Gizmos.DrawLine(start - vector2, end - vector2);
		for (int i = 1; i < 26; i++)
		{
			Gizmos.DrawLine(Vector3.Slerp(vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(vector3, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(-vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector3, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(vector2, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(-vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector2, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(vector3, vector, (float)i / 25f) + end, Vector3.Slerp(vector3, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(-vector3, vector, (float)i / 25f) + end, Vector3.Slerp(-vector3, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(vector2, vector, (float)i / 25f) + end, Vector3.Slerp(vector2, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(-vector2, vector, (float)i / 25f) + end, Vector3.Slerp(-vector2, vector, (float)(i - 1) / 25f) + end);
		}
		Gizmos.color = color2;
	}

	public static void DrawCapsule(Vector3 start, Vector3 end, float radius = 1f)
	{
		DrawCapsule(start, end, Color.white, radius);
	}

	public static string MethodsOfObject(object obj, bool includeInfo = false)
	{
		string text = string.Empty;
		MethodInfo[] methods = obj.GetType().GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			text = ((!includeInfo) ? (text + methods[i].Name + "\n") : (text + methods[i] + "\n"));
		}
		return text;
	}

	public static string MethodsOfType(Type type, bool includeInfo = false)
	{
		string text = string.Empty;
		MethodInfo[] methods = type.GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			text = ((!includeInfo) ? (text + methods[i].Name + "\n") : (text + methods[i] + "\n"));
		}
		return text;
	}
}
