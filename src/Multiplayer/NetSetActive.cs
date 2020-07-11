using UnityEngine;

namespace Multiplayer
{
	public class NetSetActive : MonoBehaviour, INetBehavior
	{
		private bool initialActive;

		public void StartNetwork(NetIdentity identity)
		{
			initialActive = base.gameObject.activeSelf;
		}

		public void SetMaster(bool isMaster)
		{
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			SetActive(initialActive);
		}

		public void CollectState(NetStream stream)
		{
			NetBoolEncoder.CollectState(stream, base.gameObject.activeSelf);
		}

		public void SetActive(bool show)
		{
			if (show != base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(show);
			}
		}

		public void ApplyState(NetStream state)
		{
			SetActive(NetBoolEncoder.ApplyState(state));
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			SetActive(NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
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
	}
}
