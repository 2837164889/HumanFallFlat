using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
	[Tooltip("List of the colliders to ignore")]
	public Collider[] collidersToIgnore;

	[Tooltip("Whether the check is recursive or not")]
	public bool recursive = true;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Started ");
		}
		if (recursive)
		{
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Ignore(componentsInChildren[i]);
			}
		}
		else
		{
			Collider component = GetComponent<Collider>();
			Ignore(component);
		}
	}

	private void Ignore(Collider collider)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Ignoring ");
		}
		if (collidersToIgnore == null && !(collider != null))
		{
			return;
		}
		for (int i = 0; i < collidersToIgnore.Length; i++)
		{
			if (collider != null)
			{
				if (collidersToIgnore[i] == null)
				{
					Debug.LogWarning("Colliders to ignore has null objects in list", this);
				}
				else
				{
					Physics.IgnoreCollision(collider, collidersToIgnore[i], ignore: true);
				}
			}
		}
	}

	public static void Ignore(Transform t1, Transform t2)
	{
		Collider[] componentsInChildren = t1.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = t2.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Physics.IgnoreCollision(componentsInChildren[i], componentsInChildren2[j], ignore: true);
			}
		}
	}

	public static void Unignore(Transform t1, Transform t2)
	{
		Collider[] componentsInChildren = t1.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = t2.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Physics.IgnoreCollision(componentsInChildren[i], componentsInChildren2[j], ignore: false);
			}
		}
	}
}
