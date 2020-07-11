using Multiplayer;
using System;
using UnityEngine;

namespace HumanAPI
{
	[RequireComponent(typeof(NetIdentity))]
	public class Conveyor : Node, INetBehavior
	{
		private abstract class Part
		{
			protected readonly Conveyor belt;

			[Tooltip("Reference location for a rigid body")]
			public readonly Rigidbody body;

			[Tooltip("Refence location for a Transform")]
			public readonly Transform transform;

			[Tooltip("Location on which to bind to the conveyor when on the low side")]
			private readonly float lowBound;

			[Tooltip("Location on which to binf to the conveyor when on the high side")]
			private readonly float upBound;

			public Part(Conveyor belt, Rigidbody body, float lowBound, float upBound)
			{
				this.belt = belt;
				this.body = body;
				this.lowBound = lowBound;
				this.upBound = upBound;
				transform = body.GetComponent<Transform>();
			}

			public bool IsOutside(Vector3 localPos, bool checkLowerBound)
			{
				float num = ToOffset(localPos);
				if (checkLowerBound)
				{
					return num < lowBound;
				}
				return num > upBound;
			}

			protected abstract float ToOffset(Vector3 localPos);

			public abstract Vector3 Move(Vector3 localPos, float delta);
		}

		private class LinearPart : Part
		{
			public LinearPart(Conveyor belt, Rigidbody body, float lowBound, float upBound)
				: base(belt, body, lowBound, upBound)
			{
			}

			protected override float ToOffset(Vector3 localPos)
			{
				return Vector3.Dot(localPos, belt.forward);
			}

			public override Vector3 Move(Vector3 localPos, float delta)
			{
				return localPos + belt.forward * delta;
			}
		}

		private class RadialPart : Part
		{
			public RadialPart(Conveyor belt, Rigidbody body, float lowBound, float upBound)
				: base(belt, body, lowBound, upBound)
			{
			}

			protected override float ToOffset(Vector3 localPos)
			{
				float num = 90f - Math2d.SignedAngle(-belt.forward, new Vector2(Vector3.Dot(localPos, belt.forward), Vector3.Dot(localPos, belt.up)));
				return num / 180f * belt.radialLength;
			}

			public override Vector3 Move(Vector3 localPos, float delta)
			{
				return Quaternion.AngleAxis(delta / belt.radialLength * 180f, belt.right) * localPos;
			}
		}

		[Tooltip("Value coming into the node ")]
		public NodeInput input;

		[Tooltip("Forward direction")]
		public Vector3 forward = Vector3.forward;

		[Tooltip("Right Direction")]
		public Vector3 right = Vector3.right;

		[Tooltip("The up direction")]
		public Vector3 up;

		private Vector3 axis;

		private Vector3 axisForward;

		[Tooltip("The object the conveyor will be using")]
		public GameObject itemPrefab;

		[Tooltip("(Normally we use the mesh of the prefab for collision. Tick this to enable an override collider)")]
		public bool useOverrideCollider;

		[Tooltip("Override for collision (Tick above box to enable)")]
		public GameObject colliderPrefab;

		[Tooltip("The number of segments within the conveyors total run")]
		public int segmentCount = 10;

		private float segmentSpacing;

		[Tooltip("The distance along which the conveyor travels , top and bottom")]
		public float length = 5f;

		[Tooltip("The size of the rounded section at the ends of the conveyor")]
		public float radius = 0.5f;

		[Tooltip("The speed at which the conveyor should change over time")]
		public float speed = 1f;

		private float radialLength;

		private float totalLength;

		[Tooltip("Can the player grab the top of the conveyor?")]
		public bool topGrabbable = true;

		[Tooltip("Can the player grab the start of the conveyor?")]
		public bool startGrabbable = true;

		[Tooltip("Can the player grab the bottom of the conveyor?")]
		public bool bottomGrabbable = true;

		[Tooltip("Can the player grab the end of the conveyor?")]
		public bool endGrabbable = true;

		private Mesh linearLong;

		private Mesh linearShort;

		private Mesh radialShort;

		private Mesh radialLong;

		private GameObject linearCollidersTop;

		private GameObject linearCollidersBottom;

		private GameObject radialCollidersStart;

