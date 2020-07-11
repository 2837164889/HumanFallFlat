using UnityEngine;
using UnityEngine.Serialization;

namespace HumanAPI
{
	public abstract class JointBase : MonoBehaviour
	{
		[Tooltip("Value that matches model state")]
		public float initialValue;

		public float target;

		[Tooltip("Should the limits be set")]
		public bool useLimits = true;

		[Tooltip("Minimum range of servo action")]
		public float minValue = -1f;

		[Tooltip("Maximum range of servo action")]
		public float maxValue = 1f;

		[Tooltip("Maximum speed to move servo")]
		public float maxSpeed = 1f;

		[Tooltip("Maximum acceleration to move servo")]
		public float maxAcceleration = 10f;

		public bool useSpring = true;

		[Tooltip("The stiffness of spring driving the servo, smaller values - more stiff")]
		[FormerlySerializedAs("tensionAngle")]
		[FormerlySerializedAs("tensionRange")]
		public float tensionDist = 0.1f;

		[Tooltip("Maximum force of the servo motor")]
		public float maxForce;

		public float spring;

		public float damper;

		protected bool useTension;

		protected bool _isKinematic;

		public bool isKinematic => _isKinematic;

		public abstract void EnsureInitialized();

		public abstract float GetValue();

		public abstract float GetTarget();

		public abstract void SetValue(float value);

		public abstract void SetTarget(float value);
	}
}
