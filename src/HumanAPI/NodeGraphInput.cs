using System;
using UnityEngine;

namespace HumanAPI
{
	[Serializable]
	public class NodeGraphInput
	{
		public string name;

		[HideInInspector]
		public NodeInput input = new NodeInput();

		[HideInInspector]
		public NodeOutput inputSocket = new NodeOutput();
	}
}
