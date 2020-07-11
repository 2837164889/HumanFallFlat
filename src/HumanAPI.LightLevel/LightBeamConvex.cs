using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class LightBeamConvex : LightBeam
	{
		public float focalLength = 5f;

		private MeshFilter meshFilter;

		private List<int> vertEndids;

		private Vector3[] startingVerts;

		private Color _color = Color.white;

		private float _range;

		public override Color color
		{
			get
			{
				return _color;
			}
			set
			{
				if (value.r + value.g + value.b == 0f)
				{
					DisableLight();
				}
				else if (value.a == 0f)
				{
					value.a = 1f;
				}
			}
		}

		public override float range
		{
			get
			{
				return _range;
			}
			protected set
			{
				_range = value;
				Vector3 localScale = mycollider.transform.localScale;
				localScale.z = value;
				mycollider.transform.localScale = localScale;
				UpdateCone();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			meshFilter = GetComponentInChildren<MeshFilter>();
			startingVerts = new Vector3[meshFilter.mesh.vertexCount];
			meshFilter.mesh.vertices.CopyTo(startingVerts, 0);
			vertEndids = new List<int>();
			Vector3 vector = startingVerts[0];
			for (int i = 0; i < startingVerts.Length; i++)
			{
				if (vector.y != startingVerts[i].y)
				{
					vertEndids.Add(i);
				}
			}
		}

		private void UpdateCone()
		{
			if (!(meshFilter == null))
			{
				Vector3[] vertices = meshFilter.mesh.vertices;
				float y = vertices[0].y;
				Vector3 b = new Vector3(0f, focalLength + y, 0f);
				float t = range / focalLength;
				for (int i = 0; i < vertEndids.Count; i++)
				{
					int num = vertEndids[i];
					vertices[num] = Vector3.LerpUnclamped(startingVerts[num], b, t);
					vertices[num].y = range + y;
				}
				meshFilter.mesh.vertices = vertices;
				meshFilter.mesh.RecalculateBounds();
			}
		}

		public override void SetSize(Bounds b)
		{
			Vector3 size = b.size;
			size.z = 1f;
			base.transform.localScale = size;
		}

		public override void Reset()
		{
			base.Reset();
			focalLength = 5f;
			meshFilter.mesh.vertices = startingVerts;
		}
	}
}
