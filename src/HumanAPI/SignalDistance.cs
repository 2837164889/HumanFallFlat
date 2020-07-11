using UnityEngine;

namespace HumanAPI
{
	public class SignalDistance : Node
	{
		public NodeOutput distance;

		public Transform transform1;

		public Transform transform2;

		private void Update()
		{
			distance.SetValue((transform1.position - transform2.position).magnitude);
		}
	}
}
