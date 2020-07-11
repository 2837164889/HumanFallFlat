using HumanAPI;
using Multiplayer;
using UnityEngine;

public class HumanAudio : MonoBehaviour
{
	public Sound2 underwater;

	public Sound2 underwaterBubble;

	public Sound2 grab;

	private Transform leftHand;

	private Transform rightHand;

	private uint evtHandGrab;

	private NetIdentity identity;

	public void Start()
	{
		Human componentInParent = GetComponentInParent<Human>();
		Ragdoll ragdoll = componentInParent.ragdoll;
		CollisionAudioSensor component = ragdoll.partLeftFoot.transform.GetComponent<CollisionAudioSensor>();
		CollisionAudioSensor component2 = ragdoll.partRightFoot.transform.GetComponent<CollisionAudioSensor>();
		leftHand = ragdoll.partLeftHand.transform;
		rightHand = ragdoll.partRightHand.transform;
		ragdoll.partLeftHand.sensor.onGrabTap = HandGrabLeft;
		ragdoll.partRightHand.sensor.onGrabTap = HandGrabRight;
		CollisionAudioSensor[] componentsInChildren = GetComponentsInChildren<CollisionAudioSensor>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].id = -i;
		}
		identity = GetComponentInParent<NetIdentity>();
		if (identity != null)
		{
			evtHandGrab = identity.RegisterEvent(OnHandGrab);
		}
	}

	private void HandGrabLeft(GameObject source, Vector3 pos, PhysicMaterial arg3, Vector3 arg4)
	{
		HandGrab(left: true);
	}

	private void HandGrabRight(GameObject source, Vector3 pos, PhysicMaterial arg3, Vector3 arg4)
	{
		HandGrab(left: false);
	}

	private void HandGrab(bool left)
	{
		grab.PlayOneShot((!left) ? rightHand.position : leftHand.position, Random.Range(0.5f, 1.5f), Random.Range(0.95f, 1.05f));
		if (NetGame.isServer || ReplayRecorder.isRecording)
		{
			NetStream netStream = identity.BeginEvent(evtHandGrab);
			netStream.Write(left);
			identity.EndEvent();
		}
	}

	public void OnHandGrab(NetStream stream)
	{
		HandGrab(stream.ReadBool());
	}
}
