using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Collision Audio Sensor", 10)]
	public class CollisionAudioSensor : MonoBehaviour
	{
		private static int idCounter;

		[Tooltip("Override for the counter")]
		public int id;

		private Rigidbody body;

		[Tooltip("Override value for the pitch")]
		public float pitch = 1f;

		[Tooltip("Override for the volume")]
		public float volume = 1f;

		[Tooltip("Use ths in order to print debug info to the Log")]
		public bool showDebug;

		private float nextSoundTime;

		private uint evtCollisionAudio;

		private NetIdentity identity;

		private void Awake()
		{
			id = ++idCounter;
		}

		protected virtual void OnEnable()
		{
		}

		private void OnCollisionEnter(Collision collision)
		{
			ReportCollision(collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			ReportCollision(collision);
		}

		private void ReportCollision(Collision collision)
		{
			if (NetGame.isClient || ReplayRecorder.isPlaying || collision.contacts.Length == 0)
			{
				return;
			}
			float time = Time.time;
			if (nextSoundTime > time || collision.relativeVelocity.magnitude < CollisionAudioEngine.instance.minVelocity || collision.impulse.magnitude < CollisionAudioEngine.instance.minImpulse)
			{
				return;
			}
			collision.Analyze(out Vector3 pos, out float impulse, out float normalVelocity, out float tangentVelocity, out PhysicMaterial mat, out PhysicMaterial mat2, out int id, out float volume, out float pitch);
			if (this.id < id)
			{
				SurfaceType surf = SurfaceTypes.Resolve(mat);
				SurfaceType surf2 = SurfaceTypes.Resolve(mat2);
				if (ReportCollision(surf, surf2, pos, impulse, normalVelocity, tangentVelocity, this.volume * volume, this.pitch * pitch))
				{
					nextSoundTime = time + Random.Range(0.5f, 1.5f) * CollisionAudioEngine.instance.hitDelay;
				}
			}
		}

		protected virtual bool ReportCollision(SurfaceType surf1, SurfaceType surf2, Vector3 pos, float impulse, float normalVelocity, float tangentVelocity, float volume, float pitch)
		{
			return CollisionAudioEngine.instance.ReportCollision(this, surf1, surf2, pos, impulse, normalVelocity, tangentVelocity, volume, pitch);
		}

		private void Start()
		{
			identity = GetComponentInParent<NetIdentity>();
			if (identity != null)
			{
				evtCollisionAudio = identity.RegisterEvent(OnReceiveCollisionAudio);
			}
		}

		public void BroadcastCollisionAudio(CollisionAudioHitConfig config, AudioChannel channel, Vector3 pos, float rms, float pitch)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Broadcast Collision Audio ");
			}
			float num = AudioUtils.ValueToDB(rms) + 32f;
			if (num < -64f)
			{
				return;
			}
			if (identity == null)
			{
				Debug.LogErrorFormat(this, "No NetIdentity for {0}", base.name);
				return;
			}
			NetStream netStream = identity.BeginEvent(evtCollisionAudio);
			netStream.Write(config.netId, 8);
			if (channel == AudioChannel.Footsteps)
			{
				netStream.Write(v: true);
			}
			else
			{
				netStream.Write(v: false);
				if (channel == AudioChannel.Body)
				{
					netStream.Write(v: true);
				}
				else
				{
					netStream.Write(v: false);
				}
			}
			Vector3 vec = pos - base.transform.position;
			NetVector3.Quantize(vec, 100f, 10).Write(netStream, 3);
			netStream.Write(NetFloat.Quantize(AudioUtils.ValueToDB(rms) + 32f, 64f, 6), 6);
			netStream.Write(NetFloat.Quantize(AudioUtils.RatioToCents(pitch), 4800f, 8), 3, 8);
			identity.EndEvent();
		}

		public void OnReceiveCollisionAudio(NetStream stream)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Recieve Collision Audio ");
			}
			ushort libId = (ushort)stream.ReadUInt32(8);
			AudioChannel channel = (!stream.ReadBool()) ? (stream.ReadBool() ? AudioChannel.Body : AudioChannel.Physics) : AudioChannel.Footsteps;
			Vector3 b = NetVector3.Read(stream, 3, 10).Dequantize(100f);
			Vector3 pos = base.transform.position + b;
			float emit = AudioUtils.DBToValue(NetFloat.Dequantize(stream.ReadInt32(6), 64f, 6) - 32f);
			float num = AudioUtils.CentsToRatio(NetFloat.Dequantize(stream.ReadInt32(3, 8), 4800f, 8));
			CollisionAudioHitConfig config = CollisionAudioEngine.instance.GetConfig(libId);
			if (config != null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " There is no audio engine ");
				}
				config.PlayWithKnownEmit(channel, null, pos, emit, num);
			}
		}
	}
}
