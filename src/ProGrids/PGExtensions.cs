using UnityEngine;

namespace ProGrids
{
	public static class PGExtensions
	{
		public static bool Contains(this Transform[] t_arr, Transform t)
		{
			for (int i = 0; i < t_arr.Length; i++)
			{
				if (t_arr[i] == t)
				{
					return true;
				}
			}
			return false;
		}

		public static float Sum(this Vector3 v)
		{
			return v[0] + v[1] + v[2];
		}

		public static bool InFrustum(this Camera cam, Vector3 point)
		{
			Vector3 vector = cam.WorldToViewportPoint(point);
			return vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f && vector.z >= 0f;
		}
	}
}
