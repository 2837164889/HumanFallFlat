using HumanAPI;
using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;
using Voronoi2;

public class VoronoiShatter : ShatterBase
{
	public GameObject shardPrefab;

	public PhysicMaterial physicsMaterial;

	public ShatterAxis thicknessLocalAxis;

	public float densityPerSqMeter;

	public float totalMass = 100f;

	public int shardLayer = 10;

	public Vector3 adjustColliderSize;

	public float minExplodeImpulse;

	public float maxExplodeImpulse = float.PositiveInfinity;

	public float perShardImpulseFraction = 0.25f;

	public float maxShardVelocity = float.PositiveInfinity;

	public float cellInset;

	private List<GameObject> cells = new List<GameObject>();

	private float scale = 1f;

	private Rigidbody body;

	private Material material;

	private NetScope shardParent;

	protected override void OnEnable()
	{
		base.OnEnable();
		body = GetComponent<Rigidbody>();
		material = renderer.sharedMaterial;
		collider = GetComponent<BoxCollider>();
		Vector3 lossyScale = base.transform.lossyScale;
		scale = lossyScale.x;
	}

	private Vector3 To3D(Vector3 v)
	{
		switch (thicknessLocalAxis)
		{
		case ShatterAxis.X:
			return new Vector3(v.z, v.x, v.y) / scale;
		case ShatterAxis.Y:
			return new Vector3(v.y, v.z, v.x) / scale;
		case ShatterAxis.Z:
			return new Vector3(v.x, v.y, v.z) / scale;
		default:
			throw new InvalidOperationException();
		}
	}

	private Vector3 To2D(Vector3 v)
	{
		switch (thicknessLocalAxis)
		{
		case ShatterAxis.X:
			return new Vector3(v.y, v.z, v.x) * scale;
		case ShatterAxis.Y:
			return new Vector3(v.z, v.x, v.y) * scale;
		case ShatterAxis.Z:
			return new Vector3(v.x, v.y, v.z) * scale;
		default:
			throw new InvalidOperationException();
		}
	}

