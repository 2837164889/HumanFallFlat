using HumanAPI;
using UnityEngine;

namespace Multiplayer
{
	[AddNodeMenuItem]
	[RequireComponent(typeof(NetIdentity))]
	public class NetSignal : Node, INetBehavior
	{
		public NodeInput input;

		public NodeOutput output;

		public static NetFloat encoder = new NetFloat(2f, 9, 3, 3);

		public override void Process()
		{
			base.Process();
			if (!ReplayRecorder.isPlaying && !NetGame.isClient)
			{
				output.SetValue(input.value);
			}
		}

		public void StartNetwork(NetIdentity identity)
		{
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
		}

		public void CollectState(NetStream stream)
		{
			encoder.CollectState(stream, input.value);
		}

		public void ApplyState(NetStream state)
		{
			output.SetValue(encoder.ApplyState(state));
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			output.SetValue(encoder.ApplyLerpedState(state0, state1, mix));
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

		public void SetMaster(bool isMaster)
		{
		}
	}
}
