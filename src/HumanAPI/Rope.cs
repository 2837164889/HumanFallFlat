using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class Rope : RopeRender, INetBehavior, IRespawnable
	{
		[Tooltip("A list of points to use in connection with the rope")]
		public Transform[] handles;

		[Tooltip("Whether or not the start  of the rope should be a fixed location")]
		public bool fixStart;

		[Tooltip("Whether or not hte end of the rope should be a fixed location")]
		public bool fixEnd;

		[Tooltip("The start joint is locked")]
		public bool fixStartDir;

		[Tooltip("The end joint is fixed")]
		public bool fixEndDir;

		[Tooltip("Start location for the Rope via a connected Rigid body")]
		public Rigidbody startBody;

		[Tooltip("End location for the Rope via a connected Rigid body")]
		public Rigidbody endBody;

		[Tooltip("How much to divide up the rope")]
		public int rigidSegments = 10;

		[Tooltip("Use_this_in_order_to_print_debug_info_to_the_Log")]
		public float segmentMass = 20f;

		[Tooltip("Use_this_in_order_to_print_debug_info_to_the_Log")]
		public float lengthMultiplier = 1f;

		[Tooltip("Use_this_in_order_to_print_debug_info_to_the_Log")]
		public PhysicMaterial ropeMaterial;

		protected Transform[] bones;

		protected NetBodySleep[] boneSleep;

		[Tooltip("Use_this_in_order_to_print_debug_info_to_the_Log")]
		public Vector3[] bonePos;

		[Tooltip("Use_this_in_order_to_print_debug_info_to_the_Log")]
		public Vector3[] boneRot;

		[Tooltip("Print stuff from this script to the Log")]
		public bool showDebug;

		private bool initialized;

		private Vector3[] boneForward;

		private Vector3[] boneRight;

		private Vector3[] boneUp;

		protected Vector3[] originalPositions;

		protected float boneLen;

		private NetStream originalState;

		private NetVector3Encoder posEncoder = new NetVector3Encoder(500f, 18, 4, 8);

		private NetVector3Encoder diffEncoder = new NetVector3Encoder(3.90625f, 11, 3, 7);

		private NetQuaternionEncoder rotEncoder = new NetQuaternionEncoder(9, 4, 6);

		public virtual Vector3[] GetHandlePositions()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Get Handle positions ");
			}
			Vector3[] array = new Vector3[handles.Length];
			for (int i = 0; i < handles.Length; i++)
			{
				array[i] = handles[i].position;
			}
			return array;
		}

		public override void OnEnable()
		{
			EnsureInitialized();
		}

		private void EnsureInitialized()
		{
			if (!initialized)
			{
				initialized = true;
				Initialize();
			}
		}

		private void Initialize()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " OnEnable ");
			}
			if (GetComponent<NetIdentity>() == null)
			{
				Debug.LogError("Missing NetIdentity", this);
			}
			Vector3[] handlePositions = GetHandlePositions();
			if (handlePositions.Length < 2)
			{
				return;
			}
			Vector3 vector = handlePositions[0];
			Vector3 vector2 = handlePositions[handlePositions.Length - 1];
			float num = 0f;
			for (int i = 0; i < handlePositions.Length - 1; i++)
			{
				num += (handlePositions[i] - handlePositions[i + 1]).magnitude;
			}
			num *= lengthMultiplier;
			boneLen = num / (float)rigidSegments;
			bones = new Transform[rigidSegments];
			boneSleep = new NetBodySleep[rigidSegments];
			boneRight = new Vector3[rigidSegments];
			boneUp = new Vector3[rigidSegments];
			originalPositions = new Vector3[bones.Length];
			boneForward = new Vector3[rigidSegments];
			Vector3 a = (vector2 - vector) / rigidSegments;
			Quaternion rotation = Quaternion.LookRotation(a.normalized);
			Vector3 zero = Vector3.zero;
			for (int j = 0; j < rigidSegments; j++)
			{
				zero = vector + a * (0.5f + (float)j);
				GameObject gameObject = base.gameObject;
				gameObject = new GameObject("bone" + j);
				gameObject.transform.SetParent(base.transform, worldPositionStays: true);
				originalPositions[j] = zero;
				gameObject.transform.position = zero;
				gameObject.transform.rotation = rotation;
				bones[j] = gameObject.transform;
				gameObject.tag = "Target";
				gameObject.layer = 14;
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = segmentMass;
				rigidbody.drag = 0.1f;
				rigidbody.angularDrag = 0.1f;
				CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.direction = 2;
				capsuleCollider.radius = radius;
				capsuleCollider.height = boneLen + radius * 2f;
				capsuleCollider.sharedMaterial = ropeMaterial;
				if (j != 0)
				{
					ConfigurableJoint configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
					configurableJoint.connectedBody = bones[j - 1].GetComponent<Rigidbody>();
					ConfigurableJointMotion configurableJointMotion2 = configurableJoint.zMotion = ConfigurableJointMotion.Locked;
					configurableJointMotion2 = (configurableJoint.xMotion = (configurableJoint.yMotion = configurableJointMotion2));
					configurableJointMotion2 = (configurableJoint.angularZMotion = ConfigurableJointMotion.Limited);
					configurableJointMotion2 = (configurableJoint.angularXMotion = (configurableJoint.angularYMotion = configurableJointMotion2));
					configurableJoint.angularXLimitSpring = new SoftJointLimitSpring
					{
						spring = 100f,
						damper = 10f
					};
					configurableJoint.angularYZLimitSpring = new SoftJointLimitSpring
					{
						spring = 100f,
						damper = 10f
					};
					configurableJoint.lowAngularXLimit = new SoftJointLimit
					{
						limit = -20f
					};
					configurableJoint.highAngularXLimit = new SoftJointLimit
					{
						limit = 20f
					};
					configurableJoint.angularYLimit = new SoftJointLimit
					{
						limit = 20f
					};
					configurableJoint.angularZLimit = new SoftJointLimit
					{
						limit = 20f
					};
					configurableJoint.angularXDrive = new JointDrive
					{
						positionSpring = 50f
					};
					configurableJoint.angularYZDrive = new JointDrive
					{
						positionSpring = 50f
					};
					configurableJoint.axis = new Vector3(0f, 0f, 1f);
					configurableJoint.secondaryAxis = new Vector3(1f, 0f, 0f);
					configurableJoint.autoConfigureConnectedAnchor = false;
					configurableJoint.anchor = new Vector3(0f, 0f, (0f - boneLen) / 2f);
					configurableJoint.connectedAnchor = new Vector3(0f, 0f, boneLen / 2f);
					configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
					configurableJoint.projectionDistance = 0.02f;
				}
				boneSleep[j] = new NetBodySleep(rigidbody);
			}
			float num2 = (0f - boneLen) / 2f / lengthMultiplier;
			int num3 = -1;
			zero = Vector3.zero;
			Vector3 vector3 = Vector3.zero;
			for (int k = 0; k < rigidSegments; k++)
			{
				Vector3 vector4;
				for (; num2 <= 0f; num2 += vector4.magnitude)
				{
					num3++;
					vector4 = handlePositions[num3 + 1] - handlePositions[num3];
					vector3 = vector4.normalized;
					rotation = Quaternion.LookRotation(vector3);
					zero = handlePositions[num3] - num2 * vector3;
				}
				bones[k].transform.position = zero;
				bones[k].transform.rotation = rotation;
				zero += vector3 * boneLen / lengthMultiplier;
				num2 -= boneLen / lengthMultiplier;
			}
			if (fixStart && bones != null && bones.Length > 0)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Fix Start ");
				}
				ConfigurableJoint configurableJoint2 = bones[0].gameObject.AddComponent<ConfigurableJoint>();
				ConfigurableJointMotion configurableJointMotion2 = configurableJoint2.zMotion = ConfigurableJointMotion.Locked;
				configurableJointMotion2 = (configurableJoint2.xMotion = (configurableJoint2.yMotion = configurableJointMotion2));
				configurableJoint2.projectionMode = JointProjectionMode.PositionAndRotation;
				configurableJoint2.projectionDistance = 0.02f;
				if (fixStartDir)
				{
					configurableJointMotion2 = (configurableJoint2.angularZMotion = ConfigurableJointMotion.Locked);
					configurableJointMotion2 = (configurableJoint2.angularXMotion = (configurableJoint2.angularYMotion = configurableJointMotion2));
				}
				configurableJoint2.anchor = new Vector3(0f, 0f, (0f - boneLen) / 2f);
				configurableJoint2.autoConfigureConnectedAnchor = false;
				if (startBody != null)
				{
					configurableJoint2.connectedBody = startBody;
					configurableJoint2.connectedAnchor = startBody.transform.InverseTransformPoint(vector);
				}
				else
				{
					configurableJoint2.connectedAnchor = vector;
				}
			}
			if (fixEnd)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Fix End ");
				}
				ConfigurableJoint configurableJoint3 = bones[bones.Length - 1].gameObject.AddComponent<ConfigurableJoint>();
				ConfigurableJointMotion configurableJointMotion2 = configurableJoint3.zMotion = ConfigurableJointMotion.Locked;
				configurableJointMotion2 = (configurableJoint3.xMotion = (configurableJoint3.yMotion = configurableJointMotion2));
				configurableJoint3.projectionMode = JointProjectionMode.PositionAndRotation;
				configurableJoint3.projectionDistance = 0.02f;
				if (fixEndDir)
				{
					configurableJointMotion2 = (configurableJoint3.angularZMotion = ConfigurableJointMotion.Locked);
					configurableJointMotion2 = (configurableJoint3.angularXMotion = (configurableJoint3.angularYMotion = configurableJointMotion2));
				}
				configurableJoint3.anchor = new Vector3(0f, 0f, boneLen / 2f);
				configurableJoint3.autoConfigureConnectedAnchor = false;
				if (endBody != null)
				{
					configurableJoint3.connectedBody = endBody;
					configurableJoint3.connectedAnchor = endBody.transform.InverseTransformPoint(vector2);
				}
				else
				{
					configurableJoint3.connectedAnchor = vector2;
				}
			}
			if (bonePos == null || bonePos.Length != rigidSegments)
			{
				bonePos = new Vector3[rigidSegments];
				boneRot = new Vector3[rigidSegments];
			}
			else
			{
				for (int l = 0; l < rigidSegments; l++)
				{
					bones[l].transform.position = base.transform.TransformPoint(bonePos[l]);
					bones[l].transform.rotation = base.transform.rotation * Quaternion.Euler(boneRot[l]);
				}
			}
			base.OnEnable();
			originalState = NetStream.AllocStream();
			CollectState(originalState);
		}

		private void OnDestroy()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " OnDestroy ");
			}
			if (originalState != null)
			{
				originalState = originalState.Release();
			}
		}

		public override void CheckDirty()
		{
			base.CheckDirty();
			EnsureInitialized();
			Rigidbody component = bones[0].GetComponent<Rigidbody>();
			if (!component.IsSleeping() && !component.isKinematic)
			{
				isDirty = true;
			}
		}

		public override void ReadData()
		{
			for (int i = 0; i < rigidSegments; i++)
			{
				Quaternion lhs = Quaternion.Inverse(base.transform.rotation);
				bonePos[i] = base.transform.InverseTransformPoint(bones[i].position);
				Quaternion rotation = lhs * bones[i].rotation;
				boneRot[i] = rotation.eulerAngles;
				boneForward[i] = rotation * Vector3.forward;
				boneUp[i] = rotation * Vector3.up;
				boneRight[i] = rotation * Vector3.right;
			}
		}

		public override void GetPoint(float dist, out Vector3 pos, out Vector3 normal, out Vector3 binormal)
		{
			int num = Mathf.FloorToInt(dist * (float)rigidSegments - 0.5f);
			float num2 = dist * (float)rigidSegments - 0.5f - (float)num;
			int num3 = Mathf.Clamp(num, 0, rigidSegments - 1);
			int num4 = Mathf.Clamp(num + 1, 0, rigidSegments - 1);
			if (num == -1)
			{
				normal = boneRight[num3];
				binormal = boneUp[num3];
				pos = bonePos[num3] + boneForward[num3] * boneLen * (num2 - 1f);
				return;
			}
			if (num == rigidSegments - 1)
			{
				normal = boneRight[num3];
				binormal = boneUp[num3];
				pos = bonePos[num3] + boneForward[num3] * boneLen * num2;
				return;
			}
			float num5 = num2 * num2;
			float num6 = num5 * num2;
			float d = 2f * num6 - 3f * num5 + 1f;
			float d2 = num6 - 2f * num5 + num2;
			float d3 = -2f * num6 + 3f * num5;
			float d4 = num6 - num5;
			Vector3 a = boneForward[num3] * boneLen;
			Vector3 a2 = boneForward[num4] * boneLen;
			pos = d * bonePos[num3] + d2 * a + d3 * bonePos[num4] + d4 * a2;
			normal = d * boneRight[num3] + d3 * boneRight[num4];
			binormal = d * boneUp[num3] + d3 * boneUp[num4];
		}

		public void StartNetwork(NetIdentity identity)
		{
		}

		public virtual void ResetState(int checkpoint, int subObjectives)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset State ");
			}
			EnsureInitialized();
			if (originalState != null)
			{
				originalState.Seek(0);
				ApplyState(originalState);
				for (int i = 0; i < rigidSegments; i++)
				{
					Rigidbody component = bones[i].GetComponent<Rigidbody>();
					Vector3 vector2 = component.velocity = (component.angularVelocity = Vector3.zero);
				}
			}
			isDirty = true;
		}

		public void SetMaster(bool isMaster)
		{
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Calc Max Delta in Bits  ");
			}
			return posEncoder.CalculateMaxDeltaSizeInBits() + rigidSegments * diffEncoder.CalculateMaxDeltaSizeInBits();
		}

		private void FixedUpdate()
		{
			EnsureInitialized();
			if (boneSleep != null)
			{
				for (int i = 0; i < rigidSegments; i++)
				{
					boneSleep[i].HandleSleep();
				}
			}
		}

		public void CollectState(NetStream stream)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Collect State ");
			}
			EnsureInitialized();
			Vector3 value = bones[0].TransformPoint(-Vector3.forward * boneLen / 2f);
			posEncoder.CollectState(stream, value);
			value = posEncoder.Dequantize(posEncoder.Quantize(value));
			for (int i = 0; i < rigidSegments; i++)
			{
				Transform transform = bones[i];
				Vector3 a = transform.TransformPoint(Vector3.forward * boneLen / 2f);
				Vector3 value2 = a - value;
				diffEncoder.CollectState(stream, value2);
				value += diffEncoder.Dequantize(diffEncoder.Quantize(value2));
			}
		}

		public virtual void ApplyState(NetStream state)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply State ");
			}
			EnsureInitialized();
			Vector3 vector = posEncoder.ApplyState(state);
			for (int i = 0; i < rigidSegments; i++)
			{
				Transform transform = bones[i];
				Vector3 b = diffEncoder.ApplyState(state);
				Vector3 vector2 = vector + b;
				Vector3 vector3 = (vector + vector2) / 2f;
				Quaternion rotation = (i != 0) ? Quaternion.LookRotation(vector2 - vector, bones[i - 1].up) : Quaternion.LookRotation(vector2 - vector);
				if (transform.position != vector3)
				{
					transform.position = vector3;
					isDirty = true;
				}
				if (transform.rotation.eulerAngles != rotation.eulerAngles)
				{
					transform.rotation = rotation;
					isDirty = true;
				}
				vector = vector2;
			}
		}

		public virtual void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply Lerped State ");
			}
			EnsureInitialized();
			Vector3 vector = posEncoder.ApplyLerpedState(state0, state1, mix);
			for (int i = 0; i < rigidSegments; i++)
			{
				Transform transform = bones[i];
				Vector3 b = diffEncoder.ApplyLerpedState(state0, state1, mix);
				Vector3 vector2 = vector + b;
				Vector3 vector3 = (vector + vector2) / 2f;
				Quaternion rotation = (i != 0) ? Quaternion.LookRotation(vector2 - vector, bones[i - 1].up) : Quaternion.LookRotation(vector2 - vector);
				if (transform.position != vector3)
				{
					transform.position = vector3;
					isDirty = true;
				}
				if (transform.rotation.eulerAngles != rotation.eulerAngles)
				{
					transform.rotation = rotation;
					isDirty = true;
				}
				vector = vector2;
			}
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Calculate Delta ");
			}
			posEncoder.CalculateDelta(state0, state1, delta);
			for (int i = 0; i < rigidSegments; i++)
			{
				diffEncoder.CalculateDelta(state0, state1, delta);
			}
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Add Delta ");
			}
			posEncoder.AddDelta(state0, delta, result);
			for (int i = 0; i < rigidSegments; i++)
			{
				diffEncoder.AddDelta(state0, delta, result);
			}
		}

		public void Respawn(Vector3 offset)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Respawn ");
			}
			ResetState(0, 0);
			for (int i = 0; i < rigidSegments; i++)
			{
				bones[i].position += offset;
			}
			ForceUpdate();
		}
	}
}
