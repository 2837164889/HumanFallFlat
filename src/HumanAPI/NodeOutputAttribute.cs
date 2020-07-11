using System;

namespace HumanAPI
{
	public class NodeOutputAttribute : Attribute
	{
		public Type type;

		public NodeOutputAttribute(Type type)
		{
			this.type = type;
		}
	}
}
