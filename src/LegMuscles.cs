using System;
using UnityEngine;

[Serializable]
public class LegMuscles
{
	private readonly Human human;

	private readonly Ragdoll ragdoll;

	private readonly HumanMotion2 motion;

	private PhysicMaterial ballMaterial;

	private PhysicMaterial footMaterial;

	private float ballFriction;

	private float footFriction;

	private float ballRadius;

	private float stepToAlignOverrideDuration = 0.5f;

	private float stepToAlignOverride;

	public float legPhase;

	private float forwardImpulse;

	private float upImpulse;

	private int framesToApplyJumpImpulse;

	public LegMuscles(Human human, Ragdoll ragdoll, HumanMotion2 motion)
	{
		this.human = human;
		this.ragdoll = ragdoll;
		this.motion = motion;
		ballRadius = (ragdoll.partBall.collider as SphereCollider).radius;
		ballMaterial = ragdoll.partBall.collider.material;
		footMaterial = ragdoll.partLeftFoot.collider.material;
		ballFriction = ballMaterial.staticFriction;
		footFriction = footMaterial.staticFriction;
	}

	public void OnFixedUpdate(Vector3 torsoFeedback)
	{
		stepToAlignOverride -= Time.fixedDeltaTime;
		float num = (human.state != HumanState.Slide) ? ballFriction : 0f;
		if (num != ballMaterial.staticFriction)
		{
			PhysicMaterial physicMaterial = ballMaterial;
			float num2 = num;
			ballMaterial.dynamicFriction = num2;
			physicMaterial.staticFriction = num2;
			PhysicMaterial physicMaterial2 = footMaterial;
			num2 = ((human.state != HumanState.Slide) ? footFriction : 0f);
			footMaterial.dynamicFriction = num2;
			physicMaterial2.staticFriction = num2;
			ragdoll.partBall.collider.sharedMaterial = ballMaterial;
			ragdoll.partLeftFoot.collider.sharedMaterial = footMaterial;
			ragdoll.partRightFoot.collider.sharedMaterial = footMaterial;
		}
		switch (human.state)
		{
		case HumanState.Idle:
		{
			int num3 = (!human.controls.leftGrab && !human.controls.rightGrab) ? 90 : 75;
			if (Vector2.Angle(new Vector2(0f, 1f).Rotate((0f - human.controls.targetYawAngle) * ((float)Math.PI / 180f)), ragdoll.partHips.transform.forward.To2D()) > (float)num3)
			{
				stepToAlignOverride = stepToAlignOverrideDuration;
			}
			if (stepToAlignOverride > 0f)
			{
				RunAnimation(torsoFeedback, 0.5f);
			}
			else
			{
				StandAnimation(torsoFeedback, 1f);
			}
			break;
		}
		case HumanState.Walk:
			RunAnimation(torsoFeedback, 1f);
			break;
		case HumanState.Climb:
			if (human.controls.walkSpeed > 0f)
			{
				RunAnimation(torsoFeedback, 0f);
			}
			else
			{
				StandAnimation(torsoFeedback, 0f);
			}
			break;
		case HumanState.Jump:
			JumpAnimation(torsoFeedback);
			break;
		case HumanState.Slide:
			StandAnimation(torsoFeedback, 1f);
			break;
		case HumanState.Fall:
			NoAnimation(torsoFeedback);
			break;
		case HumanState.FreeFall:
			NoAnimation(torsoFeedback);
			break;
		case HumanState.Unconscious:
			NoAnimation(torsoFeedback);
			break;
		case HumanState.Dead:
			NoAnimation(torsoFeedback);
			break;
		case HumanState.Spawning:
			NoAnimation(torsoFeedback);
			break;
		}
	}

	private void NoAnimation(Vector3 torsoFeedback)
	{
		if (!CheatCodes.throwCheat || human.state != HumanState.Fall || !(human.grabbedByHuman != null))
		{
			ragdoll.partHips.rigidbody.SafeAddForce(torsoFeedback);
		}
	}

