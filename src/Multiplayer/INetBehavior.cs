namespace Multiplayer
{
	public interface INetBehavior
	{
		void StartNetwork(NetIdentity identity);

		void CollectState(NetStream stream);

		void ApplyLerpedState(NetStream state0, NetStream state1, float mix);

		void ApplyState(NetStream state);

		void CalculateDelta(NetStream state0, NetStream state1, NetStream delta);

		void AddDelta(NetStream state0, NetStream delta, NetStream result);

		int CalculateMaxDeltaSizeInBits();

		void SetMaster(bool isMaster);

		void ResetState(int checkpoint, int subObjectives);
	}
}
