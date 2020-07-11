using Multiplayer;
using System;
using UnityEngine;

namespace HumanAPI
{
	public class AngularJoint : JointImplementation
	{
		public Transform axis;

		public bool disableOnClient;

		private bool useLimitJoint;

		private Vector3 connectedAxis;

		private Vector3 connectedCenter;

		private Vector3 connectedForward;

		private Vector3 localForward;

		private float lastKnownAngle;

		protected Joint limitJoint;

		private bool limitMin;

		protected float limitUpdateValue;

		public override void CreateMainJoint()
		{
			EnsureInitialized();
			if (axis == null)
			{
				axis = base.transform;
			}
			if (!base.isKinematic)
			{
				rigid.maxAngularVelocity = 100f;
			}
			connectedAxis = axis.right;
			connectedForward = axis.forward;
			connectedCenter = axis.position;
			if (anchorTransform != null)
			{
				connectedAxis = anchorTransform.InverseTransformDirection(connectedAxis);
				connectedForward = anchorTransform.InverseTransformDirection(connectedForward);
				connectedCenter = anchorTransform.InverseTransformPoint(connectedCenter);
			}
			Vector3 anchor = body.InverseTransformPoint(axis.position);
			Vector3 vector = body.InverseTransformDirection(axis.up);
			localForward = body.InverseTransformDirection(axis.forward);
			Vector3 vector2 = body.InverseTransformDirection(axis.right);
			centerValue = 0f;
			if (!base.isKinematic)
			{
				useLimitJoint = (useLimits && maxValue - minValue > 270f);
				if (useLimits && !useLimitJoint)
				{
					centerValue = (maxValue + minValue) / 2f;
					SetValue(centerValue);
				}
				joint = body.gameObject.AddComponent<ConfigurableJoint>();
				joint.autoConfigureConnectedAnchor = false;
				joint.axis = vector2;
				joint.secondaryAxis = localForward;
				joint.anchor = anchor;
				joint.connectedBody = anchorRigid;
				joint.connectedAnchor = connectedCenter;
				joint.enableCollision = enableCollision;
				joint.xMotion = ConfigurableJointMotion.Locked;
				joint.yMotion = ConfigurableJointMotion.Locked;
				joint.zMotion = ConfigurableJointMotion.Locked;
				joint.angularXMotion = ((useLimits && !useLimitJoint) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free);
				joint.angularYMotion = ConfigurableJointMotion.Locked;
				joint.angularZMotion = ConfigurableJointMotion.Locked;
				if (useLimits && !useLimitJoint)
				{
					joint.lowAngularXLimit = new SoftJointLimit
					{
						limit = minValue - centerValue
					};
					joint.highAngularXLimit = new SoftJointLimit
					{
						limit = maxValue - centerValue
					};
					SetValue(0f);
				}
			}
		}

		private void FixedUpdate()
		{
			if (disableOnClient && NetGame.isClient)
			{
				AngularJoint component = GetComponent<AngularJoint>();
				if (component != null)
				{
					component.enabled = false;
				}
				JointSensor component2 = GetComponent<JointSensor>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
			}
			if (joint == null)
			{
				return;
			}
			UpdateLimitJoint();
			if (useSpring)
			{
				Quaternion quaternion = Quaternion.Euler(0f - target + centerValue, 0f, 0f);
				float num = (!useTension) ? spring : (maxForce / (tensionDist * ((float)Math.PI / 180f)));
				float num2 = (!useTension) ? damper : (maxForce / (maxSpeed * ((float)Math.PI / 180f)));
				JointDrive angularXDrive = joint.angularXDrive;
				if (joint.targetRotation != quaternion || angularXDrive.maximumForce != maxForce || angularXDrive.positionSpring != num || angularXDrive.positionDamper != num2)
				{
					joint.angularXDrive = new JointDrive
					{
						maximumForce = maxForce,
						positionSpring = num,
						positionDamper = num2
					};
					joint.targetRotation = quaternion;
					rigid.WakeUp();
				}
			}
			else
			{
				JointDrive angularXDrive2 = joint.angularXDrive;
				if (angularXDrive2.positionSpring != spring || angularXDrive2.positionDamper != damper)
				{
					joint.angularXDrive = new JointDrive
					{
						maximumForce = maxForce,
						positionSpring = spring,
						positionDamper = damper
					};
				}
			}
		}

