using UnityEngine;

public class SignalBroadcastAngle : SignalBase
{
	public float fromAngle;

	public float fromDeadAngle;

	public float toDeadAngle;

	public float toAngle;

	public HingeJoint joint;

	public bool snapZero;

	public bool snapPositive;

	public bool snapNegative;

	public float holdZeroSpringWhenNoSnap;

	private float respringBlock;

	public GameObject jointHolder;

	private Quaternion invInitialLocalRotation;

	private void Awake()
	{
		if (jointHolder == null)
		{
			jointHolder = joint.gameObject;
		}
		if (jointHolder.GetComponents<HingeJoint>().Length > 1)
		{
			Debug.LogError("SignalBroadcastAngle has multiple jooints", this);
		}
	}

	public override void PostResetState(int checkpoint)
	{
		base.PostResetState(checkpoint);
		if (joint == null)
		{
			joint = jointHolder.GetComponent<HingeJoint>();
		}
		JointSpring spring = joint.spring;
		spring.targetPosition = (fromDeadAngle + toDeadAngle) / 2f;
		joint.spring = spring;
		respringBlock = 0.5f;
		CheckHoldZero();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		CheckHoldZero();
		invInitialLocalRotation = Quaternion.Inverse(joint.transform.localRotation);
	}

	private void CheckHoldZero()
	{
		if (holdZeroSpringWhenNoSnap != 0f)
		{
			JointSpring spring = joint.spring;
			float targetPosition = spring.targetPosition;
			float spring2 = spring.spring;
			if (value == 0f)
			{
				spring.targetPosition = (fromDeadAngle + toDeadAngle) / 2f;
				spring.spring = holdZeroSpringWhenNoSnap;
			}
			else
			{
				spring.spring = 0f;
			}
			if (targetPosition != spring.targetPosition || spring2 != spring.spring)
			{
				joint.spring = spring;
			}
		}
	}

	private float GetJointAngle()
	{
		(invInitialLocalRotation * joint.transform.localRotation).ToAngleAxis(out float angle, out Vector3 axis);
		if (Vector3.Dot(axis, joint.axis) < 0f)
		{
			return 0f - angle;
		}
		return angle;
	}

	private void FixedUpdate()
	{
		if (joint == null)
		{
			return;
		}
		float jointAngle = GetJointAngle();
		float num = 0f;
		if (jointAngle < fromDeadAngle)
		{
			num = 0f - Mathf.InverseLerp(fromDeadAngle, fromAngle, jointAngle);
		}
		if (jointAngle > toDeadAngle)
		{
			num = Mathf.InverseLerp(toDeadAngle, toAngle, jointAngle);
		}
		SetValue(num);
		if (respringBlock > 0f)
		{
			respringBlock -= Time.fixedDeltaTime;
			if (respringBlock > 0f)
			{
				return;
			}
		}
		CheckHoldZero();
		JointSpring spring = joint.spring;
		float targetPosition = spring.targetPosition;
		if (snapNegative && num < -0.75f)
		{
			spring.targetPosition = fromAngle;
		}
		if (snapPositive && num > 0.75f)
		{
			spring.targetPosition = toAngle;
		}
		if (snapZero && num > -0.5f && num < 0.5f)
		{
			spring.targetPosition = (fromDeadAngle + toDeadAngle) / 2f;
		}
		if (spring.targetPosition != targetPosition)
		{
			joint.spring = spring;
		}
	}
}
