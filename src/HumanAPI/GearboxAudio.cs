using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class GearboxAudio : MonoBehaviour, IReset, INetBehavior
	{
		public Gearbox gearbox;

		public Rigidbody lever;

		public float volume = 1f;

		public float gearVolume = 1f;

		private float crossfadeDuration = 0.1f;

		public float holdEngineOn = 1.5f;

		public Sound2 engine;

		public Sound2 gearShift;

		public Sound2 gearReverse;

		public Sound2 reverseBeep;

		public GearboxSampleType sampleType;

		private int currentGear;

		private bool leverGrabbed;

		private AudioClip startClip;

		private void Awake()
		{
		}

		public void GrabLever()
		{
			leverGrabbed = true;
			if (sampleType == GearboxSampleType.ReverseIdleFirstSecond && currentGear == 0)
			{
				engine.Switch('B');
				engine.Play();
			}
		}

		public void ReleaseLever()
		{
			leverGrabbed = false;
			if (sampleType == GearboxSampleType.ReverseIdleFirstSecond && currentGear == 0)
			{
				engine.Stop();
			}
		}

		public void SetGet(int gear)
		{
			currentGear = gear;
			if (sampleType == GearboxSampleType.ReverseIdleFirstSecond)
			{
				engine.Switch((char)(66 + currentGear));
			}
			else
			{
				engine.Switch((gear != 2) ? 'A' : 'B');
				if (gear != 0)
				{
					engine.Play();
				}
				else
				{
					engine.Stop();
				}
			}
			if (gear < 0)
			{
				if (gearReverse != null)
				{
					gearReverse.PlayOneShot();
				}
			}
			else if (gearShift != null)
			{
				gearShift.PlayOneShot();
			}
			if (!(reverseBeep != null))
			{
				return;
			}
			if (gear == -1)
			{
				if (!reverseBeep.isPlaying)
				{
					reverseBeep.Play(forceLoop: true);
				}
			}
			else if (reverseBeep.isPlaying)
			{
				reverseBeep.Stop();
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
		}

		private void Update()
		{
			if (ReplayRecorder.isPlaying || NetGame.isClient)
			{
				return;
			}
			if (gearbox.gear != currentGear)
			{
				SetGet(gearbox.gear);
			}
			if (GrabManager.IsGrabbedAny(lever.gameObject) != leverGrabbed)
			{
				if (!leverGrabbed)
				{
					GrabLever();
				}
				else
				{
					ReleaseLever();
				}
			}
		}

		public void StartNetwork(NetIdentity identity)
		{
		}

		public void CollectState(NetStream stream)
		{
			stream.Write(leverGrabbed);
			stream.Write((ushort)(currentGear + 1), 2);
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			state0.ReadBool();
			state0.ReadUInt32(2);
			ApplyState(state1);
		}

		public void ApplyState(NetStream state)
		{
			bool flag = state.ReadBool();
			int num = (int)(state.ReadUInt32(2) - 1);
			if (num != currentGear)
			{
				SetGet(num);
			}
			if (flag != leverGrabbed)
			{
				if (flag)
				{
					GrabLever();
				}
				else
				{
					ReleaseLever();
				}
			}
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			bool flag = state0?.ReadBool() ?? false;
			int num = (int)((state0 != null) ? (state0.ReadUInt32(2) - 1) : 0);
			bool flag2 = state1.ReadBool();
			int num2 = (int)(state1.ReadUInt32(2) - 1);
			if (num != num2 || flag != flag2)
			{
				delta.Write(v: true);
				delta.Write(flag2);
				delta.Write((ushort)(num2 + 1), 2);
			}
			else
			{
				delta.Write(v: false);
			}
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			bool v = state0?.ReadBool() ?? false;
			int num = (int)((state0 != null) ? (state0.ReadUInt32(2) - 1) : 0);
			if (delta.ReadBool())
			{
				v = delta.ReadBool();
				num = (int)(delta.ReadUInt32(2) - 1);
			}
			result.Write(v);
			result.Write((ushort)(num + 1), 2);
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			return 4;
		}

		public void SetMaster(bool isMaster)
		{
		}
	}
}
