using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetIdentity))]
public class MechScriptTreeCutting1 : MonoBehaviour, IReset
{
	private enum HitType
	{
		TooWeak,
		Cut
	}

	[Tooltip("The collider the tree will look for when being chopped down")]
	public Collider axe;

	[Tooltip("The Stump of the tree so this knows to ignore its collision")]
	public Collider stump;

	[Tooltip("The direction the tree is pushed in when cut down")]
	public Vector3 pushDirection;

	[Tooltip("The location the tree is pushed towards when cut down")]
	public Vector3 pushLocation;

	[Tooltip("The amount of cuts needed to make the tree fall")]
	public int cutsRequired = 3;

	[Tooltip("The amount of force needed to make a cut")]
	public float cutForce = 2f;

	[Tooltip("The FX drawn when there is a cut")]
	public GameObject cutFxTemplate;

	[Tooltip("Length of the list of the effects use")]
	public int poolFxItemCount = 5;

	private List<GameObject> poolObjects = new List<GameObject>();

	private bool isCut;

	private bool isOnStump;

	private int cutCount;

	private SignalScriptNode1 graphNode;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private uint evtHit;

	private NetIdentity identity;

	private NetVector3Encoder posEncoder = new NetVector3Encoder(500f, 18, 4, 8);

	public bool IsCut => isCut;

	private void Start()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Started ");
		}
		identity = GetComponent<NetIdentity>();
		evtHit = identity.RegisterEvent(OnHit);
	}

	private void OnEnable()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Enabled ");
		}
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
		if (showDebug)
		{
			Debug.Log(base.name + " Collision Enter ");
		}
		if (!isOnStump && collision.collider == stump)
		{
			isOnStump = true;
			Rigidbody component = GetComponent<Rigidbody>();
			if (component != null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Tree is not null ");
				}
				component.isKinematic = true;
			}
		}
		if (isCut || !(collision.collider == axe))
		{
			return;
		}
		if (showDebug)
		{
			Debug.Log(base.name + " Hit by the Axe ");
		}
		if (collision.relativeVelocity.magnitude > cutForce)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Hit Hard enough ");
			}
			cutCount++;
			SendHit(HitType.Cut, collision.GetPoint());
			if (cutCount < cutsRequired)
			{
				return;
			}
			if (showDebug)
			{
				Debug.Log(base.name + " No More cuts needed ");
			}
			isCut = true;
			Rigidbody component2 = GetComponent<Rigidbody>();
			if (component2 != null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Turning off kinematic flag ");
				}
				component2.isKinematic = false;
				component2.AddForceAtPosition(pushDirection, pushLocation);
			}
		}
		else
		{
			if (showDebug)
			{
				Debug.Log(base.name + " More Cuts needed ");
			}
			SendHit(HitType.TooWeak, collision.GetPoint());
		}
	}

	private void OnHit(NetStream stream)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " OnHit ");
		}
		HitType type = (HitType)stream.ReadUInt32(1);
		Vector3 position = posEncoder.ApplyState(stream);
		DoHit(type, position);
	}

	private void SendHit(HitType type, Vector3 position)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " SendHit ");
		}
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
		if (showDebug)
		{
			Debug.Log(base.name + " DoHit ");
		}
		foreach (GameObject poolObject in poolObjects)
		{
			if (!poolObject.activeSelf)
			{
				poolObject.transform.position = position;
				poolObject.SetActive(value: true);
				if (showDebug)
				{
					Debug.Log(base.name + " Trying to send signal now ");
				}
				graphNode = GetComponent<SignalScriptNode1>();
				graphNode.SendSignal(1f);
				break;
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " ResetState ");
		}
		isOnStump = false;
		isCut = false;
		cutCount = 0;
	}

	public void OnRespawn()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " OnRespawn ");
		}
		ResetState(0, 0);
	}
}
