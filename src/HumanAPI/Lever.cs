using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Lever", 10)]
	public class Lever : Node, IPostReset
	{
		[Tooltip("The value of the incoming signal")]
		public NodeInput snapTo;

		[Tooltip("The value for the outgoing signal")]
		public NodeOutput output;

		[Tooltip("Whether or not to add a CGShift component")]
		public bool centerCG = true;

		[Tooltip("Minumum Angle for the lever")]
		public float fromAngle;

		[Tooltip("Rate of change from the Dead Angle")]
		public float fromDeadAngle;

		[Tooltip("Rate of change to the Dead Angle ")]
		public float toDeadAngle;

		[Tooltip("Maximum Angle for the lever")]
		public float toAngle;

		[Tooltip("Changes from a discrete value to a lerp for the lever movement")]
		public bool discrete;

		[Tooltip("Reference to a configuarable joint")]
		public ConfigurableJoint joint;

		[Tooltip("Whether or not the lever should snap to 0 and stay there ")]
		public bool snapZero;

		[Tooltip("Whether or not the lever should snap to positive and stay there")]
		public bool snapPositive;

		[Tooltip("Whether or not the lever should nap to the negative and stay there")]
		public bool snapNegative;

		[Tooltip("Unused")]
		public float holdZeroSpringWhenNoSnap;

		[Tooltip("Reference to an AngularJoint")]
		public AngularJoint angularJoint;

		private float respringBlock;

		private Quaternion invInitialLocalRotation;

		[Tooltip("Store for the current angle of the lever")]
		public float angle;

		protected virtual void Awake()
		{
			if (centerCG && angularJoint.body.GetComponent<CGShift>() == null)
			{
				angularJoint.body.gameObject.AddComponent<CGShift>();
			}
			if (joint != null)
			{
				invInitialLocalRotation = joint.ReadInitialRotation();
			}
			if (joint == null && angularJoint == null)
			{
				Debug.LogError("Missing a joint", this);
			}
		}

		public override void Process()
		{
			base.Process();
			if (!(angularJoint != null) || angularJoint.jointCreated)
			{
				CheckSnap(snapTo.value, SignalManager.skipTransitions);
			}
		}

		public void PostResetState(int checkpoint)
		{
		}

		private void FixedUpdate()
		{
			if ((angularJoint != null && !angularJoint.jointCreated) || (angularJoint == null && joint == null))
			{
				return;
			}
			if (angularJoint != null)
			{
				angle = angularJoint.GetValue();
			}
			else
			{
				angle = joint.GetXAngle(invInitialLocalRotation);
			}
			float num = 0f;
			if (angle < fromDeadAngle)
			{
				num = ((!discrete) ? (0f - Mathf.InverseLerp(fromDeadAngle, fromAngle, angle)) : (-1f));
			}
			if (angle > toDeadAngle)
			{
				num = ((!discrete) ? Mathf.InverseLerp(toDeadAngle, toAngle, angle) : 1f);
			}
			if (respringBlock > 0f)
			{
				respringBlock -= Time.fixedDeltaTime;
				if (respringBlock > 0f)
				{
					return;
				}
			}
			if (output.value != num)
			{
				output.SetValue(num);
				if (snapTo.connectedNode == null)
				{
					CheckSnap(num, forcePosition: false);
				}
			}
		}

		protected virtual void CheckSnap(float value, bool forcePosition)
		{
			if (joint == null && angularJoint == null)
			{
				Debug.LogError("Missing a joint", this);
			}
			if (snapNegative && value < -0.75f)
			{
				Snap(fromAngle, forcePosition);
			}
			if (snapPositive && value > 0.75f)
			{
				Snap(toAngle, forcePosition);
			}
			if (snapZero && value > -0.5f && value < 0.5f)
			{
				Snap((fromDeadAngle + toDeadAngle) / 2f, forcePosition);
			}
		}

		private void Snap(float angle, bool forcePosition)
		{
			if (angularJoint != null)
			{
				angularJoint.SetTarget(angle);
			}
			else if (forcePosition)
			{
				joint.ApplyXAngle(invInitialLocalRotation, angle);
			}
			else
			{
				joint.SetXAngleTarget(angle);
			}
		}
	}
}
