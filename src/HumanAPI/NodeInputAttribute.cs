using System;

namespace HumanAPI
{
	public class NodeInputAttribute : Attribute
	{
		public Type type;

		public NodeInputAttribute(Type type)
		{
			this.type = type;
		}
	}
}
