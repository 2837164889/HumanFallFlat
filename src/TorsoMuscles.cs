using System;
using UnityEngine;

[Serializable]
public class TorsoMuscles
{
	private readonly Human human;

	private readonly Ragdoll ragdoll;

	private readonly HumanMotion2 motion;

	public Vector3 feedbackForce;

	private float timeSinceUnconsious;

	private float timeSinceOffGround;

	private float idleAnimationPhase;

	private float idleAnimationDuration = 3f;

	public float predictTime = 0.001f;

	public float springSpeed = 0.01f;

	public float m = 1f;

	public float headOffset = 0.5f;

	public float headApparentMass = 1f;

	public float headMaxForceY = 40f;

	public float headMaxForceZ = 30f;

	public float chestOffset = 0.5f;

	public float chestApparentMass = 5f;

	public float chestMaxForceY = 20f;

	public float chestMaxForceZ = 20f;

	public float waistOffset = 0.5f;

	public float waistApparentMass = 10f;

	public float waistMaxForceY = 30f;

	public float waistMaxForceZ = 30f;

	public float hipsOffset = 0.5f;

	public float hipsApparentMass = 2f;

	public float hipsMaxForceY = 50f;

	public float hipsMaxForceZ = 50f;

	public float chestAngle = 1f;

	public float waistAngle = 0.7f;

	public float hipsAngle = 0.4f;

	public TorsoMuscles(Human human, Ragdoll ragdoll, HumanMotion2 motion)
	{
		this.human = human;
		this.ragdoll = ragdoll;
		this.motion = motion;
	}

	public void OnFixedUpdate()
	{
		timeSinceUnconsious += Time.fixedDeltaTime;
		timeSinceOffGround += Time.fixedDeltaTime;
		if (!human.onGround)
		{
			timeSinceOffGround = 0f;
		}
		feedbackForce = Vector3.zero;
		HumanState humanState = human.state;
		if (humanState == HumanState.Fall && human.grabbedByHuman != null)
		{
			humanState = HumanState.Idle;
		}
		switch (humanState)
		{
		case HumanState.Dead:
			break;
		case HumanState.Spawning:
			break;
		case HumanState.Idle:
		case HumanState.Slide:
			feedbackForce = IdleAnimation();
			break;
		case HumanState.Walk:
			feedbackForce = StandAnimation();
			break;
		case HumanState.Climb:
			feedbackForce = ClimbAnimation();
			break;
		case HumanState.Jump:
			feedbackForce = JumpAnimation();
			break;
		case HumanState.Fall:
			feedbackForce = FallAnimation();
			break;
		case HumanState.FreeFall:
			feedbackForce = FreeFallAnimation();
			break;
		case HumanState.Unconscious:
			timeSinceUnconsious = 0f;
			break;
		}
	}

	private Vector3 IdleAnimation()
	{
		idleAnimationPhase = MathUtils.Wrap(idleAnimationPhase + Time.deltaTime / idleAnimationDuration, 1f);
		float torsoBend = Mathf.Lerp(1f, -0.5f, Mathf.Sin(idleAnimationPhase * (float)Math.PI * 2f) / 2f + 0.5f);
		return ApplyTorsoPose(1f, 1f, torsoBend, 1f);
	}

	private Vector3 StandAnimation()
	{
		return ApplyTorsoPose(1f, 1f, 0f, 1f);
	}

	private Vector3 ClimbAnimation()
	{
		float t = Mathf.Clamp01((human.controls.targetPitchAngle - 10f) / 60f);
		int num = ((ragdoll.partLeftHand.sensor.grabJoint != null || human.controls.leftGrab) && (ragdoll.partRightHand.sensor.grabJoint != null || human.controls.rightGrab)) ? 1 : 0;
		return ApplyTorsoPose(num, 1f, 0f, Mathf.Lerp(0.2f, 1f, t));
	}

	private Vector3 JumpAnimation()
	{
		float lift = 1f;
		return ApplyTorsoPose(1f, 1f, 0f, lift);
	}

	private Vector3 FallAnimation()
	{
		float lift = 0.3f;
		return ApplyTorsoPose(0.5f, 1f, 0f, lift);
	}

	private Vector3 FreeFallAnimation()
	{
		human.AddRandomTorque(0.01f);
		float num = Mathf.Sin(Time.time * 3f) * 0.1f;
		float weight = human.weight;
		ragdoll.partHips.rigidbody.SafeAddForce(-Vector3.up * weight * num);
		ragdoll.partChest.rigidbody.SafeAddForce(-Vector3.up * weight * (0f - num));
		return Vector3.zero;
	}

