using HumanAPI;
using Multiplayer;
using System.Collections;
using UnityEngine;

public class MechScriptCatapult1 : MonoBehaviour, INetBehavior
{
	[Tooltip("Reference to the joint Ratchet of this gameobject")]
	public JointScriptRatchet1 ratchet;

	[Tooltip("Reference to the Angular jount used as the Arm in this gameobject")]
	public AngularJoint arm;

	[Tooltip("Reference to the Angular Joint being used as the release mech for this gameobject")]
	public AngularJoint release;

	[Tooltip("Refence to the Audio source to use as the release sound")]
	public AudioSource releaseSound;

	[Tooltip("Reference to the Catapult Rope being used as the rope in this gameobject")]
	public CatapultRope rope;

	private bool firing;

	private bool hasHuman;

	[Tooltip("The angle the catapult will return to once fired")]
	public float catapultAngle;

	[Tooltip("How fast to move the Arm of the catapult to the firing angle initially")]
	public float initialAcceleration = 90f;

	[Tooltip("Max amount of acceleration for the catapult arm")]
	public float accelerationAcceleration = 270f;

	private float fireTime;

	private float fireStart;

	private float releaseArmIn;

	private float initialMass;

	private float armMass;

	private float currentAudioClipDuration;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void Awake()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Awake ");
		}
		initialMass = GetComponent<Rigidbody>().mass;
		armMass = arm.body.GetComponent<Rigidbody>().mass;
	}

	private void LateUpdate()
	{
		rope.catapultWindCount = ratchet.GetValue() / 360f + 1.5f;
	}

	public void FixedUpdate()
	{
		if (ReplayRecorder.isPlaying || NetGame.isClient)
		{
			return;
		}
		bool flag = GrabManager.IsGrabbedAny(release.body.gameObject);
		bool flag2 = GrabManager.IsGrabbedAny(ratchet.body.gameObject);
		if (!firing && flag && !flag2 && release.GetValue() > release.centerValue && catapultAngle < arm.maxValue - 10f)
		{
			hasHuman = false;
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				bool flag3 = human.groundManager.IsStanding(arm.body.gameObject);
				bool flag4 = human.grabManager.IsGrabbed(arm.body.gameObject);
				if (flag3 || flag4)
				{
					human.ReleaseGrab(release.body.gameObject);
					hasHuman = true;
					human.ragdoll.ToggleHeavyArms(human.ragdoll.partLeftHand.sensor.grabBody == arm.body.GetComponent<Rigidbody>(), human.ragdoll.partRightHand.sensor.grabBody == arm.body.GetComponent<Rigidbody>());
				}
				if (flag3)
				{
					StatsAndAchievements.UnlockAchievement(Achievement.ACH_SIEGE_HUMAN_CANNON);
				}
			}
			firing = true;
			fireTime = 0f;
			fireStart = catapultAngle;
			if (releaseSound != null)
			{
				releaseSound.Play();
			}
			currentAudioClipDuration = releaseSound.clip.length;
			currentAudioClipDuration -= 0.5f;
			StartCoroutine(Wait());
			ratchet.release = true;
			release.SetTarget(release.maxValue);
		}
		if (firing && catapultAngle == arm.maxValue)
		{
			firing = false;
			arm.anchor.GetComponent<Rigidbody>().isKinematic = false;
			ratchet.release = false;
			release.SetTarget(release.minValue);
			releaseArmIn = 0.12f;
		}
		if (releaseArmIn > 0f)
		{
			releaseArmIn -= Time.fixedDeltaTime;
			if (releaseArmIn <= 0f)
			{
				for (int j = 0; j < Human.all.Count; j++)
				{
					Human human2 = Human.all[j];
					human2.ReleaseGrab(arm.body.gameObject, 0.1f);
					human2.ragdoll.ReleaseHeavyArms();
				}
			}
		}
		if (firing)
		{
			fireTime += Time.fixedDeltaTime;
			catapultAngle = Mathf.Clamp(fireStart + initialAcceleration * fireTime * fireTime / 2f + accelerationAcceleration * fireTime * fireTime * fireTime / 3f, arm.minValue, arm.maxValue);
			arm.SetTarget(catapultAngle);
			ratchet.SetValue(Mathf.Lerp(ratchet.minValue, ratchet.maxValue, Mathf.InverseLerp(arm.maxValue, arm.minValue, catapultAngle)));
		}
		else
		{
			catapultAngle = Mathf.Lerp(arm.maxValue, arm.minValue, Mathf.InverseLerp(ratchet.minValue, ratchet.maxValue, ratchet.GetValue()));
			arm.SetTarget(catapultAngle);
		}
		float num = 1f;
		if (firing)
		{
			num = ((!hasHuman) ? 1.5f : 2f);
		}
		else if (GrabManager.IsGrabbedAny(base.gameObject) && !flag && !flag2)
		{
			num = 0.2f;
		}
		Rigidbody component = GetComponent<Rigidbody>();
		Rigidbody component2 = arm.body.GetComponent<Rigidbody>();
		if (component.mass != num * initialMass)
		{
			component.mass = num * initialMass;
			component2.mass = num * armMass;
		}
	}

	public void StartNetwork(NetIdentity identity)
	{
	}

	public void SetMaster(bool isMaster)
	{
	}

	public void CollectState(NetStream stream)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Collect State ");
		}
		NetSignal.encoder.CollectState(stream, Mathf.InverseLerp(ratchet.minValue, ratchet.maxValue, ratchet.GetValue()));
		NetBoolEncoder.CollectState(stream, firing);
	}

	private void Apply(float ratchetPhase, bool firing)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Apply Ratchet Phase");
		}
		catapultAngle = Mathf.Lerp(arm.maxValue, arm.minValue, ratchetPhase);
		if (firing != this.firing)
		{
			this.firing = firing;
			ratchet.release = firing;
			if (firing)
			{
				release.SetTarget(release.maxValue);
				if (catapultAngle < arm.maxValue - 10f && releaseSound != null)
				{
					releaseSound.Play();
				}
			}
			else
			{
				ResetState(0, 0);
				release.SetTarget(release.maxValue);
			}
		}
		fireStart = catapultAngle;
		fireTime = 0f;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Reset State ");
		}
		firing = false;
	}

	public void ApplyState(NetStream state)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Apply State");
		}
		Apply(NetSignal.encoder.ApplyState(state), NetBoolEncoder.ApplyState(state));
	}

	public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Lerped State ");
		}
		Apply(NetSignal.encoder.ApplyLerpedState(state0, state1, mix), NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
	}

	public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Calculate Delta ");
		}
		NetSignal.encoder.CalculateDelta(state0, state1, delta);
		NetBoolEncoder.CalculateDelta(state0, state1, delta);
	}

	public void AddDelta(NetStream state0, NetStream delta, NetStream result)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Add Delta ");
		}
		NetSignal.encoder.AddDelta(state0, delta, result);
		NetBoolEncoder.AddDelta(state0, delta, result);
	}

	public int CalculateMaxDeltaSizeInBits()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Calculate Delta Max in Bits ");
		}
		return NetSignal.encoder.CalculateMaxDeltaSizeInBits() + NetBoolEncoder.CalculateMaxDeltaSizeInBits();
	}

	public IEnumerator Wait()
	{
		if (showDebug)
		{
			Debug.Log(base.name + "Waiting for the sound to finish ");
		}
		yield return new WaitForSecondsRealtime(currentAudioClipDuration);
		SignalScriptNode1 dustParticles = GetComponent<SignalScriptNode1>();
		if (dustParticles != null)
		{
			dustParticles.SendSignal(1f);
		}
	}
}
