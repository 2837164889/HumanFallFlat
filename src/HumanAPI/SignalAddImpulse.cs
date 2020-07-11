using UnityEngine;

namespace HumanAPI
{
	public class SignalAddImpulse : Node
	{
		public NodeInput input;

		public Rigidbody body;

		public Vector3 force;

		public Vector3 torque;

		public bool applyImpulseToChildren;

		private Rigidbody[] childrenRigidbodies;

		private bool appliedImpulse;

		protected override void OnEnable()
		{
			base.OnEnable();
			if (body == null && !applyImpulseToChildren)
			{
				body = GetComponent<Rigidbody>();
			}
			if (applyImpulseToChildren)
			{
				childrenRigidbodies = GetComponentsInChildren<Rigidbody>();
			}
		}

		public override void Process()
		{
			base.Process();
			if (!(input.value > 0.5f) || appliedImpulse)
			{
				return;
			}
			if (applyImpulseToChildren)
			{
				childrenRigidbodies = base.transform.GetComponentsInChildren<Rigidbody>();
				for (int i = 0; i < childrenRigidbodies.Length; i++)
				{
					childrenRigidbodies[i].AddForce(force, ForceMode.Impulse);
					childrenRigidbodies[i].AddTorque(torque, ForceMode.Impulse);
				}
				appliedImpulse = true;
			}
			else if (body != null)
			{
				body.AddForce(force, ForceMode.Impulse);
				body.AddTorque(torque, ForceMode.Impulse);
				appliedImpulse = true;
			}
		}
	}
}
