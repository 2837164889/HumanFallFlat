using HumanAPI;
using Multiplayer;
using UnityEngine;

public class TrainWarning : MonoBehaviour
{
	public Gearbox trainGearbox;

	public Sound2 sound;

	public float cooldown = 3f;

	private float timeToCool;

	private NetIdentity identity;

	private uint evtBeep;

	private void Awake()
	{
		identity = GetComponentInParent<NetIdentity>();
		evtBeep = identity.RegisterEvent(OnBeep);
	}

	private void OnBeep(NetStream stream)
	{
		sound.PlayOneShot();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!NetGame.isClient && !ReplayRecorder.isPlaying && !(other.tag != "Player") && !(timeToCool > 0f) && trainGearbox.gear > 0)
		{
			timeToCool = cooldown;
			NetStream netStream = identity.BeginEvent(evtBeep);
			identity.EndEvent();
			OnBeep(null);
		}
	}

	private void FixedUpdate()
	{
		timeToCool -= Time.fixedDeltaTime;
	}
}
