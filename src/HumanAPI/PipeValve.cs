using UnityEngine;

namespace HumanAPI
{
	public class PipeValve : Node
	{
		private class PipeValvePort : SteamPort
		{
			public PipeValvePort otherSide;

			public override SteamPort connectedPort => otherSide;

			public override bool isOpen => false;

			public override float ownPressure => 0f;

			public override void ApplySystemState(bool isOpenSystem, float pressure)
			{
			}
		}

		public SteamNode node1;

		public SteamNode node2;

		private PipeValvePort port1;

		private PipeValvePort port2;

		public NodeInput close;

		private void Awake()
		{
			port1 = node1.gameObject.AddComponent<PipeValvePort>();
			port1.node = node1;
			port2 = node2.gameObject.AddComponent<PipeValvePort>();
			port2.node = node2;
			if (!node1.ports.Contains(port1))
			{
				node1.ports.Add(port1);
				SteamSystem.Recalculate(node1);
			}
			if (!node2.ports.Contains(port2))
			{
				node2.ports.Add(port2);
				SteamSystem.Recalculate(node2);
			}
		}

		public override void Process()
		{
			base.Process();
			if (Mathf.Abs(close.value) >= 0.5f)
			{
				if (port1.connectedPort == null)
				{
					port1.otherSide = port2;
					port2.otherSide = port1;
					SteamSystem.Recalculate(node1);
				}
			}
			else if (port1.connectedPort != null)
			{
				port1.otherSide = null;
				port2.otherSide = null;
				SteamSystem.Recalculate(node1);
				SteamSystem.Recalculate(node2);
			}
		}
	}
}
