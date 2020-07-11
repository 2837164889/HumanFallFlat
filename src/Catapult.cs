using HumanAPI;
using UnityEngine;

public class Catapult : MonoBehaviour, IPostReset
{
	public enum CatapultState
	{
		Idle,
		Wind,
		Trigger,
		Fire
	}

	public Sound2 ratchetSound;

	public Sound2 releaseSound;

	public CatapultState state;

	public CatapultRope rope;

	public Rigidbody arm;

	public Rigidbody body;

	public Rigidbody windlass;

	public HingeJoint release;

	public Transform springRatchet;

	public Transform springFrame;

	private bool shoot;

	private bool armed;

	public float gearRatio = 12f;

	public float windlassAngle;

	public float topWindlassAngle = 660f;

	public float bottomWindlassAngle = -276f;

	public float currentTooth = -90f;

	private float toothStep = 30f;

	private float speed;

	private float acceleration = -3600f;

	private float oldPullAngle;

	private Vector3 startPos;

	private Quaternion startRot;

	private float initialWindlassAngle;

	private float timeReleased;

	private float timeFired;

	private bool fired;

	private void Awake()
	{
		startPos = base.transform.position;
		startRot = base.transform.rotation;
		initialWindlassAngle = windlassAngle;
	}

	private void Start()
	{
		LockPull();
		LockTrigger();
		PostResetState(0);
	}

	public void PostResetState(int checkpoint)
	{
		shoot = (armed = false);
		currentTooth = (windlassAngle = initialWindlassAngle);
		SetState(CatapultState.Idle);
		LeaveIdle();
		EnterWind();
		body.velocity = Vector3.zero;
		body.angularVelocity = Vector3.zero;
		base.transform.position = startPos;
		base.transform.rotation = startRot;
		arm.transform.rotation = WriteArm();
		windlass.transform.rotation = WriteWindlass();
		LeaveWind();
		EnterIdle();
	}

	private void FixedUpdate()
	{
		switch (state)
		{
		case CatapultState.Idle:
			UpdateIdle();
			break;
		case CatapultState.Wind:
			UpdateWind();
			break;
		case CatapultState.Trigger:
			UpdateTrigger();
			break;
		case CatapultState.Fire:
			UpdateFire();
			break;
		}
		rope.catapultWindCount = 1.5f - windlassAngle / 360f;
	}

	private void SetState(CatapultState newState)
	{
		if (newState != state)
		{
			switch (state)
			{
			case CatapultState.Idle:
				LeaveIdle();
				break;
			case CatapultState.Wind:
				LeaveWind();
				break;
			case CatapultState.Trigger:
				LeaveTrigger();
				break;
			case CatapultState.Fire:
				LeaveFire();
				break;
			}
			state = newState;
			switch (state)
			{
			case CatapultState.Idle:
				EnterIdle();
				break;
			case CatapultState.Wind:
				EnterWind();
				break;
			case CatapultState.Trigger:
				EnterTrigger();
				break;
			case CatapultState.Fire:
				EnterFire();
				break;
			}
		}
	}

	private void EnterIdle()
	{
		body.isKinematic = false;
		arm.isKinematic = false;
		arm.gameObject.AddComponent<FixedJoint>().connectedBody = body;
	}

	private void LeaveIdle()
	{
		Object.DestroyImmediate(arm.GetComponent<FixedJoint>());
		body.isKinematic = true;
		arm.isKinematic = true;
	}

	private void UpdateIdle()
	{
		if (GrabManager.IsGrabbedAny(windlass.gameObject))
		{
			SetState(CatapultState.Wind);
		}
		else if (windlassAngle != topWindlassAngle && GrabManager.IsGrabbedAny(release.gameObject))
		{
			SetState(CatapultState.Trigger);
		}
	}

	private void EnterWind()
	{
		UnlockPull();
		UnlockTrigger();
	}

	private void LeaveWind()
	{
		LockPull();
		LockTrigger();
	}

	private void UpdateWind()
	{
		if (!GrabManager.IsGrabbedAny(windlass.gameObject))
		{
			if (timeReleased > 1f)
			{
				SetState(CatapultState.Idle);
				return;
			}
			timeReleased += Time.fixedDeltaTime;
		}
		else
		{
			timeReleased = 0f;
		}
		if (armed)
		{
			WriteWindlass();
			WriteArm();
			return;
		}
		ReadWindlass();
		if (windlassAngle < bottomWindlassAngle)
		{
			SetArmed(value: true);
			ratchetSound.PlayOneShot();
		}
		else if (windlassAngle < currentTooth - toothStep)
		{
			float pitch = 0.5f + 0.5f * Mathf.InverseLerp(topWindlassAngle, bottomWindlassAngle, windlassAngle);
			ratchetSound.PlayOneShot(1f, pitch);
			currentTooth -= toothStep;
		}
		WriteArm();
		PullBackWindlass();
		PullDownTrigger();
	}

	private void PullBackWindlass()
	{
		int num = (!armed) ? 30 : 2000;
		windlass.SafeAddForceAtPosition(-body.transform.forward * num, springRatchet.position);
		body.SafeAddForceAtPosition(body.transform.forward * num, springRatchet.position);
	}

