using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class WaterBodyCollider : MonoBehaviour
{
	public sealed class RespawnDelay
	{
		public NetBody respawnObject;

		public float delay;
	}

	private List<WaterSensor> sensors = new List<WaterSensor>();

	public WaterBody waterBody;

	private CameraController3 cameraController;

	private WaterSensor cameraWaterSensor;

	private Vector3 oldCameraPosition;

	private Collider[] colliders;

	[SerializeField]
	private bool respawnNetBody;

	[SerializeField]
	private float respawnNetBodyDelay = 2f;

	private const int kSizeDelayObjectCache = 16;

	private static List<RespawnDelay> delayObjects;

	private static List<RespawnDelay> objectsToRespawn;

	private static bool waterMasterSet;

	private bool waterMaster;

	private void RespawnSetup()
	{
		objectsToRespawn = new List<RespawnDelay>(16);
		delayObjects = new List<RespawnDelay>(16);
		for (int i = 0; i < 16; i++)
		{
			delayObjects.Add(new RespawnDelay());
		}
	}

	private void RespawnAdd(NetBody body)
	{
		for (int i = 0; i < objectsToRespawn.Count; i++)
		{
			if (objectsToRespawn[i].respawnObject.Equals(body))
			{
				return;
			}
		}
		RespawnDelay respawnDelay = delayObjects[delayObjects.Count - 1];
		respawnDelay.respawnObject = body;
		respawnDelay.delay = respawnNetBodyDelay;
		objectsToRespawn.Add(respawnDelay);
		delayObjects.RemoveAt(delayObjects.Count - 1);
	}

	private void Awake()
	{
		if (!waterMasterSet)
		{
			waterMasterSet = true;
			waterMaster = true;
			RespawnSetup();
		}
	}

	private void RespawnProcess()
	{
		for (int num = objectsToRespawn.Count - 1; num >= 0; num--)
		{
			objectsToRespawn[num].delay -= Time.fixedDeltaTime;
			if (objectsToRespawn[num].delay < 0f)
			{
				NetBody respawnObject = objectsToRespawn[num].respawnObject;
				delayObjects.Add(objectsToRespawn[num]);
				objectsToRespawn.RemoveAt(num);
				respawnObject.Respawn();
			}
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (waterBody == null)
		{
			Debug.LogError("waterbody null", this);
		}
		WaterSensor component = other.gameObject.GetComponent<WaterSensor>();
		if (component != null)
		{
			component.OnEnterBody(waterBody);
			sensors.Add(component);
		}
		else
		{
			if (!respawnNetBody || NetGame.isClient)
			{
				return;
			}
			NetBody component2 = other.gameObject.GetComponent<NetBody>();
			if (component2 != null && component2.respawn)
			{
				MeshFilter component3 = other.gameObject.GetComponent<MeshFilter>();
				if (component3 != null)
				{
					RespawnAdd(component2);
				}
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		WaterSensor component = other.gameObject.GetComponent<WaterSensor>();
		if (component != null)
		{
			component.OnLeaveBody(waterBody);
			sensors.Remove(component);
		}
	}

	private void OnEnable()
	{
		if (waterBody == null)
		{
			waterBody = GetComponentInParent<WaterBody>();
		}
		if (NetGame.isClient)
		{
			cameraController = Object.FindObjectOfType<CameraController3>();
			cameraWaterSensor = cameraController.gameObject.GetComponent<WaterSensor>();
			oldCameraPosition = cameraController.transform.position;
			colliders = GetComponents<Collider>();
		}
	}

	public void OnDisable()
	{
		for (int i = 0; i < sensors.Count; i++)
		{
			sensors[i].OnLeaveBody(waterBody);
		}
		sensors.Clear();
		if (NetGame.isClient)
		{
			cameraController = null;
			cameraWaterSensor = null;
			colliders = null;
		}
	}

	private void FixedUpdate()
	{
		if (waterMaster)
		{
			RespawnProcess();
		}
	}

	private void Update()
	{
		if (!NetGame.isClient || !(cameraWaterSensor != null) || !(waterBody != null) || colliders == null)
		{
			return;
		}
		Vector3 position = cameraController.transform.position;
		float magnitude = (position - oldCameraPosition).magnitude;
		if (!(magnitude > 0.0001f))
		{
			return;
		}
		Vector3 normalized = (position - oldCameraPosition).normalized;
		Ray ray = new Ray(oldCameraPosition, normalized);
		Ray ray2 = new Ray(position, -normalized);
		bool flag = sensors.Contains(cameraWaterSensor);
		Collider[] array = colliders;
		foreach (Collider collider in array)
		{
			bool flag2 = collider.bounds.Contains(position);
			if ((!flag2 && flag) || ((flag2 || flag) && (collider.Raycast(ray, out RaycastHit hitInfo, magnitude) || collider.Raycast(ray2, out hitInfo, magnitude))))
			{
				if (flag)
				{
					cameraWaterSensor.OnLeaveBody(waterBody);
					sensors.Remove(cameraWaterSensor);
				}
				else
				{
					cameraWaterSensor.OnEnterBody(waterBody);
					sensors.Add(cameraWaterSensor);
				}
			}
		}
		oldCameraPosition = position;
	}
}
