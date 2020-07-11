using UnityEngine;

namespace HumanAPI
{
	public class RotateTowardsTargetRotation : Node
	{
		public NodeInput active;

		private Quaternion targetRot;

		private Rigidbody rb;

		public float rotSpeed = 1f;

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			targetRot = rb.rotation;
		}

		public override void Process()
		{
			base.Process();
			if (active.value == 1f)
			{
				targetRot = rb.rotation;
			}
		}

		private void FixedUpdate()
		{
			if (active.value == 1f)
			{
				rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRot, rotSpeed * Time.fixedDeltaTime));
			}
		}
	}
}
