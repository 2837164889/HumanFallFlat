using UnityEngine;

namespace HumanAPI
{
	public class PullJoint : JointImplementation
	{
		[Tooltip("Sting extends beweet anchor and hook")]
		public Transform hook;

		[Tooltip("Sting extends beweet anchor and hook")]
		public Transform anchorPoint;

		private Vector3 connectedAxis;

		private Vector3 connectedCenter;

		private Vector3 localPos;

		public override void CreateMainJoint()
		{
			if (anchorTransform == null)
			{
				Debug.LogError("Pull joint needs an achor to function");
			}
			if (useSpring)
			{
				Debug.LogError("Pull joint can't use spring");
			}
			if (hook == null)
			{
				hook = base.transform;
			}
			if (anchorPoint == null)
			{
				anchorPoint = anchor;
			}
			centerValue = (hook.position - anchorPoint.position).magnitude;
			connectedAxis = (hook.position - anchorPoint.position).normalized;
			connectedCenter = anchorPoint.position;
			if (anchorTransform != null)
			{
				connectedAxis = anchorTransform.InverseTransformDirection(connectedAxis);
				connectedCenter = anchorTransform.InverseTransformPoint(connectedCenter);
			}
			localPos = body.InverseTransformPoint(hook.position);
			if (!base.isKinematic)
			{
				joint = body.gameObject.AddComponent<ConfigurableJoint>();
				joint.autoConfigureConnectedAnchor = false;
				joint.anchor = localPos;
				joint.connectedAnchor = connectedCenter;
				joint.connectedBody = anchorRigid;
				joint.enableCollision = enableCollision;
				joint.xMotion = ConfigurableJointMotion.Limited;
				joint.yMotion = ConfigurableJointMotion.Limited;
				joint.zMotion = ConfigurableJointMotion.Limited;
				joint.angularXMotion = ConfigurableJointMotion.Free;
				joint.angularYMotion = ConfigurableJointMotion.Free;
				joint.angularZMotion = ConfigurableJointMotion.Free;
				joint.linearLimit = new SoftJointLimit
				{
					limit = centerValue
				};
			}
		}

		private void FixedUpdate()
		{
			EnsureInitialized();
			if (!(joint == null) && useSpring && joint.linearLimit.limit != centerValue + target)
			{
				joint.linearLimit = new SoftJointLimit
				{
					limit = centerValue + target
				};
				rigid.WakeUp();
			}
		}

		public override float GetValue()
		{
			EnsureInitialized();
			if (joint != null)
			{
				return joint.linearLimit.limit - centerValue;
			}
			Vector3 vector = body.TransformPoint(localPos);
			if (anchorTransform != null)
			{
				return Vector3.Dot(anchorTransform.InverseTransformPoint(vector) - connectedCenter, connectedAxis);
			}
			return Vector3.Dot(vector - connectedCenter, connectedAxis);
		}

		public override void SetValue(float pos)
		{
			EnsureInitialized();
			if (joint != null)
			{
				if (joint.linearLimit.limit != centerValue + pos)
				{
					joint.linearLimit = new SoftJointLimit
					{
						limit = centerValue + pos
					};
					rigid.WakeUp();
				}
				return;
			}
			Vector3 position = connectedCenter + connectedAxis * (centerValue + pos);
			if (anchorTransform != null)
			{
				position = anchorTransform.TransformPoint(position);
			}
			position -= body.TransformDirection(localPos);
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
