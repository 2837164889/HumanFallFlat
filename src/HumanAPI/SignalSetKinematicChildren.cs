using UnityEngine;

namespace HumanAPI
{
	public class SignalSetKinematicChildren : Node
	{
		public NodeInput input;

		private Rigidbody[] rigidbodies;

		protected override void OnEnable()
		{
			base.OnEnable();
			rigidbodies = GetComponentsInChildren<Rigidbody>();
		}

		public override void Process()
		{
			base.Process();
			if (rigidbodies != null)
			{
				Rigidbody[] array = rigidbodies;
				foreach (Rigidbody rigidbody in array)
				{
					rigidbody.isKinematic = (input.value > 0.5f);
				}
			}
		}
	}
}
