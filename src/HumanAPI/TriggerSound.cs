using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class TriggerSound : MonoBehaviour, IReset
	{
		[Tooltip("Whether or not the trigger volume reacts to the player or not")]
		public bool acceptPlayer;

		[Tooltip("An array of colliders the trigger volume reacts to")]
		public Collider[] acceptColliders;

		[Tooltip("Min velocity the thing being looked for needs to be moving at to trigger this sound")]
		public float minVelocity;

		[Tooltip("Max velocity the thing being looked for needs to be moving at to trigger this sound")]
		public float maxVelocity;

		[Tooltip("Min angular velocity the thing being looked for needs to be doing to trigger this sound")]
		public float minAngular;

		[Tooltip("Max angular velocity the thing being looked for needs to be doing to trigger this sound")]
		public float maxAngular;

		[Tooltip("Min Volume the sound can be played at")]
		public float minVolume;

		[Tooltip("Min Delay before the sound plays")]
		public float minDelay = 0.2f;

		[Tooltip("Sound to play")]
		public Sound2 sound;

		public AudioSource unitySound;

		private float lastSoundTime;

		private uint evtImpact;

		private NetIdentity identity;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		private void Start()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Started ");
			}
			identity = GetComponentInParent<NetIdentity>();
			if (identity != null)
			{
				evtImpact = identity.RegisterEvent(OnImpact);
			}
		}

		private void BroadcastImpact(float impact)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Broadcast impact ");
			}
			float num = AudioUtils.ValueToDB(impact) + 32f;
			if (!(num < -64f))
			{
				NetStream netStream = identity.BeginEvent(evtImpact);
				netStream.Write(NetFloat.Quantize(num, 64f, 6), 6);
				identity.EndEvent();
			}
		}

		private void OnImpact(NetStream stream)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Impact ");
			}
			float impact = AudioUtils.DBToValue(NetFloat.Dequantize(stream.ReadInt32(6), 64f, 6) - 32f);
			PlayImpact(impact);
		}

		private void Awake()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Awake ");
			}
			if (sound == null)
			{
				sound = GetComponentInChildren<Sound2>();
			}
			else if (unitySound == null)
			{
				unitySound = GetComponentInChildren<AudioSource>();
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Trigger Enter ");
			}
			float num = 1f;
			if (acceptPlayer && other.tag == "Player")
			{
				if (maxVelocity > 0f)
				{
					float magnitude = other.GetComponentInParent<Human>().velocity.magnitude;
					if (magnitude < minVelocity)
					{
						return;
					}
					num = Mathf.InverseLerp(minVelocity, maxVelocity, magnitude);
				}
			}
			else
			{
				if (acceptColliders.Length <= 0 || !(other.tag != "Player"))
				{
					return;
				}
				bool flag = false;
				for (int i = 0; i < acceptColliders.Length; i++)
				{
					if (acceptColliders[i] == other)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return;
				}
				if (minVelocity > 0f || maxVelocity > 0f || minAngular > 0f || maxAngular > 0f)
				{
					Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
					if (componentInParent == null)
					{
						return;
					}
					num = 0f;
					if (minVelocity > 0f || maxVelocity > 0f)
					{
						float magnitude2 = componentInParent.velocity.magnitude;
						if (maxVelocity != 0f)
						{
							num = Mathf.InverseLerp(minVelocity, maxVelocity, magnitude2);
						}
						else if (magnitude2 > minVelocity)
						{
							num = 1f;
						}
					}
					if (minAngular > 0f || maxAngular > 0f)
					{
						float magnitude3 = componentInParent.angularVelocity.magnitude;
						if (maxAngular != 0f)
						{
							num = Mathf.Max(num, Mathf.InverseLerp(minAngular, maxAngular, magnitude3));
						}
						else if (magnitude3 > minAngular)
						{
							num = 1f;
						}
					}
					if (num == 0f)
					{
						return;
					}
				}
			}
			if (!(num < minVolume))
			{
				if (NetGame.isServer || ReplayRecorder.isRecording)
				{
					BroadcastImpact(num);
				}
				PlayImpact(num);
			}
		}

		private void PlayImpact(float impact)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Player Impact ");
			}
			float time = Time.time;
			if (!(lastSoundTime + minDelay > time))
			{
				if (sound != null)
				{
					sound.PlayOneShot(impact);
				}
				else if (unitySound != null)
				{
					unitySound.volume = impact;
					unitySound.Play();
				}
				lastSoundTime = time;
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset State ");
			}
			lastSoundTime = 0f;
		}
	}
}
