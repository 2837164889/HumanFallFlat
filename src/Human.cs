using HumanAPI;
using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Human : HumanBase
{
	public static Human instance;

	public static List<Human> all = new List<Human>();

	public Vector3 targetDirection;

	public Vector3 targetLiftDirection;

	public bool jump;

	public bool disableInput;

	public NetPlayer player;

	public Ragdoll ragdoll;

	public HumanControls controls;

	internal GroundManager groundManager;

	internal GrabManager grabManager;

	[NonSerialized]
	public HumanMotion2 motionControl2;

	public HumanState state;

	public bool onGround;

	public float groundAngle;

	public bool hasGrabbed;

	public bool isClimbing;

	public Human grabbedByHuman;

	public float wakeUpTime;

	internal float maxWakeUpTime = 2f;

	public float unconsciousTime;

	private float maxUnconsciousTime = 3f;

	private Vector3 grabStartPosition;

	[NonSerialized]
	public Rigidbody[] rigidbodies;

	private Vector3[] velocities;

	public float weight;

	public float mass;

	private Vector3 lastVelocity;

	private float totalHit;

	private float lastFrameHit;

	private float thisFrameHit;

	private float fallTimer;

	private float groundDelay;

	private float jumpDelay;

	private float slideTimer;

	private float[] groundAngles = new float[60];

	private int groundAnglesIdx;

	private float groundAnglesSum;

	private float lastGroundAngle;

	private uint evtScroll;

	private NetIdentity identity;

	private FixedJoint hook;

	public bool skipLimiting;

	private bool isFallSpeedInitialized;

	private bool isFallSpeedLimited;

	private bool overridenDrag;

	public Vector3 momentum
	{
		get
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < rigidbodies.Length; i++)
			{
				Rigidbody rigidbody = rigidbodies[i];
				zero += rigidbody.velocity * rigidbody.mass;
			}
			return zero;
		}
	}

	public Vector3 velocity => momentum / mass;

	private void OnEnable()
	{
		all.Add(this);
		instance = this;
		grabManager = GetComponent<GrabManager>();
		groundManager = GetComponent<GroundManager>();
		motionControl2 = GetComponent<HumanMotion2>();
		controls = GetComponentInParent<HumanControls>();
	}

	private void OnDisable()
	{
		all.Remove(this);
	}

	public void Initialize()
	{
		ragdoll = GetComponentInChildren<Ragdoll>();
		motionControl2.Initialize();
		ServoSound componentInChildren = GetComponentInChildren<ServoSound>();
		HumanHead humanHead = ragdoll.partHead.transform.gameObject.AddComponent<HumanHead>();
		humanHead.sounds = componentInChildren;
		humanHead.humanAudio = GetComponentInChildren<HumanAudio>();
		componentInChildren.transform.SetParent(humanHead.transform, worldPositionStays: false);
		InitializeBodies();
	}

	private void InitializeBodies()
	{
		rigidbodies = GetComponentsInChildren<Rigidbody>();
		velocities = new Vector3[rigidbodies.Length];
		mass = 0f;
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			Rigidbody rigidbody = rigidbodies[i];
			if (rigidbody != null)
			{
				rigidbody.maxAngularVelocity = 10f;
				mass += rigidbody.mass;
			}
		}
		weight = mass * 9.81f;
	}

	internal void ReceiveHit(Vector3 impulse)
	{
		thisFrameHit = Mathf.Max(thisFrameHit, impulse.magnitude);
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		if (thisFrameHit + lastFrameHit > 30f)
		{
			MakeUnconscious();
			ReleaseGrab(3f);
		}
		lastFrameHit = thisFrameHit;
		thisFrameHit = 0f;
		jumpDelay -= Time.fixedDeltaTime;
		groundDelay -= Time.fixedDeltaTime;
		if (!disableInput)
		{
			ProcessInput();
		}
		LimitFallSpeed();
		Quaternion rotation = Quaternion.Euler(controls.targetPitchAngle, controls.targetYawAngle, 0f);
		targetDirection = rotation * Vector3.forward;
		targetLiftDirection = Quaternion.Euler(Mathf.Clamp(controls.targetPitchAngle, -70f, 80f), controls.targetYawAngle, 0f) * Vector3.forward;
		if (NetGame.isClient || ReplayRecorder.isPlaying)
		{
			return;
		}
		if (state == HumanState.Dead || state == HumanState.Unconscious || state == HumanState.Spawning)
		{
			controls.leftGrab = (controls.rightGrab = false);
			controls.shootingFirework = false;
		}
		groundAngle = 90f;
		groundAngle = Mathf.Min(groundAngle, ragdoll.partBall.sensor.groundAngle);
		groundAngle = Mathf.Min(groundAngle, ragdoll.partLeftFoot.sensor.groundAngle);
		groundAngle = Mathf.Min(groundAngle, ragdoll.partRightFoot.sensor.groundAngle);
		bool flag = hasGrabbed;
		onGround = (groundDelay <= 0f && groundManager.onGround);
		hasGrabbed = grabManager.hasGrabbed;
		ragdoll.partBall.sensor.groundAngle = (ragdoll.partLeftFoot.sensor.groundAngle = (ragdoll.partRightFoot.sensor.groundAngle = 90f));
		if (hasGrabbed)
		{
			Vector3 position = base.transform.position;
			if (position.y < grabStartPosition.y)
			{
				grabStartPosition = base.transform.position;
			}
		}
		if (hasGrabbed)
		{
			Vector3 position2 = base.transform.position;
			if (position2.y - grabStartPosition.y > 0.5f)
			{
				isClimbing = true;
				goto IL_02d6;
			}
		}
		isClimbing = false;
		goto IL_02d6;
		IL_02d6:
		if (flag != hasGrabbed && hasGrabbed)
		{
			grabStartPosition = base.transform.position;
		}
		if (state == HumanState.Spawning && onGround)
		{
			MakeUnconscious();
		}
		ProcessUnconscious();
		if (state != HumanState.Dead && state != HumanState.Unconscious && state != HumanState.Spawning)
		{
			ProcessFall();
			if (onGround)
			{
				if (controls.jump && jumpDelay <= 0f)
				{
					state = HumanState.Jump;
					jump = true;
					jumpDelay = 0.5f;
					groundDelay = 0.2f;
				}
				else if (controls.walkSpeed > 0f)
				{
					state = HumanState.Walk;
				}
				else
				{
					state = HumanState.Idle;
				}
			}
			else if (ragdoll.partLeftHand.sensor.grabObject != null || ragdoll.partRightHand.sensor.grabObject != null)
			{
				state = HumanState.Climb;
			}
		}
		if (skipLimiting)
		{
			skipLimiting = false;
			return;
		}
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			Vector3 vector = velocities[i];
			Vector3 velocity = rigidbodies[i].velocity;
			Vector3 vector2 = velocity - vector;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				Vector3 normalized = vector.normalized;
				float magnitude = vector.magnitude;
				float value = 0f - Vector3.Dot(normalized, vector2);
				float d = Mathf.Clamp(value, 0f, magnitude);
				vector2 += normalized * d;
			}
			float num = 1000f * Time.deltaTime;
			if (vector2.magnitude > num)
			{
				Vector3 b = Vector3.ClampMagnitude(vector2, num);
				velocity -= vector2 - b;
				rigidbodies[i].velocity = velocity;
			}
			velocities[i] = velocity;
		}
	}

	private void ProcessInput()
	{
		if (!NetGame.isClient && !ReplayRecorder.isPlaying)
		{
			if (controls.unconscious)
			{
				MakeUnconscious();
			}
			if (motionControl2.enabled)
			{
				motionControl2.OnFixedUpdate();
			}
		}
	}

	private void PushGroundAngle()
	{
		float num = lastGroundAngle = ((!onGround || !(groundAngle < 80f)) ? lastGroundAngle : groundAngle);
		groundAnglesSum -= groundAngles[groundAnglesIdx];
		groundAnglesSum += num;
		groundAngles[groundAnglesIdx] = num;
		groundAnglesIdx = (groundAnglesIdx + 1) % groundAngles.Length;
	}

	private void ProcessFall()
	{
		PushGroundAngle();
		bool flag = false;
		if (groundAnglesSum / (float)groundAngles.Length > 45f)
		{
			flag = true;
			slideTimer = 0f;
			onGround = false;
			state = HumanState.Slide;
		}
		else if (state == HumanState.Slide && groundAnglesSum / (float)groundAngles.Length < 37f)
		{
			Vector3 velocity = ragdoll.partBall.rigidbody.velocity;
			if (velocity.y > -1f)
			{
				slideTimer += Time.fixedDeltaTime;
				if (slideTimer < 0.003f)
				{
					onGround = false;
				}
			}
		}
		if (!onGround && !flag)
		{
			if (fallTimer < 5f)
			{
				fallTimer += Time.deltaTime;
			}
			if (state == HumanState.Climb)
			{
				fallTimer = 0f;
			}
			if (fallTimer > 3f)
			{
				state = HumanState.FreeFall;
			}
			else if (fallTimer > 1f)
			{
				state = HumanState.Fall;
			}
		}
		else
		{
			fallTimer = 0f;
		}
	}

	private void ProcessUnconscious()
	{
		if (state == HumanState.Unconscious)
		{
			unconsciousTime -= Time.fixedDeltaTime;
			if (unconsciousTime <= 0f)
			{
				state = HumanState.Fall;
				wakeUpTime = maxWakeUpTime;
				unconsciousTime = 0f;
			}
		}
		if (wakeUpTime > 0f)
		{
			wakeUpTime -= Time.fixedDeltaTime;
			if (wakeUpTime <= 0f)
			{
				wakeUpTime = 0f;
			}
		}
	}

	public void MakeUnconscious(float time)
	{
		unconsciousTime = time;
		state = HumanState.Unconscious;
	}

	public void MakeUnconscious()
	{
		unconsciousTime = maxUnconsciousTime;
		state = HumanState.Unconscious;
	}

	public void Reset()
	{
		groundManager.Reset();
		grabManager.Reset();
		for (int i = 0; i < groundAngles.Length; i++)
		{
			groundAngles[i] = 0f;
		}
		groundAnglesSum = 0f;
	}

	public void SpawnAt(Vector3 pos)
	{
		state = HumanState.Spawning;
		Vector3 a = KillHorizontalVelocity();
		int num = 2;
		if (Game.currentLevel != null)
		{
			float maxHumanVelocity = Game.currentLevel.MaxHumanVelocity;
			if (a.magnitude > maxHumanVelocity)
			{
				ControlVelocity(maxHumanVelocity, killHorizontal: false);
				a = new Vector3(0f, 0f - maxHumanVelocity, 0f);
			}
		}
		Vector3 position = pos - a * num - Physics.gravity * num * num / 2f;
		SetPosition(position);
		if (a.magnitude < 5f)
		{
			AddRandomTorque(1f);
		}
		Reset();
	}

	public void SpawnAt(Transform spawnPoint, Vector3 offset)
	{
		SpawnAt(offset + spawnPoint.position);
	}

	public Vector3 LimitHorizontalVelocity(float max)
	{
		Rigidbody[] array = rigidbodies;
		Vector3 velocity = this.velocity;
		Vector3 vector = velocity;
		vector.y = 0f;
		if (vector.magnitude < max)
		{
			return velocity;
		}
		vector -= Vector3.ClampMagnitude(vector, max);
		velocity -= vector;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].velocity += -vector;
		}
		return velocity;
	}

	public Vector3 KillHorizontalVelocity()
	{
		Rigidbody[] array = rigidbodies;
		Vector3 velocity = this.velocity;
		Vector3 vector = velocity;
		vector.y = 0f;
		velocity -= vector;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].velocity += -vector;
		}
		return velocity;
	}

	public Vector3 ControlVelocity(float maxVelocity, bool killHorizontal)
	{
		Rigidbody[] array = rigidbodies;
		Vector3 velocity = this.velocity;
		Vector3 b = velocity;
		b.y = 0f;
		Vector3 vector = (!killHorizontal) ? (Vector3.ClampMagnitude(velocity, maxVelocity) - velocity) : (Vector3.ClampMagnitude(velocity - b, maxVelocity) - velocity);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].velocity += vector;
		}
		return velocity;
	}

	public void AddRandomTorque(float multiplier)
	{
		Vector3 torque = UnityEngine.Random.onUnitSphere * 100f * multiplier;
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			Rigidbody body = rigidbodies[i];
			body.SafeAddTorque(torque, ForceMode.VelocityChange);
		}
	}

	private void Start()
	{
		identity = GetComponentInParent<NetIdentity>();
		if (identity != null)
		{
			evtScroll = identity.RegisterEvent(OnScroll);
		}
	}

	private void OnScroll(NetStream stream)
	{
		Vector3 scroll = NetVector3.Read(stream, 12).Dequantize(500f);
		Scroll(scroll);
	}

	public void SetPosition(Vector3 spawnPos)
	{
		if (!NetGame.isClient && !ReplayRecorder.isPlaying)
		{
			Vector3 scroll = spawnPos - base.transform.position;
			Scroll(scroll);
		}
	}

	private void Scroll(Vector3 scroll)
	{
		if (!NetGame.isClient && !ReplayRecorder.isPlaying)
		{
			base.transform.position += scroll;
		}
		if (player.isLocalPlayer)
		{
			CloudSystem.instance.Scroll(scroll);
			player.cameraController.Scroll(scroll);
			for (int i = 0; i < CloudBox.all.Count; i++)
			{
				CloudBox.all[i].FadeIn(1f);
			}
		}
		if (identity != null && (NetGame.isServer || ReplayRecorder.isRecording))
		{
			NetStream stream = identity.BeginEvent(evtScroll);
			NetVector3.Quantize(scroll, 500f, 12).Write(stream);
			identity.EndEvent();
		}
	}

	public void ReleaseGrab(float blockTime = 0f)
	{
		ragdoll.partLeftHand.sensor.ReleaseGrab(blockTime);
		ragdoll.partRightHand.sensor.ReleaseGrab(blockTime);
	}

	public void ReleaseGrab(GameObject item, float blockTime = 0f)
	{
		if (ragdoll.partLeftHand.sensor.IsGrabbed(item))
		{
			ragdoll.partLeftHand.sensor.ReleaseGrab(blockTime);
		}
		if (ragdoll.partRightHand.sensor.IsGrabbed(item))
		{
			ragdoll.partRightHand.sensor.ReleaseGrab(blockTime);
		}
	}

	internal void Show()
	{
		UnityEngine.Object.Destroy(hook);
		SetPosition(new Vector3(0f, 50f, 0f));
	}

	internal void Hide()
	{
		SetPosition(new Vector3(0f, 500f, 0f));
		hook = ragdoll.partHead.rigidbody.gameObject.AddComponent<FixedJoint>();
	}

	public void SetDrag(float drag, bool external = true)
	{
		if (external || !overridenDrag)
		{
			overridenDrag = external;
			for (int i = 0; i < rigidbodies.Length; i++)
			{
				rigidbodies[i].drag = drag;
			}
		}
	}

	public void ResetDrag()
	{
		overridenDrag = false;
		isFallSpeedInitialized = false;
		LimitFallSpeed();
	}

	private void LimitFallSpeed()
	{
		bool flag = Game.instance.state != GameState.PlayingLevel;
		if (isFallSpeedLimited != flag || !isFallSpeedInitialized)
		{
			isFallSpeedInitialized = true;
			isFallSpeedLimited = flag;
			if (flag)
			{
				SetDrag(0.1f, external: false);
			}
			else
			{
				SetDrag(0.05f, external: false);
			}
		}
	}
}
