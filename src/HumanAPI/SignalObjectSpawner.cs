using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class SignalObjectSpawner : Node, IReset
	{
		public NodeInput input;

		[Tooltip("Objects spawned will be copies of this object (Recommend you set it to inactive, objects will be activated on spawn!)")]
		public GameObject objectToDuplicate;

		[Tooltip("")]
		public Transform spawnLocation;

		public int maxSpawnedObjects = 5;

		private int nextObjToSpawn;

		private List<GameObject> spawnedObjects;

		private const string kNameSuffix = " (Managed by SignalObjectSpawner)";

		private void Awake()
		{
			spawnedObjects = new List<GameObject>();
			for (int i = 0; i < spawnLocation.childCount; i++)
			{
				GameObject gameObject = spawnLocation.GetChild(i).gameObject;
				if (gameObject.name.Contains(" (Managed by SignalObjectSpawner)"))
				{
					spawnedObjects.Add(gameObject);
				}
			}
		}

		private void OnValidate()
		{
		}

		public override void Process()
		{
			base.Process();
			if (input.value > 0.5f)
			{
				spawnedObjects[nextObjToSpawn].transform.localPosition = new Vector3(0f, 0f, 0f);
				SetObjectActive(spawnedObjects[nextObjToSpawn], active: true);
				nextObjToSpawn++;
				if (nextObjToSpawn == maxSpawnedObjects)
				{
					nextObjToSpawn = 0;
				}
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			foreach (GameObject spawnedObject in spawnedObjects)
			{
				spawnedObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				SetObjectActive(spawnedObject, active: false);
			}
		}

		private GameObject SpawnObjectEditor()
		{
			GameObject gameObject = Object.Instantiate(objectToDuplicate);
			gameObject.name = gameObject.name.Remove(gameObject.name.Length - 7) + " (Managed by SignalObjectSpawner)";
			gameObject.transform.parent = null;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.parent = spawnLocation;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			SetObjectActive(gameObject, active: false);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.isKinematic = false;
			}
			return gameObject;
		}

		private void SetObjectActive(GameObject obj, bool active)
		{
			NetBody[] componentsInChildren = obj.GetComponentsInChildren<NetBody>();
			NetBody[] array = componentsInChildren;
			foreach (NetBody netBody in array)
			{
				netBody.SetVisible(active);
				netBody.gameObject.SetActive(value: true);
			}
			obj.SetActive(active);
		}
	}
}
