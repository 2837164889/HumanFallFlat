using Multiplayer;
using UnityEngine;

public class EasterGrab : MonoBehaviour
{
	public GameObject toGrab;

	public float time = 10f;

	public float dist;

	private float timeGrabbed;

	private float timeToCool;

	private uint evtCollision;

	private NetIdentity identity;

	private AudioSource audioSource;

	public void Start()
	{
		audioSource = GetComponent<AudioSource>();
		identity = GetComponent<NetIdentity>();
		if (identity != null)
		{
			evtCollision = identity.RegisterEvent(OnPlayEasterEggAudio);
		}
	}

	private void OnPlayEasterEggAudio(NetStream stream)
	{
		if ((bool)audioSource)
		{
			audioSource.Play();
		}
	}

	private void Update()
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		if (timeToCool > 0f)
		{
			timeToCool -= Time.deltaTime;
			return;
		}
		bool flag = false;
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			Ragdoll ragdoll = human.ragdoll;
			if ((toGrab == null || ragdoll.partLeftHand.sensor.grabObject == toGrab) && (dist == 0f || (ragdoll.partLeftHand.transform.position - base.transform.position).magnitude < dist))
			{
				flag = true;
			}
			if ((toGrab == null || ragdoll.partRightHand.sensor.grabObject == toGrab) && (dist == 0f || (ragdoll.partRightHand.transform.position - base.transform.position).magnitude < dist))
			{
				flag = true;
			}
		}
		if (flag)
		{
			timeGrabbed += Time.deltaTime;
			if (timeGrabbed > time)
			{
				if ((bool)audioSource)
				{
					audioSource.Play();
				}
				if ((bool)identity)
				{
					identity.BeginEvent(evtCollision);
					identity.EndEvent();
				}
				timeToCool = 240f;
				timeGrabbed = 0f;
			}
		}
		else
		{
			timeGrabbed = 0f;
		}
	}
}
