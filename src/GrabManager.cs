using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
	public List<GameObject> grabbedObjects = new List<GameObject>();

	private static Dictionary<GameObject, Vector3> grabStartPositions = new Dictionary<GameObject, Vector3>();

	private static List<GrabManager> all = new List<GrabManager>();

	private Human human;

	public bool hasGrabbed => grabbedObjects.Count > 0;

	private void OnEnable()
	{
		all.Add(this);
		human = GetComponentInParent<Human>();
	}

	private void OnDisable()
	{
		all.Remove(this);
	}

	public void ObjectGrabbed(GameObject grabObject)
	{
		bool flag = true;
		for (int i = 0; i < all.Count; i++)
		{
			flag &= !all[i].grabbedObjects.Contains(grabObject);
		}
		grabbedObjects.Add(grabObject);
		if (flag)
		{
			grabObject.GetComponentInParent<IGrabbable>()?.OnGrab();
			grabStartPositions[grabObject] = grabObject.transform.position;
			Human componentInParent = grabObject.GetComponentInParent<Human>();
			if (componentInParent != null)
			{
				componentInParent.grabbedByHuman = human;
			}
		}
	}

	private void CheckCarryEnd(GameObject grabObject)
	{
		bool flag = true;
		for (int i = 0; i < all.Count; i++)
		{
			flag &= !all[i].grabbedObjects.Contains(grabObject);
		}
		if (!flag)
		{
			return;
		}
		grabObject.GetComponentInParent<IGrabbable>()?.OnRelease();
		if (grabObject != null && grabStartPositions.ContainsKey(grabObject))
		{
			float magnitude = (grabStartPositions[grabObject] - grabObject.transform.position).To2D().magnitude;
			if (magnitude > 0.1f)
			{
				StatsAndAchievements.AddCarry(human, magnitude);
			}
		}
		grabStartPositions.Remove(grabObject);
		if (!CheatCodes.throwCheat)
		{
			Human componentInParent = grabObject.GetComponentInParent<Human>();
			if (componentInParent != null)
			{
				componentInParent.grabbedByHuman = null;
			}
		}
	}

	public void ObjectReleased(GameObject grabObject)
	{
		grabbedObjects.Remove(grabObject);
		CheckCarryEnd(grabObject);
	}

	public static void Release(GameObject item, float delay = 0f)
	{
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human.all[i].ReleaseGrab(item, delay);
		}
	}

	public static bool IsGrabbedAny(GameObject item)
	{
		for (int i = 0; i < all.Count; i++)
		{
			if (all[i].IsGrabbed(item))
			{
				return true;
			}
		}
		return false;
	}

	public static Human GrabbedBy(GameObject item)
	{
		for (int i = 0; i < all.Count; i++)
		{
			if (all[i].IsGrabbed(item))
			{
				return all[i].human;
			}
		}
		return null;
	}

	public bool IsGrabbed(GameObject item)
	{
		for (int num = grabbedObjects.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = grabbedObjects[num];
			if (!(gameObject == null))
			{
				Transform transform = gameObject.transform;
				while (transform != null)
				{
					if (transform.gameObject == item)
					{
						return true;
					}
					transform = transform.parent;
				}
			}
		}
		return false;
	}

	public void DistributeForce(Vector3 force)
	{
		for (int i = 0; i < grabbedObjects.Count; i++)
		{
			Rigidbody componentInParent = grabbedObjects[i].GetComponentInParent<Rigidbody>();
			if (componentInParent != null)
			{
				componentInParent.SafeAddForce(force / grabbedObjects.Count);
			}
		}
	}

	internal void Reset()
	{
		GameObject[] array = grabbedObjects.ToArray();
		grabbedObjects.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[0];
			if (gameObject != null)
			{
				CheckCarryEnd(gameObject);
			}
		}
	}
}
