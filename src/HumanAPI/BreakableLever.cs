using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class BreakableLever : Lever, INetBehavior
	{
		[Tooltip("Reference to the sound2 to make when breaking")]
		public Sound2 breakSound;

		[Tooltip("Reference to the audiosource to make when breaking")]
		public AudioSource audioSourceBreakSound;

		[Tooltip("Whether or not to break on Positive movement")]
		public bool breakPositive;

		[Tooltip("Whether or not to break on Negative Movement")]
		public bool breakNegative;

		protected override void Awake()
		{
			base.Awake();
		}

		private void Break()
		{
			if (angularJoint.jointCreated)
			{
				angularJoint.DestroyMainJoint();
				angularJoint.body.gameObject.GetComponent<CGShift>().ResetCG();
				if (breakSound != null)
				{
					breakSound.PlayOneShot();
				}
				if (audioSourceBreakSound != null)
				{
					audioSourceBreakSound.Play();
				}
			}
		}

		protected override void CheckSnap(float value, bool forcePosition)
		{
			if (breakNegative && value < -0.75f)
			{
				Break();
				output.SetValue(-1f);
			}
			else if (breakPositive && value > 0.75f)
			{
				Break();
				output.SetValue(1f);
			}
			else
			{
				base.CheckSnap(value, forcePosition);
			}
		}

		public void StartNetwork(NetIdentity identity)
		{
			angularJoint.syncNetBody = false;
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
		}

		public void CollectState(NetStream stream)
		{
			NetBoolEncoder.CollectState(stream, angularJoint.jointCreated);
		}

		public void ApplyState(NetStream state)
		{
			bool jointCreated = NetBoolEncoder.ApplyState(state);
			ApplyState(jointCreated);
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			bool jointCreated = NetBoolEncoder.ApplyLerpedState(state0, state1, mix);
			ApplyState(jointCreated);
		}

		private void ApplyState(bool jointCreated)
		{
			if (jointCreated != angularJoint.jointCreated)
			{
				if (jointCreated)
				{
					angularJoint.ResetState(0, 0);
				}
				else
				{
					Break();
				}
			}
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			NetBoolEncoder.CalculateDelta(state0, state1, delta);
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			NetBoolEncoder.AddDelta(state0, delta, result);
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			return NetBoolEncoder.CalculateMaxDeltaSizeInBits();
		}

		public void SetMaster(bool isMaster)
		{
		}
	}
}
