using Multiplayer;
using UnityEngine;

public abstract class ShatterBase : MonoBehaviour, INetBehavior
{
	private struct NetState
	{
		public NetVector3 shatterPos;

		public uint shatterMagnitude;

		public uint shatterSeed;

		public uint netId;

		public void Write(NetStream stream)
		{
			stream.WriteNetId(netId);
			shatterPos.Write(stream);
			stream.Write(shatterMagnitude, 10);
			stream.Write(shatterSeed, 10);
		}

		public static NetState Read(NetStream stream)
		{
			NetState result = default(NetState);
			result.netId = stream.ReadNetId();
			result.shatterPos = NetVector3.Read(stream, 10);
			result.shatterMagnitude = stream.ReadUInt32(10);
			result.shatterSeed = stream.ReadUInt32(10);
			return result;
		}
	}

	protected MeshRenderer renderer;

	protected ShatterAudio audio;

	protected ShatterAudioAudioSource audioSource;

	protected Collider collider;

	public float impulseTreshold = 10f;

	public float breakTreshold = 100f;

	public float accumulatedBreakTreshold = 300f;

	public float humanHardness;

	public bool shattered;

	private Vector3 adjustedImpulse = Vector3.zero;

	private Vector3 lastFrameImpact;

	private Vector3 maxImpactPoint;

	private float maxImpact;

	public float accumulatedImpact;

	public float maxMomentum;

	private NetIdentity identity;

	private uint evtCrack;

	private bool isMaster;

	private NetState netState;

	protected virtual void OnEnable()
	{
		if (GetComponent<NetIdentity>() == null)
		{
			Debug.LogError("Missing NetIdentity", this);
		}
		audio = GetComponent<ShatterAudio>();
		collider = GetComponent<Collider>();
		audioSource = GetComponent<ShatterAudioAudioSource>();
		renderer = GetComponentInChildren<MeshRenderer>();
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!shattered)
		{
			HandleCollision(collision, enter: false);
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		if (!shattered)
		{
			HandleCollision(collision, enter: false);
		}
	}