	private void StandAnimation(Vector3 torsoFeedback, float tonus)
	{
		HumanMotion2.AlignToVector(ragdoll.partLeftThigh, -ragdoll.partLeftThigh.transform.up, Vector3.up, 10f * tonus);
		HumanMotion2.AlignToVector(ragdoll.partRightThigh, -ragdoll.partRightThigh.transform.up, Vector3.up, 10f * tonus);
		HumanMotion2.AlignToVector(ragdoll.partLeftLeg, -ragdoll.partLeftLeg.transform.up, Vector3.up, 10f * tonus);
		HumanMotion2.AlignToVector(ragdoll.partRightLeg, -ragdoll.partRightLeg.transform.up, Vector3.up, 10f * tonus);
		ragdoll.partBall.rigidbody.SafeAddForce(torsoFeedback * 0.2f);
		ragdoll.partLeftFoot.rigidbody.SafeAddForce(torsoFeedback * 0.4f);
		ragdoll.partRightFoot.rigidbody.SafeAddForce(torsoFeedback * 0.4f);
		ragdoll.partBall.rigidbody.angularVelocity = Vector3.zero;
	}

	private void RunAnimation(Vector3 torsoFeedback, float tonus)
	{
		legPhase = Time.realtimeSinceStartup * 1.5f;
		torsoFeedback += AnimateLeg(ragdoll.partLeftThigh, ragdoll.partLeftLeg, ragdoll.partLeftFoot, legPhase, torsoFeedback, tonus);
		torsoFeedback += AnimateLeg(ragdoll.partRightThigh, ragdoll.partRightLeg, ragdoll.partRightFoot, legPhase + 0.5f, torsoFeedback, tonus);
		ragdoll.partBall.rigidbody.SafeAddForce(torsoFeedback);
		RotateBall();
		AddWalkForce();
	}

	private void JumpAnimation(Vector3 torsoFeedback)
	{
		ragdoll.partHips.rigidbody.SafeAddForce(torsoFeedback);
		if (human.jump)
		{
			float num = 0.75f;
			int num2 = 2;
			float num3 = Mathf.Sqrt(2f * num / Physics.gravity.magnitude);
			Vector3 groudSpeed = human.groundManager.groudSpeed;
			float f = Mathf.Clamp(groudSpeed.y, 0f, 100f);
			f = Mathf.Pow(f, 1.2f);
			num3 += f / Physics.gravity.magnitude;
			float num4 = num3 * human.weight;
			float num5 = human.controls.unsmoothedWalkSpeed * ((float)num2 + f / 2f) * human.mass;
			Vector3 momentum = human.momentum;
			float num6 = Vector3.Dot(human.controls.walkDirection.normalized, momentum);
			if (num6 < 0f)
			{
				num6 = 0f;
			}
			upImpulse = num4 - momentum.y;
			if (upImpulse < 0f)
			{
				upImpulse = 0f;
			}
			forwardImpulse = num5 - num6;
			if (forwardImpulse < 0f)
			{
				forwardImpulse = 0f;
			}
			framesToApplyJumpImpulse = 1;
			if (human.onGround || Time.time - human.GetComponent<Ball>().timeSinceLastNonzeroImpulse < 0.2f)
			{
				upImpulse /= framesToApplyJumpImpulse;
				forwardImpulse /= framesToApplyJumpImpulse;
				ApplyJumpImpulses();
				framesToApplyJumpImpulse--;
			}
			human.skipLimiting = true;
			human.jump = false;
		}
		else
		{
			if (framesToApplyJumpImpulse-- > 0)
			{
				ApplyJumpImpulses();
			}
			int num7 = 3;
			int num8 = 500;
			float num9 = human.controls.unsmoothedWalkSpeed * (float)num7 * human.mass;
			Vector3 momentum2 = human.momentum;
			float num10 = Vector3.Dot(human.controls.walkDirection.normalized, momentum2);
			float num11 = num9 - num10;
			float d = Mathf.Clamp(num11 / Time.fixedDeltaTime, 0f, num8);
			ragdoll.partChest.rigidbody.SafeAddForce(d * human.controls.walkDirection.normalized);
		}
	}