		protected virtual void UpdateLimitJoint()
		{
			if (!useLimitJoint)
			{
				return;
			}
			float value = GetValue();
			if (value != limitUpdateValue)
			{
				if (value < minValue + 60f)
				{
					if (limitJoint != null && !limitMin)
					{
						UnityEngine.Object.Destroy(limitJoint);
					}
					if (limitJoint == null)
					{
						limitJoint = CreateLimitJoint(minValue - value, Mathf.Min(minValue - value + 120f, maxValue - value));
						limitMin = true;
					}
				}
				else if (value > maxValue - 60f)
				{
					if (limitJoint != null && limitMin)
					{
						UnityEngine.Object.Destroy(limitJoint);
					}
					if (limitJoint == null)
					{
						limitJoint = CreateLimitJoint(Mathf.Max(maxValue - value - 120f, minValue - value), maxValue - value);
						limitMin = false;
					}
				}
				else if (limitJoint != null && value > minValue + 90f && value < maxValue - 90f)
				{
					UnityEngine.Object.Destroy(limitJoint);
				}
			}
			limitUpdateValue = value;
		}

		protected Joint CreateLimitJoint(float min, float max)
		{
			ConfigurableJoint configurableJoint = body.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.anchor = joint.anchor;
			configurableJoint.axis = joint.axis;
			configurableJoint.autoConfigureConnectedAnchor = false;
			configurableJoint.connectedBody = joint.connectedBody;
			configurableJoint.connectedAnchor = joint.connectedAnchor;
			ConfigurableJointMotion configurableJointMotion2 = configurableJoint.zMotion = ConfigurableJointMotion.Free;
			configurableJointMotion2 = (configurableJoint.xMotion = (configurableJoint.yMotion = configurableJointMotion2));
			configurableJointMotion2 = (configurableJoint.angularYMotion = (configurableJoint.angularZMotion = ConfigurableJointMotion.Free));
			configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
			configurableJoint.lowAngularXLimit = new SoftJointLimit
			{
				limit = 0f - max
			};
			configurableJoint.highAngularXLimit = new SoftJointLimit
			{
				limit = 0f - min
			};
			return configurableJoint;
		}

		public override float GetValue()
		{
			float num;
			for (num = ((!(anchorTransform != null)) ? Math3d.SignedVectorAngle(connectedForward, body.TransformDirection(localForward), connectedAxis) : Math3d.SignedVectorAngle(connectedForward, anchorTransform.InverseTransformDirection(body.TransformDirection(localForward)), connectedAxis)); num - lastKnownAngle < -180f; num += 360f)
			{
			}
			while (num - lastKnownAngle > 180f)
			{
				num -= 360f;
			}
			lastKnownAngle = num;
			return num;
		}

		public override void SetValue(float angle)
		{
			lastKnownAngle = angle;
			Quaternion quaternion = Quaternion.AngleAxis(angle, connectedAxis) * relativeRotation;
			if (anchorTransform != null)
			{
				quaternion = anchorTransform.rotation * quaternion;
			}
			if (rigid != null)
			{
				rigid.MoveRotation(quaternion);
				if (!rigid.isKinematic)
				{
					body.transform.rotation = quaternion;
				}
			}
			else
			{
				body.rotation = quaternion;
			}
		}

		public override void ResetState(int checkpoint, int subObjectives)
		{
			if (limitJoint != null)
			{
				UnityEngine.Object.Destroy(limitJoint);
				limitJoint = null;
			}
			base.ResetState(checkpoint, subObjectives);
		}

		public override void ApplyState(NetStream state)
		{
			base.ApplyState(state);
			UpdateLimitJoint();
		}

		public override void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			base.ApplyLerpedState(state0, state1, mix);
			UpdateLimitJoint();
		}
	}
}