	protected override void Shatter(Vector3 contactPoint, Vector3 adjustedImpulse, float impactMagnitude, uint seed, uint netId)
	{
		base.Shatter(contactPoint, adjustedImpulse, impactMagnitude, seed, netId);
		GameObject gameObject = new GameObject(base.name + "shards");
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		shardParent = gameObject.AddComponent<NetScope>();
		shardParent.AssignNetId(netId);
		shardParent.suppressThrottling = 3f;
		BoxCollider boxCollider = collider as BoxCollider;
		Vector2 b = To2D(base.transform.InverseTransformPoint(contactPoint) - boxCollider.center);
		Vector3 vector = To2D(boxCollider.center);
		Vector3 vector2 = To2D(boxCollider.size + adjustColliderSize);
		float x = vector2.x;
		float y = vector2.y;
		float z = vector2.z;
		float num = (0f - vector2.x) / 2f;
		float num2 = vector2.x / 2f;
		float num3 = (0f - vector2.y) / 2f;
		float num4 = vector2.y / 2f;
		float z2 = (0f - vector2.z) / 2f;
		float z3 = vector2.z / 2f;
		float num5 = x * y;
		if (densityPerSqMeter == 0f)
		{
			densityPerSqMeter = totalMass / num5;
		}
		float d = Mathf.Min(x, y) / 4f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)seed);
		int num6 = (int)Mathf.Clamp(num5 * 10f, 5f, 50f);
		if (NetGame.isNetStarted)
		{
			num6 /= 2;
		}
		int num7 = num6 / 2;
		Voronoi voronoi = new Voronoi(0.1f);
		float[] array = new float[num6];
		float[] array2 = new float[num6];
		for (int i = 0; i < num7; i++)
		{
			array[i] = UnityEngine.Random.Range(num, num2);
			array2[i] = UnityEngine.Random.Range(num3, num4);
		}
		for (int j = num7; j < num6; j++)
		{
			int num8 = 0;
			Vector2 vector3;
			do
			{
				if (num8++ > 1000)
				{
					return;
				}
				vector3 = UnityEngine.Random.insideUnitCircle * d + b;
			}
			while (vector3.x < num || vector3.y < num3 || vector3.x > num2 || vector3.y > num4);
			array[j] = vector3.x;
			array2[j] = vector3.y;
		}
		UnityEngine.Random.state = state;
		List<GraphEdge> list = voronoi.generateVoronoi(array, array2, num, num2, num3, num4);
		List<Vector2>[] array3 = new List<Vector2>[num6];
		for (int k = 0; k < num6; k++)
		{
			array3[k] = new List<Vector2>();
		}
		int count = list.Count;
		for (int l = 0; l < count; l++)
		{
			GraphEdge graphEdge = list[l];
			Vector2 vector4 = new Vector2(graphEdge.x1, graphEdge.y1);
			Vector2 vector5 = new Vector2(graphEdge.x2, graphEdge.y2);
			if (!(vector4 == vector5))
			{
				if (!array3[graphEdge.site1].Contains(vector4))
				{
					array3[graphEdge.site1].Add(vector4);
				}
				if (!array3[graphEdge.site2].Contains(vector4))
				{
					array3[graphEdge.site2].Add(vector4);
				}
				if (!array3[graphEdge.site1].Contains(vector5))
				{
					array3[graphEdge.site1].Add(vector5);
				}
				if (!array3[graphEdge.site2].Contains(vector5))
				{
					array3[graphEdge.site2].Add(vector5);
				}
			}
		}
		float num9 = float.MaxValue;
		int num10 = 0;
		Vector2 vector6 = new Vector2(num, num3);
		float num11 = float.MaxValue;
		int num12 = 0;
		Vector2 vector7 = new Vector2(num, num4);
		float num13 = float.MaxValue;
		int num14 = 0;
		Vector2 vector8 = new Vector2(num2, num3);
		float num15 = float.MaxValue;
		int num16 = 0;
		Vector2 vector9 = new Vector2(num2, num4);
		for (int m = 0; m < num6; m++)
		{
			Vector2 b2 = new Vector2(array[m], array2[m]);
			float sqrMagnitude = (vector6 - b2).sqrMagnitude;
			if (sqrMagnitude < num9)
			{
				num9 = sqrMagnitude;
				num10 = m;
			}
			float sqrMagnitude2 = (vector7 - b2).sqrMagnitude;
			if (sqrMagnitude2 < num11)
			{
				num11 = sqrMagnitude2;
				num12 = m;
			}
			float sqrMagnitude3 = (vector8 - b2).sqrMagnitude;
			if (sqrMagnitude3 < num13)
			{
				num13 = sqrMagnitude3;
				num14 = m;
			}
			float sqrMagnitude4 = (vector9 - b2).sqrMagnitude;
			if (sqrMagnitude4 < num15)
			{
				num15 = sqrMagnitude4;
				num16 = m;
			}
		}
		array3[num10].Add(vector6);
		array3[num12].Add(vector7);
		array3[num14].Add(vector8);
		array3[num16].Add(vector9);
		Vector3 normalized = adjustedImpulse.normalized;
		float value = Mathf.Clamp(adjustedImpulse.magnitude, minExplodeImpulse, maxExplodeImpulse) * perShardImpulseFraction;
		List<Vector2> list2 = new List<Vector2>();
		List<Vector2> list3 = new List<Vector2>();
		List<float> list4 = new List<float>();
		for (int n = 0; n < num6; n++)
		{
			Vector2 vector10 = new Vector2(array[n], array2[n]);
			List<Vector2> list5 = array3[n];
			if (list5.Count < 3)
			{
				continue;
			}
			list4.Clear();
			list3.Clear();
			list2.Clear();
			int count2 = list5.Count;
			Vector2 zero = Vector2.zero;
			for (int num17 = 0; num17 < count2; num17++)
			{
				zero += list5[num17];
			}
			zero /= (float)list5.Count;
			for (int num18 = 0; num18 < count2; num18++)
			{
				Vector2 item = list5[num18] - zero;
				float num19 = Mathf.Atan2(item.x, item.y);
				int num20;
				for (num20 = 0; num20 < list4.Count && num19 < list4[num20]; num20++)
				{
				}
				list3.Insert(num20, item);
				list4.Insert(num20, num19);
			}
			if (cellInset > 0f)
			{
				for (int num21 = 0; num21 < count2; num21++)
				{
					Vector2 b3 = list3[num21];
					Vector2 a = list3[(num21 + count2 - 1) % count2];
					Vector2 b4 = list3[(num21 + count2 - 1) % count2];
					Vector2 normalized2 = (a - b3 + b4 - b3).normalized;
					list2.Add(normalized2);
				}
				for (int num22 = 0; num22 < count2; num22++)
				{
					list3[num22] += list2[num22] * cellInset;
				}
			}
			Vector3[] array4 = new Vector3[count2 * 6];
			int[] array5 = new int[(count2 * 2 + (count2 - 2) * 2) * 3];
			int num23 = count2 * 2 * 3;
			int num24 = num23 + (count2 - 2) * 3;
			Vector3 zero2 = Vector3.zero;
			for (int num25 = 0; num25 < count2; num25++)
			{
				Vector2 vector11 = list3[num25];
				array4[num25 * 6] = (array4[num25 * 6 + 1] = (array4[num25 * 6 + 2] = To3D(new Vector3(vector11.x, vector11.y, z2) - zero2)));
				array4[num25 * 6 + 3] = (array4[num25 * 6 + 4] = (array4[num25 * 6 + 5] = To3D(new Vector3(vector11.x, vector11.y, z3) - zero2)));
				int num26 = (num25 + 1) % count2;
				int num27 = num25 * 6 + 3;
				int num28 = num25 * 6;
				int num29 = num26 * 6 + 4;
				int num30 = num26 * 6 + 1;
				array5[num25 * 6] = num27;
				array5[num25 * 6 + 1] = num28;
				array5[num25 * 6 + 2] = num29;
				array5[num25 * 6 + 3] = num29;
				array5[num25 * 6 + 4] = num28;
				array5[num25 * 6 + 5] = num30;
				if (num25 >= 2)
				{
					array5[num23 + (num25 - 2) * 3] = 2;
					array5[num23 + (num25 - 2) * 3 + 1] = num25 * 6 + 2;
					array5[num23 + (num25 - 2) * 3 + 2] = (num25 - 1) * 6 + 2;
					array5[num24 + (num25 - 2) * 3] = 5;
					array5[num24 + (num25 - 2) * 3 + 1] = (num25 - 1) * 6 + 5;
					array5[num24 + (num25 - 2) * 3 + 2] = num25 * 6 + 5;
				}
			}
			Mesh mesh = new Mesh();
			mesh.name = "cell" + n;
			mesh.vertices = array4;
			mesh.triangles = array5;
			mesh.RecalculateNormals();
			float num31 = 0f;
			for (int num32 = 0; num32 < count2; num32++)
			{
				Vector2 vector12 = list3[num32];
				Vector2 vector13 = list3[(num32 + 1) % count2];
				num31 += vector12.x * vector13.y - vector12.y * vector13.x;
			}
			num31 /= 2f;
			MeshFilter meshFilter = null;
			MeshRenderer x2 = null;
			MeshCollider meshCollider = null;
			CollisionAudioSensor collisionAudioSensor = null;
			GameObject gameObject2;
			if (shardPrefab == null)
			{
				gameObject2 = new GameObject();
				gameObject2.layer = shardLayer;
			}
			else
			{
				gameObject2 = UnityEngine.Object.Instantiate(shardPrefab);
				meshFilter = gameObject2.GetComponent<MeshFilter>();
				x2 = gameObject2.GetComponent<MeshRenderer>();
				meshCollider = gameObject2.GetComponent<MeshCollider>();
				collisionAudioSensor = gameObject2.GetComponent<CollisionAudioSensor>();
			}
			gameObject2.SetActive(value: false);
			gameObject2.name = "cell" + n;
			if (meshFilter == null)
			{
				meshFilter = gameObject2.AddComponent<MeshFilter>();
			}
			meshFilter.mesh = mesh;
			if (x2 == null)
			{
				x2 = gameObject2.AddComponent<MeshRenderer>();
				x2.sharedMaterial = material;
			}
			if (meshCollider == null)
			{
				meshCollider = gameObject2.AddComponent<MeshCollider>();
			}
			Rigidbody rigidbody = gameObject2.AddComponent<Rigidbody>();
			rigidbody.mass = Mathf.Max(4f, ((!NetGame.isNetStarted) ? 1f : 0.6f) * num31 * densityPerSqMeter);
			meshCollider.convex = true;
			meshCollider.sharedMesh = mesh;
			meshCollider.sharedMaterial = physicsMaterial;
			if (collisionAudioSensor == null)
			{
				collisionAudioSensor = gameObject2.AddComponent<CollisionAudioSensor>();
			}
			collisionAudioSensor.pitch = Mathf.Clamp(10f / rigidbody.mass, 0.9f, 1.1f);
			Vector3 vector14 = (vector10 - b) * 10f;
			vector14.z = Mathf.Lerp((vector10 - b).sqrMagnitude, 100f, 0f);
			float d2 = Mathf.Clamp(value, 0f, maxShardVelocity * rigidbody.mass);
			gameObject2.transform.SetParent(shardParent.transform, worldPositionStays: false);
			gameObject2.transform.localPosition = To3D(zero) + boxCollider.center;
			gameObject2.SetActive(value: true);
			gameObject2.GetComponent<Rigidbody>().SafeAddForceAtPosition(-normalized * d2, (3f * contactPoint + To3D(vector10)) / 4f, ForceMode.Impulse);
			NetIdentity netIdentity = gameObject2.AddComponent<NetIdentity>();
			netIdentity.sceneId = (uint)n;
			gameObject2.AddComponent<NetBody>().Start();
			shardParent.AddIdentity(netIdentity);
			cells.Add(gameObject2);
		}
		shardParent.StartNetwork(repopulate: false);
	}

	public override void ResetState(int checkpoint, int subObjectives)
	{
		base.ResetState(checkpoint, subObjectives);
		for (int i = 0; i < cells.Count; i++)
		{
			shardParent.RemoveIdentity(cells[i].GetComponent<NetIdentity>());
			UnityEngine.Object.Destroy(cells[i]);
		}
		cells.Clear();
		if (shardParent != null)
		{
			UnityEngine.Object.Destroy(shardParent.gameObject);
		}
	}
}
