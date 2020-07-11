using Multiplayer;
using UnityEngine;

public class EasterTrigger : MonoBehaviour
{
	public Collider acceptedCollider;

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
		if (ReplayRecorder.isPlaying || NetGame.isClient || timeToCool > 0f)
		{
			return;
		}
		bool flag = false;
		if (acceptedCollider == null)
		{
			foreach (Human item in Human.all)
			{
				if (item.GetComponent<Collider>() == other)
				{
					flag = true;
				}
			}
		}
		else if (other == acceptedCollider)
		{
			flag = true;
		}
		if (flag)
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
		}
	}

	private void Update()
	{
		if (timeToCool > 0f)
		{
			timeToCool -= Time.deltaTime;
		}
	}
}
