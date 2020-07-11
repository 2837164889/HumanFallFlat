using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class RailSnap : Node
	{
		private ConfigurableJoint joint;

		[Tooltip("The input this node takes from the graph")]
		public NodeInput input;

		[Tooltip("The max Speed of this rail car")]
		public float maxSpeed = 3f;

		[Tooltip("A referece to a rail snap parameter script for storing values")]
		public RailSnapParameters parameters;

		[Tooltip("The current rail the train car should be on ")]
		public Rail currentRail;

		[Tooltip("The current segment of the rail the rail car should be on ")]
		public int currentSegment;

		[Tooltip("The current speed the rail car is doing")]
		[ReadOnly]
		public float currentSpeed;

		private bool seendebugstring1;

		private Rigidbody body;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		private Vector3 current;

		private void Awake()
		{
			body = GetComponentInParent<Rigidbody>();
			if (parameters == null)
			{
				parameters = base.gameObject.AddComponent<RailSnapParameters>();
				parameters.maxSpeed = maxSpeed;
				parameters.accelerationTime = 0f;
				parameters.decelerationTime = 0f;
				parameters.posSpringX = 100000f;
				parameters.posDampX = 10000f;
				parameters.posSpringY = 100000f;
				parameters.posDampY = 10000f;
			}
			CreateJoint();
		}

		private void CreateJoint()
		{
			joint = body.gameObject.AddComponent<ConfigurableJoint>();
			joint.axis = Vector3.up;
			joint.secondaryAxis = Vector3.right;
			joint.xMotion = ConfigurableJointMotion.Free;
			joint.yMotion = ConfigurableJointMotion.Free;
			joint.zMotion = ConfigurableJointMotion.Free;
			ConfigurableJoint configurableJoint = joint;
			ConfigurableJointMotion configurableJointMotion = ConfigurableJointMotion.Free;
			joint.angularZMotion = configurableJointMotion;
			configurableJointMotion = configurableJointMotion;
			joint.angularYMotion = configurableJointMotion;
			configurableJoint.angularXMotion = configurableJointMotion;
			joint.autoConfigureConnectedAnchor = false;
			joint.anchor = body.transform.InverseTransformPoint(base.transform.position);
			joint.xDrive = new JointDrive
			{
				positionSpring = parameters.posSpringX,
				positionDamper = parameters.posDampX,
				maximumForce = float.PositiveInfinity
			};
			joint.yDrive = new JointDrive
			{
				positionSpring = parameters.posSpringY,
				positionDamper = parameters.posDampY,
				maximumForce = float.PositiveInfinity
			};
			current = base.transform.position;
		}

		private void FixedUpdate()
		{
			if (NetGame.isClient || ReplayRecorder.isPlaying)
			{
				if (joint != null)
				{
					Object.Destroy(joint);
				}
				return;
			}
			if (joint == null)
			{
				CreateJoint();
			}
			float num = parameters.maxSpeed * input.value;
			if (currentSpeed < num)
			{
				if (parameters.accelerationTime > 0f)
				{
					currentSpeed += parameters.maxSpeed * (Time.fixedDeltaTime / parameters.accelerationTime);
					currentSpeed = Mathf.Min(currentSpeed, parameters.maxSpeed);
				}
				else
				{
					currentSpeed = num;
				}
			}
			if (currentSpeed > num)
			{
				if (parameters.decelerationTime > 0f)
				{
					currentSpeed -= parameters.maxSpeed * (Time.fixedDeltaTime / parameters.decelerationTime);
					currentSpeed = Mathf.Max(currentSpeed, 0f - parameters.maxSpeed);
				}
				else
				{
					currentSpeed = num;
				}
			}
			if (num == 0f && Mathf.Abs(currentSpeed) < 0.05f)
			{
				currentSpeed = 0f;
			}
			Vector3 a = base.transform.forward * currentSpeed;
			Vector3 projected = Vector3.zero;
			Vector3 position = base.transform.position;
			current = Vector3.Lerp(current, position, (position - current).magnitude);
			if (Rail.Project(current + a * Time.fixedDeltaTime, ref projected, ref currentRail, ref currentSegment))
			{
				current = projected;
				joint.connectedAnchor = current;
			}
			else if (!seendebugstring1 && showDebug)
			{
				Debug.Log(base.name + "No track?");
				seendebugstring1 = true;
			}
		}
	}
}
