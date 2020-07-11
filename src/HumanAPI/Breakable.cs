using Multiplayer;
using System;
using UnityEngine;

namespace HumanAPI
{
	public class Breakable : Node, INetBehavior, IGrabbable
	{
		[Tooltip("This is the thing that will break")]
		public Rigidbody connectedBody;

		[Tooltip("This is the noise the thing will make when breaking")]
		public Sound2 sound;

		[NonSerialized]
		[Tooltip("This is whether or not the thing has broken")]
		public bool shattered;

		[NonSerialized]
		[Tooltip("This is the joint around which the thing will break")]
		public Joint joint;

		[Tooltip("Use ths in order to print debug info to the Log")]
		public bool showDebug;

		[Tooltip("The object has broken signal")]
		public NodeInput breakSignal;

		[Tooltip("Whether the object is broken")]
		public NodeOutput broken;

		[Tooltip("The threshold past which the signal will be sent ")]
		public float signalTreshold = 0.5f;

		public bool breakOnImpact;

		public float impulseTreshold = 10f;

		public float breakTreshold = 100f;

		public float accumulatedBreakTreshold = 300f;

		public float humanHardness;

		private float accumulatedImpact;

		private Vector3 adjustedImpulse = Vector3.zero;

		private Vector3 lastFrameImpact;

		[Tooltip("Should this break when pulled on by Bob ")]
		public bool breakOnPull;

		private bool grabbed;

		private float usedForceRelief;

		[Tooltip("The fore needed to break this thing by hand ")]
		public float grabbedBreakForce = 1500f;

		[Tooltip("The Torque needed to break this thing by Hand ")]
		public float grabbedBreakTorque = 1500f;

		[Tooltip("Use to Calc the jount break force by hand")]
		public float pullRelief = 1500f;

		[Tooltip("Used to Calc the break force for the thing no matter the interaction ")]
		public float breakForce = float.PositiveInfinity;

		[Tooltip("Used to Calc the Torque needed to break this thing no matter the interaction")]
		public float breakTorque = float.PositiveInfinity;

		[Tooltip("Used to set the direction the lock will break in when needed ")]
		public Vector3 breakNormal = Vector3.forward;

		public Joint CreateJoint()
		{
			FixedJoint fixedJoint = base.gameObject.AddComponent<FixedJoint>();
			fixedJoint.connectedBody = connectedBody;
			fixedJoint.breakForce = breakForce;
			fixedJoint.breakTorque = breakTorque;
			if (showDebug)
			{
				Debug.Log(base.name + " A new joint finalised ");
			}
			return fixedJoint;
		}