	private void PullDownTrigger()
	{
		int num = (state != CatapultState.Trigger && state != CatapultState.Wind) ? 3000 : 50;
		release.GetComponent<Rigidbody>().SafeAddForceAtPosition(-body.transform.up * num, springRatchet.position);
		body.SafeAddForceAtPosition(body.transform.up * num, springRatchet.position);
	}

	private void EnterTrigger()
	{
		state = CatapultState.Trigger;
		UnlockTrigger();
	}

	private void LeaveTrigger()
	{
		LockTrigger();
	}

	private void UpdateTrigger()
	{
		if (!GrabManager.IsGrabbedAny(release.gameObject))
		{
			SetState(CatapultState.Idle);
		}
		else
		{
			if (!(release.angle < -5f))
			{
				return;
			}
			shoot = true;
			speed = 0f;
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				bool flag = human.groundManager.IsStanding(arm.gameObject);
				if (human.ragdoll.partLeftHand.sensor.grabBody == release.GetComponent<Rigidbody>() && (flag || human.ragdoll.partRightHand.sensor.grabBody == arm.GetComponent<Rigidbody>()))
				{
					human.ragdoll.partLeftHand.sensor.ReleaseGrab(1f);
				}
				if (human.ragdoll.partRightHand.sensor.grabBody == release.GetComponent<Rigidbody>() && (flag || human.ragdoll.partLeftHand.sensor.grabBody == arm.GetComponent<Rigidbody>()))
				{
					human.ragdoll.partRightHand.sensor.ReleaseGrab(1f);
				}
				human.ragdoll.ToggleHeavyArms(human.ragdoll.partLeftHand.sensor.grabBody == arm.GetComponent<Rigidbody>(), human.ragdoll.partRightHand.sensor.grabBody == arm.GetComponent<Rigidbody>());
				if (flag)
				{
					StatsAndAchievements.UnlockAchievement(Achievement.ACH_SIEGE_HUMAN_CANNON);
				}
			}
			SetState(CatapultState.Fire);
			releaseSound.PlayOneShot();
		}
	}

	private void EnterFire()
	{
		state = CatapultState.Fire;
		SetArmed(value: false);
		UnlockPull();
		UnlockTrigger();
		fired = false;
	}

	private void LeaveFire()
	{
		LockPull();
		LockTrigger();
	}

	private void UpdateFire()
	{
		if (fired)
		{
			if (!GrabManager.IsGrabbedAny(release.gameObject))
			{
				if (timeFired > 1f)
				{
					SetState(CatapultState.Idle);
					return;
				}
				PullDownTrigger();
				timeFired += Time.fixedDeltaTime;
			}
			else
			{
				timeFired = 0f;
			}
			WriteArm();
			WriteWindlass();
			return;
		}
		timeFired = 0f;
		bool flag = GrabManager.IsGrabbedAny(arm.gameObject) || GroundManager.IsStandingAny(arm.gameObject);
		speed += acceleration * Time.fixedDeltaTime * ((!flag) ? 1f : 0.9f);
		windlassAngle -= speed * Time.fixedDeltaTime;
		if (windlassAngle > topWindlassAngle)
		{
			currentTooth = (windlassAngle = topWindlassAngle);
		}
		WriteArm();
		WriteWindlass();
		if (windlassAngle != topWindlassAngle)
		{
			return;
		}
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (human.ragdoll.partLeftHand.sensor.grabBody == arm.GetComponent<Rigidbody>())
			{
				human.ragdoll.partLeftHand.sensor.ReleaseGrab(0.1f);
			}
			if (human.ragdoll.partRightHand.sensor.grabBody == arm.GetComponent<Rigidbody>())
			{
				human.ragdoll.partRightHand.sensor.ReleaseGrab(0.1f);
			}
			human.ragdoll.ReleaseHeavyArms();
		}
		fired = true;
	}

	private void ReadWindlass()
	{
		Vector3 eulerAngles = windlass.transform.localRotation.eulerAngles;
		float num;
		for (num = eulerAngles.z - windlassAngle; num < -180f; num += 360f)
		{
		}
		while (num > 180f)
		{
			num -= 360f;
		}
		windlassAngle += num;
	}

	private Quaternion WriteWindlass()
	{
		Quaternion quaternion = windlass.transform.parent.rotation * Quaternion.Euler(0f, -90f, windlassAngle);
		windlass.MoveRotation(quaternion);
		return quaternion;
	}

	private Quaternion WriteArm()
	{
		Quaternion quaternion = arm.transform.parent.rotation * Quaternion.Euler((0f - windlassAngle) / gearRatio, 0f, 0f);
		arm.MoveRotation(quaternion);
		return quaternion;
	}

	private void LockPull()
	{
		windlass.gameObject.AddComponent<FixedJoint>().connectedBody = body;
	}

	private void UnlockPull()
	{
		Object.DestroyImmediate(windlass.GetComponent<FixedJoint>());
	}

	private void LockTrigger()
	{
		release.gameObject.AddComponent<FixedJoint>().connectedBody = body;
	}

	private void UnlockTrigger()
	{
		Object.DestroyImmediate(release.GetComponent<FixedJoint>());
	}

	private void SetArmed(bool value)
	{
		if (armed != value)
		{
			armed = value;
		}
	}
}
