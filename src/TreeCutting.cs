using HumanAPI;
using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetIdentity))]
public class TreeCutting : MonoBehaviour, IReset
{
	private enum HitType
	{
		TooWeak,
		Cut
	}

	public Collider axe;

	public Collider stump;

	public Vector3 pushDirection;

	public Vector3 pushLocation;

	public int cutsRequired = 3;

	public float cutForce = 4f;

	public Sound2 cutSound;

	public GameObject cutFxTemplate;

	public int poolFxItemCount = 5;

	private List<GameObject> poolObjects = new List<GameObject>();

	private bool isCut;

	private bool isOnStump;

	private int cutCount;

	private uint evtHit;

	private NetIdentity identity;

	private NetVector3Encoder posEncoder = new NetVector3Encoder(500f, 18, 4, 8);

	public bool IsCut => isCut;

	private void Start()
	{
		identity = GetComponent<NetIdentity>();
		evtHit = identity.RegisterEvent(OnHit);
	}

	private void OnEnable()
	{
		if ((bool)cutFxTemplate)
		{
			for (int i = 0; i < poolFxItemCount; i++)
			{
				GameObject gameObject = Object.Instantiate(cutFxTemplate);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				gameObject.SetActive(value: false);
				poolObjects.Add(gameObject);
			}
		}
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!isOnStump && collision.collider == stump)
		{
			isOnStump = true;
			Rigidbody component = GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = true;
			}
		}
		if (isCut || !(collision.collider == axe))
		{
			return;
		}
		if (collision.relativeVelocity.magnitude > cutForce)
		{
			cutCount++;
			SendHit(HitType.Cut, collision.GetPoint());
			if (cutCount >= cutsRequired)
			{
				if (cutSound != null)
				{
					cutSound.Play();
				}
				isCut = true;
				Rigidbody component2 = GetComponent<Rigidbody>();
				if (component2 != null)
				{
					component2.isKinematic = false;
					component2.AddForceAtPosition(pushDirection, pushLocation);
				}
			}
		}
		else
		{
			SendHit(HitType.TooWeak, collision.GetPoint());
		}
	}

	private void OnHit(NetStream stream)
	{
		HitType type = (HitType)stream.ReadUInt32(1);
		Vector3 position = posEncoder.ApplyState(stream);
		DoHit(type, position);
	}

	private void SendHit(HitType type, Vector3 position)
	{
		DoHit(type, position);
		if (NetGame.isServer || ReplayRecorder.isRecording)
		{
			NetStream netStream = identity.BeginEvent(evtHit);
			netStream.Write((uint)type, 1);
			posEncoder.CollectState(netStream, position);
			identity.EndEvent();
		}
	}

	private void DoHit(HitType type, Vector3 position)
	{
		foreach (GameObject poolObject in poolObjects)
		{
			if (!poolObject.activeSelf)
			{
				poolObject.transform.position = position;
				poolObject.SetActive(value: true);
				break;
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		isOnStump = false;
		isCut = false;
		cutCount = 0;
	}

	public void OnRespawn()
	{
		ResetState(0, 0);
	}
}
