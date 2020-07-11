using UnityEngine;

namespace HumanAPI
{
	public class SignalVelocity : Node
	{
		[Tooltip("The value output by this node in line with the bodies velocity")]
		public NodeOutput value;

		[Tooltip("Velocity value when coming to a dead stop")]
		public float toDeadVelocity;

		[Tooltip("Velocity value when coming to top speed")]
		public float toVelocity;

		[Tooltip("Body to which the relative velocity will be calculated , if null then world")]
		public Rigidbody relativeBody;

		[Tooltip("Body which the velocity should come from , if null then self")]
		public Rigidbody body;

		[Tooltip("Velocity Var")]
		[ReadOnly]
		public float velocity;

		private Vector3 directionalVelocity;

		[Tooltip("Whether or not to report direction as part of the velocity")]
		public bool directional;

		public bool xDirection;

		public bool yDirection;

		public bool zDirection;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		protected override void OnEnable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Enable ");
			}
			if (body == null)
			{
				body = GetComponent<Rigidbody>();
			}
			base.OnEnable();
		}

		private void FixedUpdate()
		{
			float num = 0f;
			if (relativeBody != null)
			{
				velocity = (body.velocity - relativeBody.velocity).magnitude;
			}
			else
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Relative Body = null ");
				}
				velocity = body.velocity.magnitude;
				directionalVelocity = body.velocity;
			}
			if (velocity > toDeadVelocity)
			{
				num = Mathf.InverseLerp(toDeadVelocity, toVelocity, velocity);
				if (showDebug)
				{
					Debug.Log(base.name + " velocity > toDeadVelocity ");
					Debug.Log(base.name + " value = " + num);
					Debug.Log(base.name + " directionalVelocity = " + directionalVelocity);
				}
			}
			if (xDirection && directional)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Use Velocity X Vector Value ");
				}
				num = directionalVelocity.x;
			}
			if (yDirection && directional)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Use Velocity Y Vector Value ");
				}
				num = directionalVelocity.y;
			}
			if (zDirection && directional)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Use Velocity Z Vector Value ");
				}
				num = directionalVelocity.z;
			}
			value.SetValue(num);
		}
	}
}
