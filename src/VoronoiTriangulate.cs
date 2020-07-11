using System;
using System.Collections.Generic;
using UnityEngine;
using Voronoi2;

public class VoronoiTriangulate : MonoBehaviour, IReset
{
	public ShatterAxis thicknessLocalAxis;

	public Vector3[] vertices;

	private Vector3[] vertexBackup;

	public int[] triangles;

	public float depth = 0.05f;

	private BoxCollider collider;

	public int seed = 56;

	private float scale = 1f;

	private Mesh mesh;

	public bool regenerate;

	private void OnEnable()
	{
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

	public void Deform(Vector3 worldPos, Vector3 impact)
	{
		Vector3 b = base.transform.InverseTransformPoint(worldPos);
		Vector3 a = base.transform.InverseTransformVector(-impact);
		for (int i = 0; i < vertices.Length; i++)
		{
			float num = depth * Mathf.InverseLerp(1f, 0f, (vertices[i] - b).sqrMagnitude);
			if (num > 0f)
			{
				vertices[i] += a * num;
			}
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}

	private void Start()
	{
		if (vertices == null || vertices.Length == 0)
		{
			Generate();
		}
		mesh = new Mesh();
		mesh.name = "impactlayer";
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		vertexBackup = new Vector3[vertices.Length];
		vertices.CopyTo(vertexBackup, 0);
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}

	private void Update()
	{
		if (regenerate)
		{
			regenerate = false;
			Generate();
			mesh.vertices = vertices;
			mesh.RecalculateNormals();
			GetComponent<MeshFilter>().sharedMesh = mesh;
		}
	}

	private void Generate()
	{
		Vector3 vector = To2D(collider.center);
		Vector3 vector2 = To2D(collider.size);
		float x = vector2.x;
		float y = vector2.y;
		float z = vector2.z;
		float num = (0f - vector2.x) / 2f;
		float num2 = vector2.x / 2f;
		float num3 = (0f - vector2.y) / 2f;
		float num4 = vector2.y / 2f;
		float z2 = (0f - vector2.z) / 2f;
		float z3 = vector2.z / 2f;
		UnityEngine.Random.InitState(seed);
		int num5 = 100;
		Voronoi voronoi = new Voronoi(0.1f);
		float[] array = new float[num5];
		float[] array2 = new float[num5];
		for (int i = 0; i < num5; i++)
		{
			array[i] = UnityEngine.Random.Range(num, num2);
			array2[i] = UnityEngine.Random.Range(num3, num4);
		}
		List<GraphEdge> list = voronoi.generateVoronoi(array, array2, num, num2, num3, num4);
		List<Vector2>[] array3 = new List<Vector2>[num5];
		for (int j = 0; j < num5; j++)
		{
			array3[j] = new List<Vector2>();
		}
		int count = list.Count;
		for (int k = 0; k < count; k++)
		{
			GraphEdge graphEdge = list[k];
			Vector2 vector3 = new Vector2(graphEdge.x1, graphEdge.y1);
			Vector2 vector4 = new Vector2(graphEdge.x2, graphEdge.y2);
			if (!(vector3 == vector4))
			{
				if (!array3[graphEdge.site1].Contains(vector3))
				{
					array3[graphEdge.site1].Add(vector3);
				}
				if (!array3[graphEdge.site2].Contains(vector3))
				{
					array3[graphEdge.site2].Add(vector3);
				}
				if (!array3[graphEdge.site1].Contains(vector4))
				{
					array3[graphEdge.site1].Add(vector4);
				}
				if (!array3[graphEdge.site2].Contains(vector4))
				{
					array3[graphEdge.site2].Add(vector4);
				}
			}
		}
		float num6 = float.MaxValue;
		int num7 = 0;
		Vector2 vector5 = new Vector2(num, num3);
		float num8 = float.MaxValue;
		int num9 = 0;
		Vector2 vector6 = new Vector2(num, num4);
		float num10 = float.MaxValue;
		int num11 = 0;
		Vector2 vector7 = new Vector2(num2, num3);
		float num12 = float.MaxValue;
		int num13 = 0;
		Vector2 vector8 = new Vector2(num2, num4);
		for (int l = 0; l < num5; l++)
		{
			Vector2 b = new Vector2(array[l], array2[l]);
			float sqrMagnitude = (vector5 - b).sqrMagnitude;
			if (sqrMagnitude < num6)
			{
				num6 = sqrMagnitude;
				num7 = l;
			}
			float sqrMagnitude2 = (vector6 - b).sqrMagnitude;
			if (sqrMagnitude2 < num8)
			{
				num8 = sqrMagnitude2;
				num9 = l;
			}
			float sqrMagnitude3 = (vector7 - b).sqrMagnitude;
			if (sqrMagnitude3 < num10)
			{
				num10 = sqrMagnitude3;
				num11 = l;
			}
			float sqrMagnitude4 = (vector8 - b).sqrMagnitude;
			if (sqrMagnitude4 < num12)
			{
				num12 = sqrMagnitude4;
				num13 = l;
			}
		}
		array3[num7].Add(vector5);
		array3[num9].Add(vector6);
		array3[num11].Add(vector7);
		array3[num13].Add(vector8);
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		List<Vector3> list4 = new List<Vector3>();
		List<Vector2> list5 = new List<Vector2>();
		List<Vector2> list6 = new List<Vector2>();
		List<float> list7 = new List<float>();
		for (int m = 0; m < num5; m++)
		{
			Vector2 vector9 = new Vector2(array[m], array2[m]);
			List<Vector2> list8 = array3[m];
			if (list8.Count < 3)
			{
				continue;
			}
			list7.Clear();
			list6.Clear();
			list5.Clear();
			int count2 = list8.Count;
			Vector2 zero = Vector2.zero;
			for (int n = 0; n < count2; n++)
			{
				zero += list8[n];
			}
			zero /= (float)list8.Count;
			for (int num14 = 0; num14 < count2; num14++)
			{
				Vector2 vector10 = list8[num14] - zero;
				float num15 = Mathf.Atan2(vector10.x, vector10.y);
				int num16;
				for (num16 = 0; num16 < list7.Count && num15 < list7[num16]; num16++)
				{
				}
				list6.Insert(num16, list8[num14]);
				list7.Insert(num16, num15);
			}
			int count3 = list2.Count;
			bool flag = false;
			for (int num17 = 0; num17 < count2; num17++)
			{
				Vector2 vector11 = list6[num17];
				list2.Add(To3D(new Vector3(vector11.x, vector11.y, z3)) + collider.center);
				list2.Add(To3D(new Vector3(vector11.x, vector11.y, z2)) + collider.center);
				if (num17 >= 2)
				{
					list3.Add(count3);
					list3.Add(list2.Count - 4);
					list3.Add(list2.Count - 2);
					list3.Add(list2.Count - 2 + 1);
					list3.Add(list2.Count - 4 + 1);
					list3.Add(count3 + 1);
				}
				bool flag2 = Mathf.Abs(vector11.x - num) < 0.01f || Mathf.Abs(vector11.x - num2) < 0.01f || Mathf.Abs(vector11.y - num3) < 0.01f || Mathf.Abs(vector11.y - num4) < 0.01f;
				if (flag2 && flag)
				{
					list4.Add(list2[list2.Count - 4]);
					list4.Add(list2[list2.Count - 4 + 1]);
					list4.Add(list2[list2.Count - 2]);
					list4.Add(list2[list2.Count - 2 + 1]);
				}
				flag = flag2;
			}
			for (int num18 = 0; num18 < list4.Count; num18 += 4)
			{
				list3.Add(list2.Count);
				list3.Add(list2.Count + 1);
				list3.Add(list2.Count + 2);
				list3.Add(list2.Count + 2);
				list3.Add(list2.Count + 1);
				list3.Add(list2.Count + 3);
				list2.Add(list4[num18]);
				list2.Add(list4[num18 + 1]);
				list2.Add(list4[num18 + 2]);
				list2.Add(list4[num18 + 3]);
			}
		}
		vertices = list2.ToArray();
		triangles = list3.ToArray();
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		vertexBackup.CopyTo(vertices, 0);
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}
}
