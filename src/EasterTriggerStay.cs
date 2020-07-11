using Multiplayer;
using UnityEngine;

public class EasterTriggerStay : MonoBehaviour
{
	public float time = 2f;

	private float timeGrabbed;

	private Human humanInside;

	private uint evtCollision;

	private NetIdentity identity;

	private AudioSource audioSource;

	private float timeToCool;

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

	public void OnTriggerEnter(Collider other)
	{
		if (!(timeToCool > 0f))
		{
			HumanHead component = other.GetComponent<HumanHead>();
			if (component != null)
			{
				humanInside = component.GetComponentInParent<Human>();
			}
		}
	}

	public void OnTriggerLeave(Collider other)
	{
		HumanHead component = other.GetComponent<HumanHead>();
		if (component != null)
		{
			humanInside = null;
		}
	}

	public void Update()
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
		if ((bool)humanInside)
		{
			flag = true;
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
				timeToCool = 60f;
				humanInside = null;
				timeGrabbed = 0f;
			}
		}
		else
		{
			timeGrabbed = 0f;
		}
	}
}
