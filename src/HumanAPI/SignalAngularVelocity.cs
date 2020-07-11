using UnityEngine;

namespace HumanAPI
{
	public class SignalAngularVelocity : Node
	{
		public NodeOutput value;

		public float fromVelocity;

		public float fromDeadVelocity;

		public float toDeadVelocity;

		public float toVelocity;

		public Vector3 axis;

		private Vector3 parentAxis;

		public Transform relativeTo;

		private Rigidbody relativeBody;

		public Rigidbody body;

		[ReadOnly]
		public float velocity;

		private Quaternion oldRotation;

		protected override void OnEnable()
		{
			if (body == null)
			{
				body = GetComponent<Rigidbody>();
			}
			parentAxis = base.transform.TransformDirection(axis);
			oldRotation = body.rotation;
			if (relativeTo != null)
			{
				oldRotation = Quaternion.Inverse(relativeTo.rotation) * oldRotation;
				parentAxis = relativeTo.InverseTransformDirection(parentAxis);
				relativeBody = relativeTo.GetComponent<Rigidbody>();
			}
			base.OnEnable();
		}

		private void FixedUpdate()
		{
			Quaternion quaternion = body.rotation;
			float num = 0f;
			if (relativeTo != null)
			{
				quaternion = Quaternion.Inverse(relativeTo.rotation) * quaternion;
				if (relativeBody != null)
				{
					velocity = Vector3.Dot(relativeTo.InverseTransformVector(body.angularVelocity) - relativeBody.angularVelocity, parentAxis) * 57.29578f;
				}
				else
				{
					velocity = Vector3.Dot(relativeTo.InverseTransformVector(body.angularVelocity), parentAxis) * 57.29578f;
				}
			}
			else
			{
				velocity = Vector3.Dot(body.angularVelocity, parentAxis) * 57.29578f;
			}
			velocity = Quaternion.Angle(quaternion, oldRotation) / Time.fixedDeltaTime;
			oldRotation = quaternion;
			if (velocity < fromDeadVelocity)
			{
				num = 0f - Mathf.InverseLerp(fromDeadVelocity, fromVelocity, velocity);
			}
			if (velocity > toDeadVelocity)
			{
				num = Mathf.InverseLerp(toDeadVelocity, toVelocity, velocity);
			}
			value.SetValue(num);
		}
	}
}
