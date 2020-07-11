using System;

namespace HumanAPI
{
	[Serializable]
	public class NodeExit : NodeInput
	{
		public override bool CanConnect(NodeOutput output)
		{
			return output is NodeEntry;
		}

		public void Execute()
		{
			NodeEntry nodeEntry = GetConnectedOutput() as NodeEntry;
			nodeEntry?.node.Execute(nodeEntry);
		}

		public override void OnDisable()
		{
		}

		public override void OnEnable()
		{
		}
	}
}
