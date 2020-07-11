using UnityEngine;

namespace HumanAPI
{
	public class IgnoreCollisionOverlap : MonoBehaviour
	{
		public Collider volumeCollider;

		public bool recursive = true;

		private void OnEnable()
		{
			volumeCollider.enabled = true;
			Bounds bounds = volumeCollider.bounds;
			Collider[] array = Physics.OverlapBox(bounds.center, bounds.extents);
			volumeCollider.enabled = false;
			if (recursive)
			{
				Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					for (int j = 0; j < array.Length; j++)
					{
						Physics.IgnoreCollision(componentsInChildren[i], array[j], ignore: true);
					}
				}
			}
			else
			{
				Collider component = GetComponent<Collider>();
				for (int k = 0; k < array.Length; k++)
				{
					Physics.IgnoreCollision(component, array[k], ignore: true);
				}
			}
		}
	}
}