	private void FixedUpdate()
	{
		if (shattered || ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		maxImpact = 0f;
		if (adjustedImpulse.magnitude > maxMomentum)
		{
			maxMomentum = adjustedImpulse.magnitude;
		}
		accumulatedImpact += adjustedImpulse.magnitude;
		if (accumulatedImpact > accumulatedBreakTreshold)
		{
			shattered = true;
		}
		if ((adjustedImpulse + lastFrameImpact).magnitude > breakTreshold)
		{
			shattered = true;
		}
		if (shattered)
		{
			if (maxImpactPoint == Vector3.zero)
			{
				maxImpactPoint = collider.bounds.center;
			}
			ShatterLocal(maxImpactPoint, adjustedImpulse + lastFrameImpact);
		}
		else if (adjustedImpulse != Vector3.zero)
		{
			if (NetGame.isServer || ReplayRecorder.isRecording)
			{
				SendCrack(adjustedImpulse, maxImpactPoint);
			}
			PlayCrack(adjustedImpulse, maxImpactPoint);
		}
		lastFrameImpact = adjustedImpulse;
		adjustedImpulse = Vector3.zero;
	}

	private void PlayCrack(Vector3 adjustedImpulse, Vector3 maxImpactPoint)
	{
		if (!shattered)
		{
			if (audio != null)
			{
				audio.Crack(adjustedImpulse.magnitude, maxImpactPoint);
			}
			if (audioSource != null)
			{
				audioSource.Crack(adjustedImpulse.magnitude, maxImpactPoint);
			}
			VoronoiTriangulate component = GetComponent<VoronoiTriangulate>();
			if (component != null)
			{
				component.Deform(maxImpactPoint, adjustedImpulse / breakTreshold);
			}
		}
	}

	private void SendCrack(Vector3 adjustedImpulse, Vector3 maxImpactPoint)
	{
		NetStream stream = identity.BeginEvent(evtCrack);
		NetVector3.Quantize(maxImpactPoint - base.transform.position, 100f, 16).Write(stream, 6);
		NetVector3.Quantize(adjustedImpulse / breakTreshold, 10f, 16).Write(stream, 6);
		identity.EndEvent();
	}

	private void OnCrack(NetStream stream)
	{
		Vector3 vector = NetVector3.Read(stream, 6, 16).Dequantize(100f) + base.transform.position;
		Vector3 vector2 = NetVector3.Read(stream, 6, 16).Dequantize(10f) * breakTreshold;
		PlayCrack(vector2, vector);
	}

	private void HandleCollision(Collision collision, bool enter)
	{
		if (collision.contacts.Length == 0 || ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		Vector3 a = -collision.GetImpulse();
		float magnitude = a.magnitude;
		if (magnitude < impulseTreshold)
		{
			return;
		}
		float num = 1f;
		Transform transform = collision.transform;
		while (transform != null)
		{
			if ((bool)transform.GetComponent<Human>())
			{
				num = humanHardness;
				break;
			}
			ShatterHardness component = transform.GetComponent<ShatterHardness>();
			if (component != null)
			{
				num = component.hardness;
				break;
			}
			transform = transform.parent;
		}
		if (magnitude * num > maxImpact)
		{
			maxImpact = magnitude * num;
			maxImpactPoint = collision.contacts[0].point;
		}
		adjustedImpulse += a * num;
	}

	protected virtual void Shatter(Vector3 contactPoint, Vector3 adjustedImpulse, float impactMagnitude, uint seed, uint netId)
	{
		renderer.enabled = false;
		collider.enabled = false;
		if (audio != null)
		{
			audio.Shatter(impactMagnitude, contactPoint);
		}
		if (audioSource != null)
		{
			audioSource.Shatter(impactMagnitude, contactPoint);
		}
		GrabManager.Release(base.gameObject);
	}

	public virtual void ResetState(int checkpoint, int subObjectives)
	{
		renderer.enabled = true;
		collider.enabled = true;
		lastFrameImpact = (adjustedImpulse = Vector3.zero);
		accumulatedImpact = 0f;
		shattered = false;
	}

	public void StartNetwork(NetIdentity identity)
	{
		this.identity = identity;
		evtCrack = identity.RegisterEvent(OnCrack);
	}

	public void SetMaster(bool isMaster)
	{
		this.isMaster = isMaster;
	}

	public void CollectState(NetStream stream)
	{
		stream.Write(shattered);
		if (shattered)
		{
			netState.Write(stream);
		}
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		if (state1.ReadBool())
		{
			NetState.Read(state1);
		}
		ApplyState(state0);
	}

	public void ApplyState(NetStream state)
	{
		if (state.ReadBool())
		{
			netState = NetState.Read(state);
			if (!shattered)
			{
				ShatterNet(netState);
			}
		}
		else if (shattered)
		{
			ResetState(0, 0);
		}
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		bool flag = state0?.ReadBool() ?? false;
		bool flag2 = state1.ReadBool();
		NetState netState = (!flag) ? default(NetState) : NetState.Read(state0);
		NetState netState2 = (!flag2) ? default(NetState) : NetState.Read(state1);
		if (flag == flag2)
		{
			delta.Write(v: false);
			return;
		}
		delta.Write(v: true);
		if (flag2)
		{
			netState2.Write(delta);
		}
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		bool flag = state0?.ReadBool() ?? false;
		NetState netState = (!flag) ? default(NetState) : NetState.Read(state0);
		bool flag2 = delta.ReadBool() ^ flag;
		NetState netState2 = (!flag2 || flag) ? netState : NetState.Read(delta);
		result.Write(flag2);
		if (flag2)
		{
			netState2.Write(result);
		}
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		return 41;
	}

	public void ShatterLocal(Vector3 pos, Vector3 impulse)
	{
		if (!NetGame.isClient)
		{
			shattered = true;
			pos = collider.ClosestPointOnBounds(pos);
			uint num = (uint)Random.Range(0, 1023);
			netState = new NetState
			{
				shatterPos = QuantizePos(pos),
				shatterMagnitude = (uint)NetFloat.Quantize(impulse.magnitude, 10000f, 11),
				shatterSeed = num,
				netId = NetStream.GetDynamicScopeId()
			};
			pos = DequantizePos(netState.shatterPos);
			Shatter(pos, impulse, impulse.magnitude, num, netState.netId);
		}
	}

	private void ShatterNet(NetState netState)
	{
		shattered = true;
		Vector3 contactPoint = DequantizePos(netState.shatterPos);
		float impactMagnitude = NetFloat.Dequantize((int)netState.shatterMagnitude, 10000f, 11);
		uint shatterSeed = netState.shatterSeed;
		Shatter(contactPoint, Vector3.zero, impactMagnitude, shatterSeed, netState.netId);
	}

	private NetVector3 QuantizePos(Vector3 pos)
	{
		BoxCollider boxCollider = collider as BoxCollider;
		if (boxCollider != null)
		{
			return NetVector3.Quantize(base.transform.InverseTransformPoint(pos) - boxCollider.center, boxCollider.size / 2f, 10);
		}
		return NetVector3.Quantize(pos - collider.bounds.center, collider.bounds.extents, 10);
	}

	private Vector3 DequantizePos(NetVector3 vec)
	{
		BoxCollider boxCollider = collider as BoxCollider;
		if (boxCollider != null)
		{
			return base.transform.TransformPoint(vec.Dequantize(boxCollider.size / 2f) + boxCollider.center);
		}
		return vec.Dequantize(collider.bounds.extents) + collider.bounds.center;
	}
}
