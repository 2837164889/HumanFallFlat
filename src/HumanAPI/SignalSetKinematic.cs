using UnityEngine;

namespace HumanAPI
{
	public class SignalSetKinematic : Node
	{
		public NodeInput input;

		public Rigidbody body;

		protected override void OnEnable()
		{
			base.OnEnable();
			if (body == null)
			{
				body = GetComponent<Rigidbody>();
			}
		}

		public override void Process()
		{
			base.Process();
			if (body != null)
			{
				body.isKinematic = (input.value > 0.5f);
			}
		}
	}
}
