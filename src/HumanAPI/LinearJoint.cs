using UnityEngine;

namespace HumanAPI
{
	public class LinearJoint : JointImplementation
	{
		[Tooltip("Used to get the transform direction")]
		public Transform axis;

		private Vector3 connectedAxis;

		private Vector3 connectedCenter;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		public override void CreateMainJoint()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Creating configurable joint ");
			}
			if (axis == null)
			{
				axis = base.transform;
			}
			connectedAxis = axis.forward;
			connectedCenter = body.position;
			if (anchorTransform != null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " No Transform for the axis ");
				}
				connectedAxis = anchorTransform.InverseTransformDirection(connectedAxis);
				connectedCenter = anchorTransform.InverseTransformPoint(connectedCenter);
			}
			Vector3 zero = Vector3.zero;
			Vector3 vector = body.InverseTransformDirection(axis.forward);
			Vector3 secondaryAxis = body.InverseTransformDirection(axis.right);
			centerValue = 0f;
			if (base.isKinematic)
			{
				return;
			}
			if (showDebug)
			{
				Debug.Log(base.name + " Kinematic Not Set ");
			}
			if (useLimits)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Limits Set ");
				}
				centerValue = (maxValue + minValue) / 2f;
			}
			joint = body.gameObject.AddComponent<ConfigurableJoint>();
			joint.autoConfigureConnectedAnchor = false;
			joint.axis = vector;
			joint.secondaryAxis = secondaryAxis;
			joint.anchor = zero;
			joint.connectedAnchor = connectedCenter + connectedAxis * centerValue;
			joint.connectedBody = anchorRigid;
			joint.enableCollision = enableCollision;
			joint.xMotion = (useLimits ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free);
			joint.yMotion = ConfigurableJointMotion.Locked;
			joint.zMotion = ConfigurableJointMotion.Locked;
			joint.angularXMotion = ConfigurableJointMotion.Locked;
			joint.angularYMotion = ConfigurableJointMotion.Locked;
			joint.angularZMotion = ConfigurableJointMotion.Locked;
			if (useLimits)
			{
				joint.linearLimit = new SoftJointLimit
				{
					limit = (maxValue - minValue) / 2f
				};
			}
		}

		private void FixedUpdate()
		{
			EnsureInitialized();
			if (!(joint == null) && useSpring)
			{
				float num = (!useTension) ? spring : (maxForce / tensionDist);
				float num2 = (!useTension) ? damper : (maxForce / maxSpeed);
				Vector3 vector = -new Vector3(target - centerValue, 0f, 0f);
				JointDrive xDrive = joint.xDrive;
				if (joint.targetPosition != vector || xDrive.maximumForce != maxForce || xDrive.positionSpring != num || xDrive.positionDamper != num2)
				{
					joint.xDrive = new JointDrive
					{
						maximumForce = maxForce,
						positionSpring = num,
						positionDamper = num2
					};
					joint.targetPosition = vector;
					rigid.WakeUp();
				}
			}
		}

		public override float GetValue()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Getting value ");
			}
			EnsureInitialized();
			if (anchorTransform != null)
			{
				return Vector3.Dot(anchorTransform.InverseTransformPoint(body.position) - connectedCenter, connectedAxis);
			}
			return Vector3.Dot(body.position - connectedCenter, connectedAxis);
		}

		public override void SetValue(float pos)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Setting value ");
			}
			EnsureInitialized();
			Vector3 position = connectedCenter + connectedAxis * pos;
			if (anchorTransform != null)
			{
				position = anchorTransform.TransformPoint(position);
			}
			if (rigid != null)
			{
				rigid.MovePosition(position);
				if (!rigid.isKinematic)
				{
					body.position = position;
				}
			}
			else
			{
				body.position = position;
			}
		}
	}
}
