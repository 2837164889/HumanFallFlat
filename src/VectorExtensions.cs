using UnityEngine;

public static class VectorExtensions
{
	public static Vector3 To3D(this Vector2 v2)
	{
		return new Vector3(v2.x, 0f, v2.y);
	}

	public static Vector3 To3D(this Vector2 v2, float y)
	{
		return new Vector3(v2.x, y, v2.y);
	}

	public static Vector3 ZeroY(this Vector3 v2)
	{
		return new Vector3(v2.x, 0f, v2.z);
	}

	public static Vector3 InvertZ(this Vector3 v2)
	{
		return new Vector3(v2.x, v2.y, 0f - v2.z);
	}

	public static Vector3 SetY(this Vector3 v2, float y)
	{
		return new Vector3(v2.x, y, v2.z);
	}

	public static Vector3 SetX(this Vector3 v2, float x)
	{
		return new Vector3(x, v2.y, v2.z);
	}

	public static Vector3 SetZ(this Vector3 v2, float z)
	{
		return new Vector3(v2.x, v2.y, z);
	}

	public static Vector2 To2D(this Vector3 v3)
	{
		return new Vector2(v3.x, v3.z);
	}

	public static Vector3 XZtoXY(this Vector3 v3)
	{
		return new Vector3(v3.x, v3.z, 0f - v3.y);
	}

	public static Vector3 XYtoXZ(this Vector3 v3)
	{
		return new Vector3(v3.x, 0f - v3.z, v3.y);
	}

	public static Vector2 Rotate(this Vector2 p, float angle)
	{
		float num = Mathf.Cos(angle);
		float num2 = Mathf.Sin(angle);
		return new Vector2(p.x * num - p.y * num2, p.x * num2 + p.y * num);
	}

	public static Vector3 Rotate(this Vector3 p, Vector3 axis, float angle)
	{
		return Quaternion.AngleAxis(angle, axis) * p;
	}

	public static Vector3 RotateYCW90(this Vector3 p)
	{
		return new Vector3(p.z, p.y, 0f - p.x);
	}

	public static Vector3 RotateY(this Vector3 p, float angle)
	{
		float num = Mathf.Cos(angle);
		float num2 = Mathf.Sin(angle);
		return new Vector3(p.x * num - p.z * num2, p.y, p.x * num2 + p.z * num);
	}

	public static Vector3 RotateZCW90(this Vector3 p)
	{
		return new Vector3(p.y, 0f - p.x, p.z);
	}

	public static Vector2 RotateCW90(this Vector2 p)
	{
		return new Vector2(p.y, 0f - p.x);
	}
}