		private GameObject radialCollidersEnd;

		private GameObject[] collidersTop;

		private GameObject[] collidersBottom;

		private GameObject[] collidersStart;

		private GameObject[] collidersEnd;

		private bool visible;

		public Rigidbody topTrack;

		public Rigidbody bottomTrack;

		public Rigidbody startTrack;

		public Rigidbody endTrack;

		private float frac;

		private Part topPart;

		private Part bottomPart;

		private Part startPart;

		private Part endPart;

		private bool linearIsLong = true;

		private float currentOffset;

		private NetFloat encoder;

		protected override void OnEnable()
		{
			base.OnEnable();
			up = Vector3.Cross(forward, right);
			axis = base.transform.TransformDirection(right);
			axisForward = base.transform.TransformDirection(forward);
			radialLength = (float)Math.PI * radius;
			totalLength = 2f * (length + radialLength);
			segmentSpacing = totalLength / (float)segmentCount;
			encoder = new NetFloat(segmentSpacing, 8, 3, 5);
			InitializeArrays();
			itemPrefab.SetActive(value: false);
			if (colliderPrefab != null)
			{
				colliderPrefab.SetActive(value: false);
			}
		}

		public void OnBecameVisible()
		{
			visible = true;
		}

		public void OnBecameInvisible()
		{
			visible = false;
		}

		private void FixedUpdate()
		{
			if (!topGrabbable && GrabManager.IsGrabbedAny(topTrack.gameObject))
			{
				GrabManager.Release(topTrack.gameObject, 1f);
			}
			if (!bottomGrabbable && GrabManager.IsGrabbedAny(bottomTrack.gameObject))
			{
				GrabManager.Release(bottomTrack.gameObject, 1f);
			}
			if (!startGrabbable && GrabManager.IsGrabbedAny(startTrack.gameObject))
			{
				GrabManager.Release(startTrack.gameObject, 1f);
			}
			if (!endGrabbable && GrabManager.IsGrabbedAny(endTrack.gameObject))
			{
				GrabManager.Release(endTrack.gameObject, 1f);
			}
			if (!ReplayRecorder.isPlaying && !NetGame.isClient && input.value != 0f && !(topTrack == null))
			{
				AdvanceArrays(input.value * speed * Time.fixedDeltaTime);
			}
		}

