using Multiplayer;
using UnityEngine;

public class Gift : GiftBase, INetBehavior
{
	public enum GiftPhase
	{
		Normal,
		Chute
	}

	private GiftGroup group;

	private NetBody netBody;

	private Rigidbody body;

	private GameObject chute;

	private float chuteHeight = 5f;

	private float chutePhase;

	private float targetHeight;

	public GiftPhase phase;

	private float spawnAfter;

	public static NetFloat encoder = new NetFloat(1f, 12, 4, 8);

	private void Start()
	{
		netBody = GetComponent<NetBody>();
		netBody.respawn = false;
		body = GetComponent<Rigidbody>();
		group = GetComponentInParent<GiftGroup>();
	}

	private void FixedUpdate()
	{
		if (!NetGame.isClient && !ReplayRecorder.isPlaying)
		{
			if (phase == GiftPhase.Normal)
			{
				HandleNormal();
			}
			if (phase == GiftPhase.Chute)
			{
				HandleChute();
			}
		}
	}

	private void HandleNormal()
	{
		Vector3 position = base.transform.position;
		if (position.y < netBody.despawnHeight + 10f)
		{
			GiftSpawn randomSpawn = group.GetRandomSpawn();
			Vector3 position2 = randomSpawn.GetPosition();
			netBody.Respawn(position2 - netBody.startPos + Vector3.up * Random.Range(20, 30));
			phase = GiftPhase.Chute;
			targetHeight = position2.y;
		}
	}

	private void HandleChute()
	{
		float num = 0f;
		Vector3 position = base.transform.position;
		float num2 = position.y - targetHeight;
		num = ((!(num2 < 3f)) ? Mathf.InverseLerp(15f, 5f, num2) : Mathf.InverseLerp(2f, 3f, num2));
		if (num2 < 2f)
		{
			phase = GiftPhase.Normal;
		}
		else
		{
			Vector3 velocity = body.velocity;
			if (velocity.y < -1f * (2f - num))
			{
				body.SafeAddForce(Vector3.up * 50f * num, ForceMode.Acceleration);
			}
		}
		ApplyChutePhase(num);
	}

	private void ApplyChutePhase(float phase)
	{
		if (phase == 0f)
		{
			if (chute != null)
			{
				GiftParachute.instance.Release(chute);
				chute = null;
			}
			chutePhase = phase;
			return;
		}
		if (chute == null)
		{
			chute = GiftParachute.instance.Allocate();
			GiftParachute.instance.PlaySound(base.transform.position);
		}
		chutePhase = phase;
		chute.transform.position = base.transform.position;
		chute.transform.localScale = new Vector3(chutePhase, chutePhase, Mathf.Lerp(1.2f, 1f, chutePhase));
	}

	public void StartNetwork(NetIdentity identity)
	{
		giftId = identity.sceneId;
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
	}

	public void CollectState(NetStream stream)
	{
		encoder.CollectState(stream, chutePhase);
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		float num = encoder.ApplyLerpedState(state0, state1, mix);
		ApplyChutePhase(num);
	}

	public void ApplyState(NetStream state)
	{
		float num = encoder.ApplyState(state);
		ApplyChutePhase(num);
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		encoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		encoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return encoder.CalculateMaxDeltaSizeInBits();
	}
}