		public void Shatter()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Shatter ");
			}
			shattered = true;
			if (sound != null)
			{
				sound.PlayOneShot();
			}
			if (joint != null)
			{
				UnityEngine.Object.Destroy(joint);
				joint = null;
			}
			broken.SetValue(1f);
		}

		protected virtual void FixedUpdate()
		{
			if (!shattered)
			{
				if (breakOnImpact)
				{
					FixedUpdateOnImpact();
				}
				if (breakOnPull)
				{
					FixedUpdateOnPull();
				}
			}
		}

		public override void Process()
		{
			base.Process();
			if (!shattered && breakSignal.value > signalTreshold)
			{
				Shatter();
			}
			if (showDebug)
			{
				Debug.Log(base.name + " breakSignal.value " + breakSignal.value);
			}
			if (showDebug)
			{
				Debug.Log(base.name + " signalTreshold " + signalTreshold);
			}
		}

		public void OnCollisionEnter(Collision collision)
		{
			if (!shattered && breakOnImpact)
			{
				HandleCollision(collision, enter: false);
			}
		}

		public void OnCollisionStay(Collision collision)
		{
			if (!shattered && breakOnImpact)
			{
				HandleCollision(collision, enter: false);
			}
		}

		private void HandleCollision(Collision collision, bool enter)
		{
			if (collision.contacts.Length == 0)
			{
				return;
			}
			Vector3 impulse = collision.GetImpulse();
			float magnitude = impulse.magnitude;
			if (magnitude < impulseTreshold)
			{
				return;
			}
			float num = 1f;
			Transform transform = collision.transform;
			if (transform.name.StartsWith("Lock"))
			{
				return;
			}
			while (transform != null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " T != Null ");
				}
				if ((bool)transform.GetComponent<Human>())
				{
					num = humanHardness;
					break;
				}
				ShatterHardness component = transform.GetComponent<ShatterHardness>();
				if (component != null)
				{
					num = component.hardness;
					break;
				}
				transform = transform.parent;
			}
			adjustedImpulse += impulse * num;
			if (showDebug)
			{
				Debug.Log(base.name + " adjustedImpulse " + adjustedImpulse);
			}
			if (showDebug)
			{
				Debug.Log(base.name + " impulse " + impulse);
			}
			if (showDebug)
			{
				Debug.Log(base.name + " hardnessValue " + num);
			}
		}

		private void FixedUpdateOnImpact()
		{
			accumulatedImpact += adjustedImpulse.magnitude;
			if (accumulatedImpact > accumulatedBreakTreshold)
			{
				shattered = true;
			}
			if ((adjustedImpulse + lastFrameImpact).magnitude > breakTreshold)
			{
				shattered = true;
			}
			if (shattered)
			{
				Shatter();
			}
			lastFrameImpact = adjustedImpulse;
			adjustedImpulse = Vector3.zero;
		}

		private void ResetOnImpact()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset on Impact ");
			}
			accumulatedImpact = 0f;
			lastFrameImpact = (adjustedImpulse = Vector3.zero);
		}

		public void OnGrab()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " On Grab ");
			}
			if (breakOnPull && !grabbed)
			{
				grabbed = true;
				if (joint != null)
				{
					joint.breakForce = grabbedBreakForce;
					joint.breakTorque = grabbedBreakTorque;
				}
			}
		}

		public void OnRelease()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " On Release ");
			}
			if (breakOnPull && grabbed)
			{
				grabbed = false;
				if (joint != null)
				{
					joint.breakForce = breakForce;
					joint.breakTorque = breakTorque;
				}
			}
		}

		public void OnJointBreak(float breakForce)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Joint Break  ");
			}
			Shatter();
		}

		private void FixedUpdateOnPull()
		{
			if (!grabbed || joint == null)
			{
				usedForceRelief = 0f;
				return;
			}
			float num = 0f;
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				num += GetForceRelief(human, human.ragdoll.partLeftHand) + GetForceRelief(human, human.ragdoll.partRightHand);
			}
			usedForceRelief = Mathf.Min(num, Mathf.MoveTowards(usedForceRelief, num, num * Time.fixedDeltaTime));
			joint.breakForce = Mathf.Max(0f, grabbedBreakForce - usedForceRelief * pullRelief);
		}

		private float GetForceRelief(Human human, HumanSegment hand)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Get Force Relief ");
			}
			if (!hand.sensor.IsGrabbed(base.gameObject))
			{
				return 0f;
			}
			Vector3 normalized = base.transform.TransformVector(breakNormal).normalized;
			float b = Mathf.InverseLerp(0.5f, 0.9f, Vector3.Dot(normalized, human.controls.walkDirection));
			return Mathf.Max(0f, b);
		}

		private void ResetOnPull()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset on Pull ");
			}
			usedForceRelief = 0f;
			joint.breakForce = breakForce;
			joint.breakTorque = breakTorque;
		}

		private void OnDrawGizmos()
		{
			if (breakOnPull)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawRay(base.transform.position, base.transform.TransformVector(breakNormal).normalized);
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
				Debug.Log(base.name + " CollectState ");
			}
			NetBoolEncoder.CollectState(stream, shattered);
		}

		private void ApplyShatter(bool shatter)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply Shatter ");
			}
			if (shattered != shatter)
			{
				if (shatter)
				{
					Shatter();
				}
				else
				{
					ResetState(0, 0);
				}
			}
		}

		public virtual void ResetState(int checkpoint, int subObjectives)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset the state ");
			}
			shattered = false;
			broken.SetValue(0f);
			GetComponent<NetBody>().ResetState(checkpoint, subObjectives);
			if (connectedBody != null)
			{
				connectedBody.GetComponent<NetBody>().ResetState(checkpoint, subObjectives);
			}
			if (joint == null)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " A new joint is being created ");
				}
				joint = CreateJoint();
			}
			if (breakOnImpact)
			{
				ResetOnImpact();
			}
			if (breakOnPull)
			{
				ResetOnPull();
			}
		}

		public void ApplyState(NetStream state)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply State ");
			}
			ApplyShatter(NetBoolEncoder.ApplyState(state));
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Lerped State ");
			}
			ApplyShatter(NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Calc Delta ");
			}
			NetBoolEncoder.CalculateDelta(state0, state1, delta);
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Add Delta ");
			}
			NetBoolEncoder.AddDelta(state0, delta, result);
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Max Delta size ");
			}
			return NetBoolEncoder.CalculateMaxDeltaSizeInBits();
		}
	}
}
