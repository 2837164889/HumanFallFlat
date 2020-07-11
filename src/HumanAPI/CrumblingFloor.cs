using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class CrumblingFloor : MonoBehaviour, INetBehavior
	{
		[Tooltip("The sound played when the player is stood on this thing")]
		public Sound2 sound2;

		[Tooltip("The time in seconds the player can stand on this thing")]
		public float standTime = 0.2f;

		[Tooltip("How long he player has to recover when standing on this thing ")]
		public float standRecovery = 2f;

		[Tooltip("The length of time in seconds the player can hold this thing before it breaks")]
		public float hangTime = 3f;

		[Tooltip("The length of time in seconds the player can use to recover when hanging onto this thing ")]
		public float hangRecovery = 3f;

		[Tooltip("The amount of force needed to break this thing")]
		public float breakForce = 10000f;

		private float hangHealth = 1f;

		private float standHealth = 1f;

		private bool moved;

		private Vector3 axis;

		private ConfigurableJoint leftJoint;

		private ConfigurableJoint rightJoint;

		private Vector3 startPos;

		private Quaternion startRot;

		public bool showDebug;

		private int brokenCount;

		private void OnEnable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Ran the Enable stuff ");
			}
			if (sound2 == null)
			{
				sound2 = GetComponentInChildren<Sound2>();
			}
			startPos = base.transform.position;
			startRot = base.transform.rotation;
			Vector3 size = GetComponent<BoxCollider>().size;
			if (size.x > size.y && size.x > size.z)
			{
				axis = Vector3.right * size.x;
			}
			else if (size.y > size.z)
			{
				axis = Vector3.up * size.y;
			}
			else
			{
				axis = Vector3.forward * size.z;
			}
			leftJoint = AddJoint(-axis / 3f);
			rightJoint = AddJoint(axis / 3f);
		}

		private ConfigurableJoint AddJoint(Vector3 pos)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Adding joints ");
			}
			ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.anchor = pos;
			configurableJoint.xMotion = ConfigurableJointMotion.Locked;
			configurableJoint.yMotion = ConfigurableJointMotion.Limited;
			configurableJoint.zMotion = ConfigurableJointMotion.Locked;
			configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
			configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
			configurableJoint.linearLimit = new SoftJointLimit
			{
				limit = 0.03f
			};
			configurableJoint.yDrive = new JointDrive
			{
				positionSpring = 2000f,
				maximumForce = 5000f,
				positionDamper = 100f
			};
			configurableJoint.targetPosition = -Vector3.up / 2f;
			configurableJoint.breakForce = breakForce;
			return configurableJoint;
		}

		public void FixedUpdate()
		{
			if (ReplayRecorder.isPlaying || NetGame.isClient || leftJoint == null || rightJoint == null)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				if (human.grabManager.IsGrabbed(base.gameObject))
				{
					flag = true;
				}
				else if (human.groundManager.IsStanding(base.gameObject))
				{
					flag2 = true;
					Vector3 position = human.ragdoll.partBall.transform.position;
					float y = position.y;
					Vector3 position2 = base.transform.position;
					float num = y - position2.y;
					if (num > -0.3f && num < 0.2f)
					{
						return;
					}
				}
			}
			if (flag2)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " We are standing ");
				}
				standHealth -= Time.fixedDeltaTime / standTime;
			}
			else
			{
				standHealth += Time.fixedDeltaTime / standRecovery;
			}
			if (flag)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " We have been grabbed ");
				}
				hangHealth -= Time.fixedDeltaTime / hangTime;
			}
			else
			{
				hangHealth += Time.fixedDeltaTime / hangRecovery;
			}
			standHealth = Mathf.Clamp01(standHealth);
			hangHealth = Mathf.Clamp01(hangHealth);
			if (!moved && (standHealth < 0.5f || hangHealth < 0.5f))
			{
				moved = true;
				Vector3 vector = base.transform.TransformPoint(leftJoint.anchor);
				float y2 = vector.y;
				Vector3 vector2 = base.transform.TransformPoint(rightJoint.anchor);
				if (y2 < vector2.y)
				{
					leftJoint.linearLimit = new SoftJointLimit
					{
						limit = 0.2f
					};
					leftJoint.yDrive = new JointDrive
					{
						positionSpring = 1000f,
						maximumForce = 0f,
						positionDamper = 100f
					};
				}
				else
				{
					rightJoint.linearLimit = new SoftJointLimit
					{
						limit = 0.1f
					};
					rightJoint.yDrive = new JointDrive
					{
						positionSpring = 1000f,
						maximumForce = 0f,
						positionDamper = 100f
					};
				}
				sound2.PlayOneShot(0.5f, 1.2f);
			}
			else if (standHealth == 0f || hangHealth == 0f)
			{
				Object.Destroy(leftJoint);
				Object.Destroy(rightJoint);
				sound2.PlayOneShot();
			}
		}

		public void OnJointBreak(float breakForce)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Joint Break ");
			}
			if (brokenCount == 0)
			{
				sound2.PlayOneShot();
				int num = Random.Range(0, 1);
				if (num == 1)
				{
					Object.Destroy(leftJoint);
				}
				else
				{
					Object.Destroy(rightJoint);
				}
			}
			else if (brokenCount == 1)
			{
				if (leftJoint != null)
				{
					Object.Destroy(leftJoint);
				}
				if (rightJoint != null)
				{
					Object.Destroy(rightJoint);
				}
			}
			brokenCount++;
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset State ");
			}
			hangHealth = (standHealth = 1f);
			Rigidbody component = GetComponent<Rigidbody>();
			Vector3 position = startPos;
			base.transform.position = position;
			component.position = position;
			Quaternion rotation = startRot;
			base.transform.rotation = rotation;
			component.rotation = rotation;
			position = (component.velocity = (component.angularVelocity = Vector3.zero));
			if (leftJoint != null)
			{
				Object.Destroy(leftJoint);
			}
			if (rightJoint != null)
			{
				Object.Destroy(rightJoint);
			}
			leftJoint = AddJoint(-axis / 3f);
			rightJoint = AddJoint(axis / 3f);
		}

		private void ApplyState(bool left, bool right)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply bool state ");
			}
			if (!left && leftJoint != null)
			{
				sound2.PlayOneShot(0.5f, 1.2f);
				Object.Destroy(leftJoint);
			}
			if (!right && rightJoint != null)
			{
				sound2.PlayOneShot(0.5f, 1.2f);
				Object.Destroy(rightJoint);
			}
			if (left && leftJoint == null)
			{
				leftJoint = AddJoint(-axis / 3f);
			}
			if (right && rightJoint == null)
			{
				rightJoint = AddJoint(axis / 3f);
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
				Debug.Log(base.name + " Collected State ");
			}
			NetBoolEncoder.CollectState(stream, leftJoint != null);
			NetBoolEncoder.CollectState(stream, rightJoint != null);
		}

		public void ApplyState(NetStream state)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Applying State ");
			}
			ApplyState(NetBoolEncoder.ApplyState(state), NetBoolEncoder.ApplyState(state));
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply Lerped State ");
			}
			ApplyState(NetBoolEncoder.ApplyLerpedState(state0, state1, mix), NetBoolEncoder.ApplyLerpedState(state0, state1, mix));
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Calc the Delta ");
			}
			NetBoolEncoder.CalculateDelta(state0, state1, delta);
			NetBoolEncoder.CalculateDelta(state0, state1, delta);
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Add the delta  ");
			}
			NetBoolEncoder.AddDelta(state0, delta, result);
			NetBoolEncoder.AddDelta(state0, delta, result);
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Calc max delta size in bits ");
			}
			return 2 * NetBoolEncoder.CalculateMaxDeltaSizeInBits();
		}
	}
}
