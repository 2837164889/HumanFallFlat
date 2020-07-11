using System;
using UnityEngine;

namespace HumanAPI
{
	[Serializable]
	public class NodeGraphOutput
	{
		public string name;

		[HideInInspector]
		public NodeInput outputSocket = new NodeInput();

		[HideInInspector]
		public NodeOutput output = new NodeOutput();
	}
}
