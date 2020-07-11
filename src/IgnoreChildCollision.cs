using UnityEngine;

public class IgnoreChildCollision : MonoBehaviour
{
	private void Awake()
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = i; j < componentsInChildren.Length; j++)
			{
				Physics.IgnoreCollision(componentsInChildren[i], componentsInChildren[j]);
			}
		}
	}
}
