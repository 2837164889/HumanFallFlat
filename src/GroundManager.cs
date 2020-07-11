using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
	public List<GameObject> groundObjects = new List<GameObject>();

	private List<Rigidbody> groundRigids = new List<Rigidbody>();

	private static List<GroundManager> all = new List<GroundManager>();

	private static Dictionary<GroundVehicle, Vector3> vehicleStartPositions = new Dictionary<GroundVehicle, Vector3>();

	private static Dictionary<FloatingMesh, Vector3> shipStartPositions = new Dictionary<FloatingMesh, Vector3>();

	private List<GameObject> removedObjects = new List<GameObject>();

	public float surfaceAngle;

	public bool onGround => groundObjects.Count > 0;

	public Vector3 groudSpeed
	{
		get
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < groundRigids.Count; i++)
			{
				Rigidbody rigidbody = groundRigids[i];
				if (rigidbody != null)
				{
					Vector3 velocity = rigidbody.velocity;
					if (Mathf.Abs(zero.x) < Mathf.Abs(velocity.x))
					{
						zero.x = velocity.x;
					}
					if (Mathf.Abs(zero.y) < Mathf.Abs(velocity.y))
					{
						zero.y = velocity.y;
					}
					if (Mathf.Abs(zero.z) < Mathf.Abs(velocity.z))
					{
						zero.z = velocity.z;
					}
				}
			}
			return zero;
		}
	}

	private void OnEnable()
	{
		all.Add(this);
	}

	private void OnDisable()
	{
		all.Remove(this);
	}

	public void PostFixedUpdate()
	{
		for (int i = 0; i < removedObjects.Count; i++)
		{
			if (shipStartPositions.Count > 0)
			{
				CheckDriveEnd(removedObjects[i], shipStartPositions);
			}
			if (vehicleStartPositions.Count > 0)
			{
				CheckDriveEnd(removedObjects[i], vehicleStartPositions);
			}
		}
		List<GameObject> list = removedObjects;
		removedObjects = groundObjects;
		groundObjects = list;
		groundObjects.Clear();
		groundRigids.Clear();
	}

	public void ObjectEnter(GameObject groundObject)
	{
		if (!groundObjects.Contains(groundObject))
		{
			removedObjects.Remove(groundObject);
			groundObjects.Add(groundObject);
			groundRigids.Add(groundObject.GetComponentInParent<Rigidbody>());
			FloatingMesh componentInParent = groundObject.GetComponentInParent<FloatingMesh>();
			if (componentInParent != null && !shipStartPositions.ContainsKey(componentInParent))
			{
				shipStartPositions[componentInParent] = groundObject.transform.position;
			}
			GroundVehicle componentInParent2 = groundObject.GetComponentInParent<GroundVehicle>();
			if (componentInParent2 != null && !vehicleStartPositions.ContainsKey(componentInParent2))
			{
				vehicleStartPositions[componentInParent2] = groundObject.transform.position;
			}
		}
	}

	private void CheckDriveUpdate<T>(Dictionary<T, Vector3> startPositions) where T : MonoBehaviour
	{
		for (int i = 0; i < groundObjects.Count; i++)
		{
			GameObject gameObject = groundObjects[i];
			if (gameObject == null)
			{
				continue;
			}
			T componentInParent = gameObject.GetComponentInParent<T>();
			if (!((Object)componentInParent != (Object)null) || !startPositions.ContainsKey(componentInParent))
			{
				continue;
			}
			float magnitude = (startPositions[componentInParent] - gameObject.transform.position).To2D().magnitude;
			if (magnitude > 5f)
			{
				if (typeof(T) == typeof(FloatingMesh))
				{
					StatsAndAchievements.AddShip(magnitude);
				}
				else
				{
					StatsAndAchievements.AddDrive(magnitude);
				}
				startPositions[componentInParent] = gameObject.transform.position;
			}
		}
	}

	public void Update()
	{
		if (shipStartPositions.Count > 0)
		{
			CheckDriveUpdate(shipStartPositions);
		}
		if (vehicleStartPositions.Count > 0)
		{
			CheckDriveUpdate(vehicleStartPositions);
		}
	}

	private static void CheckDriveEnd<T>(GameObject groundObject, Dictionary<T, Vector3> startPositions) where T : MonoBehaviour
	{
		T componentInParent = groundObject.GetComponentInParent<T>();
		if (!((Object)componentInParent != (Object)null) || !startPositions.ContainsKey(componentInParent))
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < all.Count; i++)
		{
			for (int j = 0; j < all[i].groundObjects.Count; j++)
			{
				GameObject gameObject = all[i].groundObjects[j];
				if (!(gameObject == null) && gameObject.transform.IsChildOf(componentInParent.transform))
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		float magnitude = (startPositions[componentInParent] - groundObject.transform.position).To2D().magnitude;
		if (magnitude > 0f)
		{
			if (typeof(T) == typeof(FloatingMesh))
			{
				StatsAndAchievements.AddShip(magnitude);
			}
			else
			{
				StatsAndAchievements.AddDrive(magnitude);
			}
		}
		startPositions.Remove(componentInParent);
	}

	public static bool IsStandingAny(GameObject item)
	{
		for (int i = 0; i < all.Count; i++)
		{
			if (all[i].IsStanding(item))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsStanding(GameObject item)
	{
		for (int num = groundObjects.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = groundObjects[num];
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

	public void DistributeForce(Vector3 force, Vector3 pos)
	{
		for (int i = 0; i < groundRigids.Count; i++)
		{
			Rigidbody rigidbody = groundRigids[i];
			if (rigidbody != null)
			{
				rigidbody.SafeAddForceAtPosition(Vector3.ClampMagnitude(force / groundRigids.Count, rigidbody.mass / Time.fixedDeltaTime * 10f), pos);
			}
		}
	}

	internal static void ResetOnSceneUnload()
	{
		foreach (GroundManager item in all)
		{
			item.groundRigids.Clear();
			item.groundObjects.Clear();
		}
		vehicleStartPositions.Clear();
		shipStartPositions.Clear();
	}

	internal void Reset()
	{
		GameObject[] array = groundObjects.ToArray();
		groundObjects.Clear();
		groundRigids.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[0];
			if (gameObject != null)
			{
				CheckDriveEnd(gameObject, shipStartPositions);
				CheckDriveEnd(gameObject, vehicleStartPositions);
			}
		}
	}

	public void ReportSurfaceAngle(float surfaceAngle)
	{
		this.surfaceAngle = Mathf.Min(surfaceAngle, this.surfaceAngle);
	}

	internal void DecaySurfaceAngle()
	{
		surfaceAngle = Mathf.Min(90f, surfaceAngle + 90f * Time.fixedDeltaTime);
	}
}
