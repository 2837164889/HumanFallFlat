using Multiplayer;
using System;
using UnityEngine;

namespace HumanAPI
{
	public class RopeRender : MonoBehaviour
	{
		public int meshSegments = 20;

		public int segmentsAround = 6;

		public float radius = 0.1f;

		private Vector2[] rotatedRadius;

		private Mesh mesh;

		private MeshFilter filter;

		private Vector3[] meshVerts;

		private bool forceUpdate;

		public bool visible;

		public bool isDirty;

		public virtual void OnEnable()
		{
			int num = meshSegments + 1;
			meshVerts = new Vector3[num * segmentsAround + 2 * segmentsAround];
			int[] array = new int[meshSegments * segmentsAround * 6 + 6 * (segmentsAround - 2)];
			int num2 = 0;
			num2 = 0;
			for (int i = 0; i < num - 1; i++)
			{
				for (int j = 0; j < segmentsAround; j++)
				{
					int num3 = i * segmentsAround;
					int num4 = num3 + segmentsAround;
					int num5 = (j + 1) % segmentsAround;
					int num6 = num3 + j;
					int num7 = num3 + num5;
					int num8 = num4 + j;
					int num9 = num4 + num5;
					array[num2++] = num6;
					array[num2++] = num7;
					array[num2++] = num8;
					array[num2++] = num8;
					array[num2++] = num7;
					array[num2++] = num9;
				}
			}
			int num10 = num * segmentsAround;
			for (int k = 0; k < segmentsAround - 2; k++)
			{
				array[num2++] = num10;
				array[num2++] = num10 + k + 2;
				array[num2++] = num10 + k + 1;
			}
			int num11 = (num + 1) * segmentsAround;
			for (int l = 0; l < segmentsAround - 2; l++)
			{
				array[num2++] = num11;
				array[num2++] = num11 + l + 1;
				array[num2++] = num11 + l + 2;
			}
			mesh = new Mesh();
			mesh.name = "rope " + base.name;
			mesh.vertices = meshVerts;
			mesh.triangles = array;
			filter = GetComponent<MeshFilter>();
			rotatedRadius = new Vector2[segmentsAround];
			for (int m = 0; m < segmentsAround; m++)
			{
				rotatedRadius[m] = new Vector2(radius, 0f).Rotate((float)Math.PI * 2f * (float)m / (float)segmentsAround);
			}
		}

		protected virtual float GetLod()
		{
			GetPoint(0.5f, out Vector3 pos, out Vector3 _, out Vector3 _);
			pos = base.transform.TransformPoint(pos);
			float num = float.MaxValue;
			for (int i = 0; i < NetGame.instance.local.players.Count; i++)
			{
				Human human = NetGame.instance.local.players[i].human;
				num = Mathf.Min(num, (human.transform.position - pos).sqrMagnitude);
			}
			return 1f + Mathf.InverseLerp(100f, 1000f, num) * 4f;
		}

		protected void ForceUpdate()
		{
			forceUpdate = true;
		}

		public virtual void LateUpdate()
		{
			if (!isDirty)
			{
				CheckDirty();
			}
			if (!forceUpdate && (!visible || !isDirty))
			{
				return;
			}
			forceUpdate = false;
			ReadData();
			float lod = GetLod();
			int idx = 0;
			int num = (int)((float)meshSegments / lod);
			int num2 = num + 1;
			for (int i = 0; i < num2; i++)
			{
				UpdateRing(1f * (float)i / (float)num, ref idx);
			}
			for (int j = 0; j < meshSegments - num; j++)
			{
				for (int k = 0; k < segmentsAround; k++)
				{
					meshVerts[idx++] = meshVerts[idx - segmentsAround];
				}
			}
			UpdateRing(0f, ref idx);
			UpdateRing(1f, ref idx);
			mesh.vertices = meshVerts;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			filter.sharedMesh = mesh;
			isDirty = false;
		}

		private void UpdateRing(float t, ref int idx)
		{
			GetPoint(t, out Vector3 pos, out Vector3 normal, out Vector3 binormal);
			for (int i = 0; i < segmentsAround; i++)
			{
				Vector2 vector = rotatedRadius[i];
				Vector3 vector2 = pos + vector.x * normal + vector.y * binormal;
				meshVerts[idx++] = vector2;
			}
		}

		public virtual void GetPoint(float dist, out Vector3 pos, out Vector3 normal, out Vector3 binormal)
		{
			pos = (normal = (binormal = Vector3.zero));
		}

		public void OnBecameInvisible()
		{
			visible = false;
		}

		public void OnBecameVisible()
		{
			visible = true;
		}

		public virtual void CheckDirty()
		{
		}

		public virtual void ReadData()
		{
		}
	}
}
