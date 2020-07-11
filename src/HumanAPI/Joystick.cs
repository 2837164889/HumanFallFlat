using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Joystick", 10)]
	public class Joystick : Node
	{
		public NodeOutput horizontal;

		public NodeOutput vertical;

		public ConfigurableJoint joint;

		[Space]
		public Vector3 verticalAxis = Vector3.right;

		public Vector3 horizontalAxis = Vector3.forward;

		private Quaternion initialLocalRotation;

		private float angle;

		private Vector3 axis;

		protected virtual void Awake()
		{
			initialLocalRotation = joint.ReadInitialRotation();
			joint.axis = Vector3.right;
			Vector3 vector = verticalAxis + horizontalAxis;
			if (vector.x != 0f)
			{
				joint.angularXMotion = ConfigurableJointMotion.Limited;
			}
			if (vector.y != 0f)
			{
				joint.angularYMotion = ConfigurableJointMotion.Limited;
			}
			if (vector.z != 0f)
			{
				joint.angularZMotion = ConfigurableJointMotion.Limited;
			}
		}

		private void FixedUpdate()
		{
			(Quaternion.Inverse(joint.transform.localRotation) * initialLocalRotation).ToAngleAxis(out angle, out axis);
			if (angle < 25f)
			{
				SafeSetValue(horizontal, 0f);
				SafeSetValue(vertical, 0f);
			}
			else
			{
				AngleToValue(axis, verticalAxis, vertical);
				AngleToValue(axis, horizontalAxis, horizontal);
			}
		}

		private void AngleToValue(Vector3 currentValue, Vector3 axis, NodeOutput output)
		{
			axis.Scale(currentValue);
			float f = axis.x + axis.y + axis.z;
			SafeSetValue(output, (!(Mathf.Abs(f) > 0.5f)) ? 0f : Mathf.Sign(f));
		}

		private void SafeSetValue(NodeOutput output, float value)
		{
			output?.SetValue(value);
		}
	}
}
