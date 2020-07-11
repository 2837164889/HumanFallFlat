using System;

namespace HumanAPI
{
	public abstract class NodeSocket
	{
		[NonSerialized]
		public Node node;

		[NonSerialized]
		public string name;

		public abstract void OnEnable();

		public abstract void OnDisable();
	}
}
