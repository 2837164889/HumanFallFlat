using UnityEngine;

namespace HumanAPI
{
	public class AutomaticDoor : LerpBase
	{
		public ConfigurableJoint slideJoint;

		public float doorForce = 1000f;

		public float doorSpeed = 1f;

		[Range(0f, 0.99f)]
		public float tension = 0.9f;

		protected override void ApplyValue(float value)
		{
			float num = slideJoint.linearLimit.limit * 2f;
			float num2 = num * tension / (1f - tension) + num / 2f;
			slideJoint.targetPosition = new Vector3(Mathf.Lerp(num2, 0f - num2, value), 0f, 0f);
			JointDrive xDrive = slideJoint.xDrive;
			xDrive.positionSpring = doorForce / (num2 + num / 2f);
			xDrive.positionDamper = doorForce / doorSpeed;
			slideJoint.xDrive = xDrive;
			slideJoint.GetComponent<Rigidbody>().WakeUp();
		}
	}
}
