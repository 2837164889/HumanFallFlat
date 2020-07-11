using System;
using System.Collections.Generic;
using UnityEngine;

public static class BuildPaddedMesh
{
	private struct Edge
	{
		public int v1;

		public int v2;

		public int prev;

		public int next;

		public Vector2 normal;
	}

	private static List<int> openEdges;

	private static Vector3[] verts;

	private static Vector2[] uvs;

	private static Edge[] edges;

	public static Mesh GeneratePadded(Mesh source, float padding)
	{
		verts = source.vertices;
		uvs = source.uv;
		int[] triangles = source.triangles;
		edges = new Edge[triangles.Length];
		openEdges = new List<int>();
		for (int i = 0; i < triangles.Length; i += 3)
		{
			AddEdge(triangles[i], triangles[i + 1], i, i + 2, i + 1);
			AddEdge(triangles[i + 1], triangles[i + 2], i + 1, i, i + 2);
			AddEdge(triangles[i + 2], triangles[i], i + 2, i + 1, i);
		}
		for (int j = 0; j < openEdges.Count; j++)
		{
			for (int k = j + 1; k < openEdges.Count; k++)
			{
				Edge edge = edges[openEdges[j]];
				Edge edge2 = edges[openEdges[k]];
				if (Match(edge.v1, edge2.v2) && Match(edge.v2, edge2.v1))
				{
					edges[edge.next].prev = edge2.prev;
					edges[edge.prev].next = edge2.next;
					edges[edge2.next].prev = edge.prev;
					edges[edge2.prev].next = edge.next;
					openEdges.RemoveAt(k);
					openEdges.RemoveAt(j);
					j--;
					break;
				}
			}
		}
		for (int l = 0; l < openEdges.Count; l++)
		{
			Edge edge3 = edges[openEdges[l]];
			Vector2 normalized = (uvs[edge3.v1] - uvs[edge3.v2]).RotateCW90().normalized;
			edges[openEdges[l]].normal = normalized;
		}
		Vector3[] array = new Vector3[verts.Length + openEdges.Count];
		Vector2[] array2 = new Vector2[uvs.Length + openEdges.Count];
		int[] array3 = new int[triangles.Length + openEdges.Count * 6];
		Array.Copy(verts, array, verts.Length);
		Array.Copy(uvs, array2, uvs.Length);
		Array.Copy(triangles, array3, triangles.Length);
		for (int m = 0; m < openEdges.Count; m++)
		{
			Edge edge4 = edges[openEdges[m]];
			int num = verts.Length + m;
			int num2 = verts.Length + openEdges.IndexOf(edge4.next);
			array[num] = verts[edge4.v1];
			array2[num] = uvs[edge4.v1] + (edge4.normal + edges[edge4.next].normal).normalized * padding;
			array3[triangles.Length + m * 6] = edge4.v2;
			array3[triangles.Length + m * 6 + 1] = edge4.v1;
			array3[triangles.Length + m * 6 + 2] = num;
			array3[triangles.Length + m * 6 + 3] = num;
			array3[triangles.Length + m * 6 + 4] = num2;
			array3[triangles.Length + m * 6 + 5] = edge4.v2;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		return mesh;
	}

	private static void AddEdge(int v1, int v2, int current, int prev, int next)
	{
		openEdges.Add(current);
		edges[current] = new Edge
		{
			v1 = v1,
			v2 = v2,
			prev = prev,
			next = next
		};
	}

	private static bool Match(int v1, int v2)
	{
		try
		{
			return uvs[v1] == uvs[v2] && verts[v1] == verts[v2];
		}
		catch
		{
			return true;
		}
	}
}
