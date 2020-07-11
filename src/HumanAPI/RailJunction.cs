using UnityEngine;

namespace HumanAPI
{
	public class RailJunction : Node
	{
		public bool showDebug;

		public NodeInput select;

		public RailEnd forkA;

		public RailEnd forkB;

		public RailSnap junctionLock;

		public GameObject trackA;

		public GameObject trackB;

		public override void Process()
		{
			base.Process();
			SyncFork();
		}

		private void SyncFork()
		{
			if (junctionLock != null && junctionLock.currentRail == forkA.rail)
			{
				forkA.connectedTo.connectedTo = forkA;
			}
			else if (junctionLock != null && junctionLock.currentRail == forkB.rail)
			{
				forkB.connectedTo.connectedTo = forkB;
			}
			else if (select.value < -0.5f)
			{
				forkA.connectedTo.connectedTo = forkA;
				if (trackA != null)
				{
					trackA.SetActive(value: false);
				}
				if (trackB != null)
				{
					trackB.SetActive(value: true);
				}
			}
			else if (select.value > 0.5f)
			{
				forkB.connectedTo.connectedTo = forkB;
				if (trackB != null)
				{
					trackB.SetActive(value: false);
				}
				if (trackA != null)
				{
					trackA.SetActive(value: true);
				}
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (junctionLock != null && junctionLock.transform.parent == other.transform.parent)
			{
				SyncFork();
			}
		}
	}
}
