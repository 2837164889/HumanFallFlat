using System.Collections.Generic;
using UnityEngine;

public class RigVolumeMesh : MonoBehaviour
{
	public struct Face
	{
		public Vector3 n;

		public float d;
	}

	public List<Face> faces = new List<Face>();

	public void Build(float normalSign)
	{
		faces.Clear();
		MeshFilter component = GetComponent<MeshFilter>();
		if (component == null)
		{
			return;
		}
		Mesh sharedMesh = component.sharedMesh;
		if (sharedMesh == null)
		{
			return;
		}
		int[] triangles = sharedMesh.triangles;
		Vector3[] vertices = sharedMesh.vertices;
		Vector3[] normals = sharedMesh.normals;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector3 vector = base.transform.TransformDirection(normals[triangles[i]]) * normalSign;
			float num = Vector3.Dot(vector, base.transform.TransformPoint(vertices[triangles[i]]));
			bool flag = false;
			for (int j = 0; j < faces.Count; j++)
			{
				Face face = faces[j];
				if (Mathf.Abs((face.n - vector).magnitude) < 0.001f)
				{
					Face face2 = faces[j];
					if (Mathf.Abs(face2.d - num) < 0.001f)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				faces.Add(new Face
				{
					n = vector,
					d = num
				});
			}
		}
	}

	public float GetDistInside(Vector3 pos)
	{
		float num = float.MaxValue;
		for (int i = 0; i < faces.Count; i++)
		{
			Face face = faces[i];
			float num2 = Vector3.Dot(face.n, pos) - face.d;
			if (num2 > 0f)
			{
				return 0f;
			}
			if (0f - num2 < num)
			{
				num = 0f - num2;
			}
		}
		return num;
	}

	public float GetDistOutside(Vector3 pos)
	{
		int num = -1;
		float num2 = 0f;
		for (int i = 0; i < faces.Count; i++)
		{
			Face face = faces[i];
			float num3 = Vector3.Dot(face.n, pos) - face.d;
			if (num3 > num2)
			{
				num2 = num3;
				num = i;
			}
		}
		if (num2 == 0f)
		{
			return 0f;
		}
		Face face2 = faces[num];
		Vector3 b = pos - face2.n * num2;
		for (int j = 0; j < faces.Count; j++)
		{
			if (j != num)
			{
				Face face3 = faces[j];
				float num4 = Vector3.Dot(face3.n, pos) - face3.d;
				if (!(num4 <= 0f))
				{
					Vector3 vector = face3.n - face2.n * Vector3.Dot(face3.n, face2.n);
					float d = Vector3.Dot(vector, face3.n);
					b += vector * num4 / d;
				}
			}
		}
		return (pos - b).magnitude;
	}
}