		private void InitializeArrays()
		{
			topTrack = CreateBody("top");
			bottomTrack = CreateBody("bottom");
			startTrack = CreateBody("start");
			endTrack = CreateBody("end");
			Mesh sharedMesh = itemPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
			Material sharedMaterial = itemPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;
			int num = Mathf.FloorToInt(length / segmentSpacing) + 1;
			int num2 = segmentCount / 2 - num;
			frac = segmentSpacing * (float)num - length;
			linearShort = CreateMeshArray(sharedMesh, num - 1, Matrix4x4.TRS(up * radius, Quaternion.identity, Vector3.one), Matrix4x4.TRS(forward * segmentSpacing, Quaternion.identity, Vector3.one));
			linearLong = CreateMeshArray(sharedMesh, num, Matrix4x4.TRS(up * radius, Quaternion.identity, Vector3.one), Matrix4x4.TRS(forward * segmentSpacing, Quaternion.identity, Vector3.one));
			radialShort = CreateMeshArray(sharedMesh, num2, Matrix4x4.TRS(up * radius, Quaternion.identity, Vector3.one), Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(segmentSpacing / radialLength * 180f, right), Vector3.one));
			radialLong = CreateMeshArray(sharedMesh, num2 + 1, Matrix4x4.TRS(up * radius, Quaternion.identity, Vector3.one), Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(segmentSpacing / radialLength * 180f, right), Vector3.one));
			topTrack.transform.localPosition = Vector3.zero;
			topTrack.transform.localRotation = Quaternion.identity;
			if (!useOverrideCollider)
			{
				topTrack.gameObject.AddComponent<MeshCollider>().sharedMesh = linearLong;
			}
			topTrack.gameObject.AddComponent<MeshFilter>().sharedMesh = linearLong;
			topTrack.gameObject.AddComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
			topTrack.gameObject.GetComponent<MeshRenderer>().probeAnchor = base.transform;
			if (!topGrabbable)
			{
				topTrack.gameObject.tag = "NoGrab";
				topTrack.gameObject.layer = 29;
			}
			endTrack.transform.localPosition = forward * length;
			endTrack.transform.localRotation = Quaternion.AngleAxis(frac / radialLength * 180f, right);
			if (!useOverrideCollider)
			{
				endTrack.gameObject.AddComponent<MeshCollider>().sharedMesh = radialShort;
			}
			endTrack.gameObject.AddComponent<MeshFilter>().sharedMesh = radialShort;
			endTrack.gameObject.AddComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
			endTrack.gameObject.GetComponent<MeshRenderer>().probeAnchor = base.transform;
			if (!endGrabbable)
			{
				endTrack.gameObject.tag = "NoGrab";
				endTrack.gameObject.layer = 29;
			}
			bottomTrack.transform.localPosition = forward * length;
			bottomTrack.transform.localRotation = Quaternion.AngleAxis(180f, right);
			if (!useOverrideCollider)
			{
				bottomTrack.gameObject.AddComponent<MeshCollider>().sharedMesh = linearLong;
			}
			bottomTrack.gameObject.AddComponent<MeshFilter>().sharedMesh = linearLong;
			bottomTrack.gameObject.AddComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
			bottomTrack.gameObject.GetComponent<MeshRenderer>().probeAnchor = base.transform;
			if (!bottomGrabbable)
			{
				bottomTrack.gameObject.tag = "NoGrab";
				bottomTrack.gameObject.layer = 29;
			}
			startTrack.transform.localPosition = Vector3.zero;
			startTrack.transform.localRotation = Quaternion.AngleAxis(frac / radialLength * 180f + 180f, right);
			if (!useOverrideCollider)
			{
				startTrack.gameObject.AddComponent<MeshCollider>().sharedMesh = radialShort;
			}
			startTrack.gameObject.AddComponent<MeshFilter>().sharedMesh = radialShort;
			startTrack.gameObject.AddComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
			startTrack.gameObject.GetComponent<MeshRenderer>().probeAnchor = base.transform;
			if (!startGrabbable)
			{
				startTrack.gameObject.tag = "NoGrab";
				startTrack.gameObject.layer = 29;
			}
			topPart = new LinearPart(this, topTrack, 0f - segmentSpacing / 2f, length + segmentSpacing / 2f);
			bottomPart = new LinearPart(this, bottomTrack, 0f - segmentSpacing / 2f, length + segmentSpacing / 2f);
			endPart = new RadialPart(this, endTrack, 0f - segmentSpacing / 2f, radialLength + segmentSpacing / 2f);
			startPart = new RadialPart(this, startTrack, 0f - segmentSpacing / 2f, radialLength + segmentSpacing / 2f);
			if (useOverrideCollider)
			{
				(linearCollidersTop = new GameObject()).transform.parent = topTrack.transform;
				linearCollidersTop.name = "Top";
				(linearCollidersBottom = new GameObject()).transform.parent = bottomTrack.transform;
				linearCollidersBottom.name = "Bottom";
				(radialCollidersStart = new GameObject()).transform.parent = startTrack.transform;
				radialCollidersStart.name = "Front";
				(radialCollidersEnd = new GameObject()).transform.parent = endTrack.transform;
				radialCollidersEnd.name = "Back";
				Transform transform = linearCollidersTop.transform;
				Vector3 zero = Vector3.zero;
				radialCollidersEnd.transform.localPosition = zero;
				zero = zero;
				radialCollidersStart.transform.localPosition = zero;
				zero = zero;
				linearCollidersBottom.transform.localPosition = zero;
				transform.localPosition = zero;
				Transform transform2 = linearCollidersTop.transform;
				Quaternion identity = Quaternion.identity;
				radialCollidersEnd.transform.localRotation = identity;
				identity = identity;
				radialCollidersStart.transform.localRotation = identity;
				identity = identity;
				linearCollidersBottom.transform.localRotation = identity;
				transform2.localRotation = identity;
				collidersTop = CreateColliderArray(colliderPrefab, linearCollidersTop.transform, num, up * radius, forward * segmentSpacing, 0f);
				collidersBottom = CreateColliderArray(colliderPrefab, linearCollidersBottom.transform, num, up * radius, forward * segmentSpacing, 0f);
				collidersStart = CreateColliderArray(colliderPrefab, radialCollidersStart.transform, num2 + 1, up * radius, Vector3.zero, segmentSpacing / radialLength * 180f);
				collidersEnd = CreateColliderArray(colliderPrefab, radialCollidersEnd.transform, num2 + 1, up * radius, Vector3.zero, segmentSpacing / radialLength * 180f);
			}
		}

		private Rigidbody CreateBody(string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = 1000f;
			rigidbody.isKinematic = true;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			return rigidbody;
		}

		private void AdvanceArrays(float delta)
		{
			float b = float.MaxValue;
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				b = Mathf.Min((human.ragdoll.transform.position - base.transform.position).magnitude, b);
				DebugGrabs(human.ragdoll.partLeftHand.sensor.grabJoint);
				DebugGrabs(human.ragdoll.partRightHand.sensor.grabJoint);
			}
			Wrap(delta);
			if (ReplayRecorder.isPlaying || NetGame.isClient)
			{
				topTrack.transform.position = base.transform.position + axisForward * currentOffset;
				bottomTrack.transform.position = base.transform.position + axisForward * (length - currentOffset);
			}
			else
			{
				topTrack.MovePosition(base.transform.position + axisForward * currentOffset);
				bottomTrack.MovePosition(base.transform.position + axisForward * (length - currentOffset));
			}
			float num = currentOffset + frac;
			if (!linearIsLong)
			{
				num -= segmentSpacing;
			}
			if (ReplayRecorder.isPlaying || NetGame.isClient)
			{
				startTrack.transform.rotation = base.transform.rotation * Quaternion.AngleAxis(num / radialLength * 180f + 180f, right);
				endTrack.transform.rotation = base.transform.rotation * Quaternion.AngleAxis(num / radialLength * 180f, right);
			}
			else
			{
				startTrack.MoveRotation(base.transform.rotation * Quaternion.AngleAxis(num / radialLength * 180f + 180f, right));
				endTrack.MoveRotation(base.transform.rotation * Quaternion.AngleAxis(num / radialLength * 180f, right));
			}
		}

		private void DebugGrabs(ConfigurableJoint joint)
		{
			DebugGrabs(joint, topTrack, Color.red);
			DebugGrabs(joint, startTrack, Color.green);
			DebugGrabs(joint, endTrack, Color.black);
			DebugGrabs(joint, bottomTrack, Color.blue);
		}

		private void DebugGrabs(ConfigurableJoint joint, Rigidbody body, Color color)
		{
			if (joint != null && joint.connectedBody == body)
			{
				Vector3 a = body.transform.TransformPoint(joint.connectedAnchor);
				Debug.DrawLine(a - Vector3.up, a + Vector3.up, color);
				Debug.DrawLine(a - Vector3.right, a + Vector3.right, color);
				Debug.DrawLine(a - Vector3.forward, a + Vector3.forward, color);
			}
		}

		private void Wrap(float delta)
		{
			bool flag = false;
			currentOffset += delta;
			if (linearIsLong)
			{
				if (currentOffset < 0f)
				{
					float num = delta - currentOffset;
					float post = 0f - segmentSpacing - num;
					if (!useOverrideCollider)
					{
						WrapParts(topPart, startPart, num, post, lowBound: true);
						WrapParts(bottomPart, endPart, num, post, lowBound: true);
					}
					currentOffset += segmentSpacing;
					topTrack.position += axisForward * segmentSpacing;
					bottomTrack.position -= axisForward * segmentSpacing;
				}
				else
				{
					if (!(currentOffset > segmentSpacing - frac))
					{
						return;
					}
					float num2 = delta - (currentOffset - (segmentSpacing - frac));
					float post2 = segmentSpacing - num2;
					if (!useOverrideCollider)
					{
						WrapParts(startPart, topPart, num2, post2, lowBound: false);
						WrapParts(endPart, bottomPart, num2, post2, lowBound: false);
					}
					startTrack.rotation = Quaternion.AngleAxis((0f - segmentSpacing) / radialLength * 180f, axis) * startTrack.rotation;
					endTrack.rotation = Quaternion.AngleAxis((0f - segmentSpacing) / radialLength * 180f, axis) * endTrack.rotation;
				}
				linearIsLong = false;
			}
			else
			{
				if (currentOffset > segmentSpacing)
				{
					float num3 = delta - (currentOffset - segmentSpacing);
					float post3 = segmentSpacing - num3;
					if (!useOverrideCollider)
					{
						WrapParts(topPart, endPart, num3, post3, lowBound: false);
						WrapParts(bottomPart, startPart, num3, post3, lowBound: false);
					}
					currentOffset -= segmentSpacing;
					topTrack.position -= axisForward * segmentSpacing;
					bottomTrack.position += axisForward * segmentSpacing;
				}
				else
				{
					if (!(currentOffset < segmentSpacing - frac))
					{
						return;
					}
					float num4 = delta - (currentOffset - (segmentSpacing - frac));
					float post4 = 0f - segmentSpacing - num4;
					if (!useOverrideCollider)
					{
						WrapParts(startPart, bottomPart, num4, post4, lowBound: true);
						WrapParts(endPart, topPart, num4, post4, lowBound: true);
					}
					startTrack.rotation = Quaternion.AngleAxis(segmentSpacing / radialLength * 180f, axis) * startTrack.rotation;
					endTrack.rotation = Quaternion.AngleAxis(segmentSpacing / radialLength * 180f, axis) * endTrack.rotation;
				}
				linearIsLong = true;
				flag = true;
			}
			if (useOverrideCollider)
			{
				if (flag)
				{
					WrapColliders(-axisForward, endTrack, collidersEnd, bottomTrack, collidersBottom[0]);
					WrapColliders(axisForward, startTrack, collidersStart, topTrack, collidersTop[0]);
					WrapColliders(-axisForward, bottomTrack, collidersBottom, startTrack, collidersStart[0]);
					WrapColliders(axisForward, topTrack, collidersTop, endTrack, collidersEnd[0]);
				}
				MeshFilter component = topTrack.gameObject.GetComponent<MeshFilter>();
				Mesh sharedMesh = (!linearIsLong) ? linearShort : linearLong;
				bottomTrack.gameObject.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				component.sharedMesh = sharedMesh;
				MeshFilter component2 = startTrack.gameObject.GetComponent<MeshFilter>();
				sharedMesh = ((!linearIsLong) ? radialLong : radialShort);
				endTrack.gameObject.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				component2.sharedMesh = sharedMesh;
				collidersTop[collidersTop.Length - 1].SetActive(linearIsLong);
				collidersBottom[collidersBottom.Length - 1].SetActive(linearIsLong);
				collidersStart[collidersStart.Length - 1].SetActive(!linearIsLong);
				collidersEnd[collidersEnd.Length - 1].SetActive(!linearIsLong);
			}
			else
			{
				MeshCollider component3 = topTrack.gameObject.GetComponent<MeshCollider>();
				Mesh sharedMesh = (!linearIsLong) ? linearShort : linearLong;
				bottomTrack.gameObject.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				sharedMesh = sharedMesh;
				bottomTrack.gameObject.GetComponent<MeshCollider>().sharedMesh = sharedMesh;
				sharedMesh = sharedMesh;
				topTrack.gameObject.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				component3.sharedMesh = sharedMesh;
				MeshCollider component4 = startTrack.gameObject.GetComponent<MeshCollider>();
				sharedMesh = ((!linearIsLong) ? radialLong : radialShort);
				endTrack.gameObject.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				sharedMesh = sharedMesh;
				endTrack.gameObject.GetComponent<MeshCollider>().sharedMesh = sharedMesh;
				sharedMesh = sharedMesh;
				startTrack.gameObject.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
				component4.sharedMesh = sharedMesh;
			}
		}

		private void WrapParts(Part current, Part next, float pre, float post, bool lowBound)
		{
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				WrapParts(human.ragdoll.partLeftHand.sensor.grabJoint, current, next, pre, post, lowBound);
				WrapParts(human.ragdoll.partRightHand.sensor.grabJoint, current, next, pre, post, lowBound);
			}
		}

		private void WrapParts(ConfigurableJoint joint, Part current, Part next, float pre, float post, bool lowBound)
		{
			if (!(joint == null) && !(joint.connectedBody != current.body))
			{
				joint.autoConfigureConnectedAnchor = false;
				Vector3 connectedAnchor = joint.connectedAnchor;
				Vector3 vector = current.Move(connectedAnchor, pre + post);
				if (current.IsOutside(vector, lowBound))
				{
					vector = current.Move(connectedAnchor, pre);
					vector = next.transform.InverseTransformPoint(current.transform.TransformPoint(vector));
					vector = next.Move(vector, 0f - pre);
					joint.connectedBody = next.body;
					joint.connectedAnchor = vector;
				}
				else
				{
					joint.connectedAnchor = vector;
				}
			}
		}

		private void WrapColliders(Vector3 forward, Rigidbody body, GameObject[] objects, Rigidbody nextBody, GameObject nextObject)
		{
			for (int i = 0; i < Human.all.Count; i++)
			{
				Human human = Human.all[i];
				WrapColliders(human.ragdoll.partLeftHand.sensor, forward, body, objects, nextBody, nextObject);
				WrapColliders(human.ragdoll.partRightHand.sensor, forward, body, objects, nextBody, nextObject);
			}
		}

		private void WrapColliders(CollisionSensor sensor, Vector3 forward, Rigidbody body, GameObject[] objects, Rigidbody nextBody, GameObject nextObject)
		{
			ConfigurableJoint grabJoint = sensor.grabJoint;
			Vector3 zero = Vector3.zero;
			if (grabJoint != null)
			{
				zero = grabJoint.connectedAnchor;
			}
			int num = 0;
			while (true)
			{
				if (num < objects.Length)
				{
					if (sensor.grabObject == objects[num])
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (num < objects.Length - 2)
			{
				sensor.ReleaseGrab();
			}
			else
			{
				sensor.ReleaseGrab();
			}
		}

		private Mesh CreateMeshArray(Mesh source, int count, Matrix4x4 initial, Matrix4x4 delta)
		{
			Vector3[] vertices = source.vertices;
			Vector2[] uv = source.uv;
			Vector3[] normals = source.normals;
			int[] triangles = source.triangles;
			Vector3[] array = new Vector3[vertices.Length * count];
			Vector2[] array2 = new Vector2[uv.Length * count];
			Vector3[] array3 = new Vector3[normals.Length * count];
			int[] array4 = new int[triangles.Length * count];
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < triangles.Length; j++)
				{
					array4[num2] = triangles[j] + num;
					num2++;
				}
				for (int k = 0; k < vertices.Length; k++)
				{
					array[num] = initial.MultiplyPoint3x4(vertices[k]);
					array3[num] = initial.MultiplyVector(normals[k]);
					array2[num] = uv[k];
					num++;
				}
				initial = delta * initial;
			}
			Mesh mesh = new Mesh();
			mesh.name = source.name + "(Array)";
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.normals = array3;
			mesh.triangles = array4;
			mesh.RecalculateBounds();
			return mesh;
		}

		private GameObject[] CreateColliderArray(GameObject source, Transform parent, int count, Vector3 initialPos, Vector3 deltaPos, float deltaRot)
		{
			GameObject[] array = new GameObject[count];
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(source, parent);
				gameObject.transform.localPosition = initialPos + deltaPos * i;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.RotateAround(parent.position, right, deltaRot * (float)i);
				gameObject.name = parent.gameObject.name + " " + i;
				array[i] = gameObject;
			}
			return array;
		}

		public void StartNetwork(NetIdentity identity)
		{
		}

		public void CollectState(NetStream stream)
		{
			encoder.CollectState(stream, currentOffset);
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			Apply(encoder.ApplyLerpedState(state0, state1, mix));
		}

		public void ApplyState(NetStream state)
		{
			Apply(encoder.ApplyState(state));
		}

		private void Apply(float newOffset)
		{
			AdvanceArrays(newOffset - currentOffset);
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			encoder.CalculateDelta(state0, state1, delta);
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			encoder.AddDelta(state0, delta, result);
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			return encoder.CalculateMaxDeltaSizeInBits();
		}

		public void SetMaster(bool isMaster)
		{
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
		}
	}
}