	private void ApplyJumpImpulses()
	{
		float d = 1f;
		for (int i = 0; i < human.groundManager.groundObjects.Count; i++)
		{
			if (human.grabManager.IsGrabbed(human.groundManager.groundObjects[i]))
			{
				d = 0.75f;
			}
		}
		Vector3 a = Vector3.up * upImpulse * d;
		Vector3 a2 = human.controls.walkDirection.normalized * forwardImpulse * d;
		ragdoll.partHead.rigidbody.SafeAddForce(a * 0.1f + a2 * 0.1f, ForceMode.Impulse);
		ragdoll.partChest.rigidbody.SafeAddForce(a * 0.1f + a2 * 0.1f, ForceMode.Impulse);
		ragdoll.partWaist.rigidbody.SafeAddForce(a * 0.1f + a2 * 0.1f, ForceMode.Impulse);
		ragdoll.partHips.rigidbody.SafeAddForce(a * 0.1f + a2 * 0.1f, ForceMode.Impulse);
		ragdoll.partBall.rigidbody.SafeAddForce(a * 0.1f + a2 * 0.1f, ForceMode.Impulse);
		ragdoll.partLeftThigh.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partRightThigh.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partLeftLeg.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partRightLeg.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partLeftFoot.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partRightFoot.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partLeftArm.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partRightArm.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partLeftForearm.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		ragdoll.partRightForearm.rigidbody.SafeAddForce(a * 0.05f + a2 * 0.05f, ForceMode.Impulse);
		human.groundManager.DistributeForce(-a / Time.fixedDeltaTime, ragdoll.partBall.rigidbody.position);
	}

	private void RotateBall()
	{
		float num = (human.state != HumanState.Walk) ? 1.2f : 2.5f;
		Vector3 a = new Vector3(human.controls.walkDirection.z, 0f, 0f - human.controls.walkDirection.x);
		ragdoll.partBall.rigidbody.angularVelocity = num / ballRadius * a;
		ragdoll.partBall.rigidbody.maxAngularVelocity = ragdoll.partBall.rigidbody.angularVelocity.magnitude;
	}

	private void AddWalkForce()
	{
		float d = 300f;
		Vector3 vector = human.controls.walkDirection * d;
		ragdoll.partBall.rigidbody.SafeAddForce(vector);
		if (human.onGround)
		{
			human.groundManager.DistributeForce(-vector, ragdoll.partBall.rigidbody.position);
		}
		else if (human.hasGrabbed)
		{
			human.grabManager.DistributeForce(-vector * 0.5f);
		}
	}

	private Vector3 AnimateLeg(HumanSegment thigh, HumanSegment leg, HumanSegment foot, float phase, Vector3 torsoFeedback, float tonus)
	{
		tonus *= 20f;
		phase -= Mathf.Floor(phase);
		if (phase < 0.2f)
		{
			HumanMotion2.AlignToVector(thigh, thigh.transform.up, human.controls.walkDirection + Vector3.down, 3f * tonus);
			HumanMotion2.AlignToVector(leg, thigh.transform.up, -human.controls.walkDirection - Vector3.up, tonus);
			Vector3 vector = Vector3.up * 20f;
			foot.rigidbody.SafeAddForce(vector);
			return -vector;
		}
		if (phase < 0.5f)
		{
			HumanMotion2.AlignToVector(thigh, thigh.transform.up, human.controls.walkDirection, 2f * tonus);
			HumanMotion2.AlignToVector(leg, thigh.transform.up, human.controls.walkDirection, 3f * tonus);
		}
		else
		{
			if (phase < 0.7f)
			{
				Vector3 vector2 = torsoFeedback * 0.2f;
				foot.rigidbody.SafeAddForce(vector2);
				HumanMotion2.AlignToVector(thigh, thigh.transform.up, human.controls.walkDirection + Vector3.down, tonus);
				HumanMotion2.AlignToVector(leg, thigh.transform.up, Vector3.down, tonus);
				return -vector2;
			}
			if (phase < 0.9f)
			{
				Vector3 vector3 = torsoFeedback * 0.2f;
				foot.rigidbody.SafeAddForce(vector3);
				HumanMotion2.AlignToVector(thigh, thigh.transform.up, -human.controls.walkDirection + Vector3.down, tonus);
				HumanMotion2.AlignToVector(leg, thigh.transform.up, -human.controls.walkDirection + Vector3.down, tonus);
				return -vector3;
			}
			HumanMotion2.AlignToVector(thigh, thigh.transform.up, -human.controls.walkDirection + Vector3.down, tonus);
			HumanMotion2.AlignToVector(leg, thigh.transform.up, -human.controls.walkDirection, tonus);
		}
		return Vector3.zero;
	}
}
