using UnityEngine;

public static class MathUtils
{
	public static Vector3 WrapSigned(Vector3 value, Vector3 size)
	{
		return new Vector3(WrapSigned(value.x, size.x), WrapSigned(value.y, size.y), WrapSigned(value.z, size.z));
	}

	public static float WrapSigned(float value, float size)
	{
		return value - Mathf.Floor(value / size + 0.5f) * size;
	}

	public static Vector3 Wrap(Vector3 value, Vector3 size)
	{
		return new Vector3(Wrap(value.x, size.x), Wrap(value.y, size.y), Wrap(value.z, size.z));
	}

	public static float Wrap(float value, float size)
	{
		return value - Mathf.Floor(value / size) * size;
	}
}
