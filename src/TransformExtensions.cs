using UnityEngine;

public static class TransformExtensions
{
	public static Transform FindRecursive(this Transform transform, string name)
	{
		if (transform.name.Equals(name))
		{
			return transform;
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform transform2 = transform.GetChild(i).FindRecursive(name);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}
}
