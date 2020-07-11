using Multiplayer;
using UnityEngine;

public class HumanHead : WaterSensor, INetBehavior
{
	public float drownTime = 10f;

	public float drownDepth = 3f;

	private float diveTime;

	private float nextBubble;

	public ServoSound sounds;

	public HumanAudio humanAudio;

	private uint underwaterState;

	private uint evtBubble;

	private NetIdentity identity;

	private void Update()
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < waterBodies.Count; i++)
		{
			if (!waterBodies[i].canDrown)
			{
				continue;
			}
			flag = true;
			Vector3 velocity;
			float num = waterBody.SampleDepth(base.transform.position, out velocity);
			if (num > 0f)
			{
				diveTime += Time.deltaTime;
				if (num > drownDepth)
				{
					diveTime += Time.deltaTime * num;
				}
				if (diveTime > drownTime)
				{
					Game.instance.Drown(GetComponentInParent<Human>());
					ExitWater(drown: true);
					underwaterState = 2u;
					break;
				}
				ApplyUnderwaterState(1u);
				if (diveTime > nextBubble)
				{
					PlayBubble(nextBubble / drownTime);
					while (diveTime > nextBubble)
					{
						nextBubble += 0.5f;
					}
				}
				underwaterState = 1u;
			}
			else
			{
				underwaterState = 0u;
				ExitWater(drown: false);
			}
			break;
		}
		if (!flag)
		{
			ExitWater(drown: false);
		}
	}

	private void PlayBubble(float drownPhase)
	{
		humanAudio.underwaterBubble.PlayOneShot(base.transform.position, 1f, 0.9f + 0.2f * drownPhase);
		if (!ReplayRecorder.isPlaying || !NetGame.isClient)
		{
			NetStream stream = identity.BeginEvent(evtBubble);
			NetSignal.encoder.CollectState(stream, drownPhase);
			identity.EndEvent();
		}
	}

	private void OnBubble(NetStream stream)
	{
		PlayBubble(NetSignal.encoder.ApplyState(stream));
	}

	private void ExitWater(bool drown)
	{
		if (diveTime < drownTime && diveTime > drownTime * 0.95f)
		{
			StatsAndAchievements.UnlockAchievement(Achievement.ACH_WATER_ALMOST_DROWN);
		}
		diveTime = 0f;
		nextBubble = 1f;
		ApplyUnderwaterState(drown ? 2u : 0u);
	}

	public void StartNetwork(NetIdentity identity)
	{
		this.identity = identity;
		evtBubble = identity.RegisterEvent(OnBubble);
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		underwaterState = 0u;
	}

	public void CollectState(NetStream stream)
	{
		stream.Write(underwaterState, 2);
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		state0.ReadUInt32(2);
		ApplyState(state1);
	}

	public void ApplyState(NetStream state)
	{
		uint under = state.ReadUInt32(2);
		ApplyUnderwaterState(under);
	}

	private void ApplyUnderwaterState(uint under)
	{
		if (underwaterState != under)
		{
			underwaterState = under;
			switch (under)
			{
			case 0u:
				StopUnderwaterSound(drown: false);
				break;
			case 1u:
				StartUnderwaterSound();
				break;
			case 2u:
				StopUnderwaterSound(drown: true);
				break;
			}
		}
	}

	private void StartUnderwaterSound()
	{
		if (!humanAudio.underwater.isPlaying)
		{
			humanAudio.underwater.Play();
		}
	}

	private void StopUnderwaterSound(bool drown)
	{
		if (humanAudio.underwater.isPlaying)
		{
			if (drown)
			{
				humanAudio.underwater.Switch('B', crossfade: false);
			}
			humanAudio.underwater.Stop();
			if (drown)
			{
				humanAudio.underwater.Switch('A', crossfade: false);
			}
		}
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		uint num = state0?.ReadUInt32(2) ?? 0;
		uint x = state1.ReadUInt32(2);
		delta.Write(x, 2);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		uint num = state0?.ReadUInt32(2) ?? 0;
		uint x = delta.ReadUInt32(2);
		result.Write(x, 2);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return 2;
	}
}
