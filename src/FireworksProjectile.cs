using HumanAPI;
using Multiplayer;
using UnityEngine;

public class FireworksProjectile : MonoBehaviour, INetBehavior
{
	public enum FireworkState
	{
		Inactive,
		Shot,
		Exploded
	}

	public GameObject visual;

	public Sound2 shootSound;

	public Sound2 explodeSound;

	public ParticleSystem flyParticles;

	public ParticleSystem explodeParticles;

	public Light light;

	private Rigidbody body;

	private FireworkState state;

	private float life;

	private const float explodeIn = 2f;

	private const float terminateIn = 5f;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (NetGame.isClient || ReplayRecorder.isPlaying)
		{
			return;
		}
		if (state == FireworkState.Shot)
		{
			body.AddForce(base.transform.forward, ForceMode.Acceleration);
			body.AddForce(Vector3.up * 5f, ForceMode.Acceleration);
		}
		if (state == FireworkState.Shot)
		{
			life += Time.fixedDeltaTime / 2f;
			if (life >= 1f)
			{
				Explode();
			}
			else
			{
				Apply(state, life);
			}
		}
		else if (state == FireworkState.Exploded)
		{
			life += Time.fixedDeltaTime / 5f;
			if (life >= 1f)
			{
				Terminate();
			}
			else
			{
				Apply(state, life);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!NetGame.isClient && !ReplayRecorder.isPlaying && state == FireworkState.Shot)
		{
			Explode();
		}
	}

	public void Shoot(Vector3 pos, Vector3 dir)
	{
		base.gameObject.SetActive(value: true);
		base.transform.position = pos;
		base.transform.rotation = Quaternion.LookRotation(dir);
		body.angularVelocity = Vector3.zero;
		body.velocity = base.transform.forward * 20f;
		Apply(FireworkState.Shot, 0f);
	}

	public void Shoot(Transform muzzle)
	{
		Shoot(muzzle.position, muzzle.forward);
	}

	private void Explode()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 2f);
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody componentInParent = array[i].GetComponentInParent<Rigidbody>();
			if (componentInParent != null && !componentInParent.isKinematic)
			{
				componentInParent.AddExplosionForce(1500f * Mathf.Pow(componentInParent.mass, 0.25f), base.transform.position, 8f);
				Human componentInParent2 = componentInParent.GetComponentInParent<Human>();
				if (componentInParent2 != null)
				{
					componentInParent2.MakeUnconscious(0.5f);
				}
			}
		}
		Apply(FireworkState.Exploded, 0f);
	}

	private void Terminate()
	{
		base.transform.position = -200f * Vector3.up;
		Fireworks.instance.Kill(this);
		Apply(FireworkState.Inactive, 0f);
	}

	private void Apply(FireworkState newState, float newPhase, bool manageLifetime = false)
	{
		life = newPhase;
		if (newState != state)
		{
			state = newState;
			if (state == FireworkState.Shot)
			{
				visual.SetActive(value: true);
				flyParticles.enableEmission = true;
				light.range = 5f;
				if (life <= 0.5f)
				{
					shootSound.PlayOneShot(Fireworks.instance.transform.position);
				}
			}
			else if (state == FireworkState.Exploded)
			{
				visual.SetActive(value: false);
				flyParticles.enableEmission = false;
				if (life <= 0.5f)
				{
					explodeParticles.Emit(128);
					explodeSound.PlayOneShot(base.transform.position);
				}
			}
		}
		float num = (state != FireworkState.Exploded) ? 5f : Mathf.Lerp(50f, 0f, life * 10f);
		if (light.range != num)
		{
			light.range = num;
		}
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
	}

	public void CollectState(NetStream stream)
	{
		NetBoolEncoder.CollectState(stream, state == FireworkState.Shot);
		NetBoolEncoder.CollectState(stream, state == FireworkState.Exploded);
		NetSignal.encoder.CollectState(stream, life);
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		bool flag = NetBoolEncoder.ApplyLerpedState(state0, state1, mix);
		bool flag2 = NetBoolEncoder.ApplyLerpedState(state0, state1, mix);
		FireworkState fireworkState = flag ? FireworkState.Shot : (flag2 ? FireworkState.Exploded : FireworkState.Inactive);
		float newPhase = NetSignal.encoder.ApplyLerpedState(state0, state1, mix);
		if (fireworkState != 0 && state == FireworkState.Inactive)
		{
			Fireworks.instance.MarkUsed(this);
		}
		if (fireworkState == FireworkState.Inactive && state != 0)
		{
			Fireworks.instance.Kill(this);
		}
		Apply(fireworkState, newPhase);
	}

	public void ApplyState(NetStream state)
	{
		bool flag = NetBoolEncoder.ApplyState(state);
		bool flag2 = NetBoolEncoder.ApplyState(state);
		FireworkState fireworkState = flag ? FireworkState.Shot : (flag2 ? FireworkState.Exploded : FireworkState.Inactive);
		float newPhase = NetSignal.encoder.ApplyState(state);
		if (fireworkState != 0 && this.state == FireworkState.Inactive)
		{
			Fireworks.instance.MarkUsed(this);
		}
		if (fireworkState == FireworkState.Inactive && this.state != 0)
		{
			Fireworks.instance.Kill(this);
		}
		Apply(fireworkState, newPhase);
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
		NetSignal.encoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		NetBoolEncoder.AddDelta(state0, delta, result);
		NetBoolEncoder.AddDelta(state0, delta, result);
		NetSignal.encoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return 2 * NetBoolEncoder.CalculateMaxDeltaSizeInBits() + NetSignal.encoder.CalculateMaxDeltaSizeInBits();
	}
}
