using System.Collections.Generic;

namespace HumanAPI
{
	public class SteamSystem
	{
		private static List<SteamNode> nodes = new List<SteamNode>();

		private static List<SteamPort> boundary = new List<SteamPort>();

		public static void Recalculate(SteamNode start)
		{
			nodes.Clear();
			boundary.Clear();
			bool flag = false;
			float num = 0f;
			nodes.Add(start);
			for (int i = 0; i < nodes.Count; i++)
			{
				SteamNode steamNode = nodes[i];
				for (int j = 0; j < steamNode.ports.Count; j++)
				{
					SteamPort steamPort = steamNode.ports[j];
					if (steamPort.connectedPort == null)
					{
						boundary.Add(steamPort);
						flag |= steamPort.isOpen;
						num += steamPort.ownPressure;
					}
					else if (!nodes.Contains(steamPort.connectedPort.node))
					{
						nodes.Add(steamPort.connectedPort.node);
					}
				}
			}
			for (int k = 0; k < nodes.Count; k++)
			{
				SteamNode steamNode2 = nodes[k];
				for (int l = 0; l < steamNode2.ports.Count; l++)
				{
					steamNode2.ports[l].ApplySystemState(flag, num);
				}
			}
		}
	}
}