	public Vector3 ApplyTorsoPose(float torsoTonus, float headTonus, float torsoBend, float lift)
	{
		lift *= Mathf.Clamp01(timeSinceUnconsious / 3f) * Mathf.Clamp01(timeSinceOffGround * 0.2f + 0.8f);
		torsoTonus *= Mathf.Clamp01(timeSinceUnconsious);
		torsoTonus *= 2f;
		headTonus *= 2f;
		float num = human.weight * 0.8f * lift;
		float num2 = human.controls.targetPitchAngle;
		if (human.hasGrabbed)
		{
			num2 = (num2 + 80f) * 0.5f - 80f;
		}
		HumanMotion2.AlignLook(ragdoll.partHead, Quaternion.Euler(num2, human.controls.targetYawAngle, 0f), 2f * headTonus, 10f * headTonus);
		if (human.onGround || human.state == HumanState.Climb)
		{
			torsoBend *= 40f;
			HumanMotion2.AlignLook(ragdoll.partChest, Quaternion.Euler(human.controls.targetPitchAngle + torsoBend, human.controls.targetYawAngle, 0f), 2f * torsoTonus, 10f * torsoTonus);
			HumanMotion2.AlignLook(ragdoll.partWaist, Quaternion.Euler(human.controls.targetPitchAngle + torsoBend / 2f, human.controls.targetYawAngle, 0f), 1f * torsoTonus, 15f * torsoTonus);
			HumanMotion2.AlignLook(ragdoll.partHips, Quaternion.Euler(human.controls.targetPitchAngle, human.controls.targetYawAngle, 0f), 0.5f * torsoTonus, 20f * torsoTonus);
		}
		float num3 = 0f;
		if (human.targetDirection.y > 0f)
		{
			num3 = human.targetDirection.y * 0.25f;
			if (human.onGround && human.ragdoll.partLeftHand.sensor.grabBody != null)
			{
				num3 *= 1.5f;
			}
			if (human.onGround && human.ragdoll.partRightHand.sensor.grabBody != null)
			{
				num3 *= 1.5f;
			}
		}
		else
		{
			num3 = 0f - human.targetDirection.y;
		}
		Vector3 vector = Mathf.Lerp(0.2f, 0f, num3) * num * headTonus * Vector3.up;
		Vector3 vector2 = Mathf.Lerp(0.6f, 0f, num3) * num * torsoTonus * Vector3.up;
		Vector3 vector3 = Mathf.Lerp(0.2f, 0.5f, num3) * num * torsoTonus * Vector3.up;
		Vector3 vector4 = Mathf.Lerp(0f, 0.5f, num3) * num * torsoTonus * Vector3.up;
		if (human.controls.leftGrab)
		{
			UnblockArmBehindTheBack(ragdoll.partLeftHand, -1f);
		}
		if (human.controls.rightGrab)
		{
			UnblockArmBehindTheBack(ragdoll.partRightHand, 1f);
		}
		ragdoll.partHead.rigidbody.SafeAddForce(vector);
		ragdoll.partChest.rigidbody.SafeAddForce(vector2);
		ragdoll.partWaist.rigidbody.SafeAddForce(vector3);
		ragdoll.partHips.rigidbody.SafeAddForce(vector4);
		StabilizeHorizontal(ragdoll.partHips.rigidbody, ragdoll.partBall.rigidbody, 1f * lift * Mathf.Lerp(1f, 0.25f, Mathf.Abs(num3)));
		StabilizeHorizontal(ragdoll.partHead.rigidbody, ragdoll.partBall.rigidbody, 0.2f * lift * Mathf.Lerp(1f, 0f, Mathf.Abs(num3)));
		return -(vector + vector2 + vector3 + vector4);
	}

	private void UnblockArmBehindTheBack(HumanSegment hand, float direction)
	{
		Vector3 center = ragdoll.partHead.collider.bounds.center;
		Vector3 center2 = hand.collider.bounds.center;
		Vector3 vector = ragdoll.partChest.transform.InverseTransformVector(center2 - center);
		float num = Mathf.InverseLerp(0f, -0.1f, vector.z) * Mathf.InverseLerp(0.1f, -0.1f, vector.x * direction) * Mathf.InverseLerp(-0.3f, -0.1f, vector.y);
		if (num > 0f)
		{
			ragdoll.partHead.rigidbody.SafeAddForce((0f - direction) * num * ragdoll.partChest.transform.right * 200f);
			hand.rigidbody.SafeAddForce(direction * num * ragdoll.partChest.transform.right * 200f);
		}
	}

	private void StabilizeHorizontal(Rigidbody top, Rigidbody bottom, float multiplier)
	{
		float d = 3f;
		Vector3 a = bottom.position + bottom.velocity * Time.fixedDeltaTime - top.position - top.velocity * Time.fixedDeltaTime * d;
		a.y = 0f;
		Vector3 vector = a * top.mass / Time.fixedDeltaTime;
		vector *= multiplier;
		top.SafeAddForce(vector);
		bottom.SafeAddForce(-vector);
	}
}
