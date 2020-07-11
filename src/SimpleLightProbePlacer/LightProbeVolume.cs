using System.Collections.Generic;
using UnityEngine;

namespace SimpleLightProbePlacer
{
	[AddComponentMenu("Rendering/Light Probe Volume")]
	public class LightProbeVolume : TransformVolume
	{
		[SerializeField]
		private LightProbeVolumeType m_type;

		[SerializeField]
		private Vector3 m_densityFixed = Vector3.one;

		[SerializeField]
		private Vector3 m_densityFloat = Vector3.one;

		public LightProbeVolumeType Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		public Vector3 Density
		{
			get
			{
				return (m_type != 0) ? m_densityFloat : m_densityFixed;
			}
			set
			{
				if (m_type == LightProbeVolumeType.Fixed)
				{
					m_densityFixed = value;
				}
				else
				{
					m_densityFloat = value;
				}
			}
		}

		public static Color EditorColor => new Color(1f, 0.9f, 0.25f);

		public List<Vector3> CreatePositions()
		{
			return CreatePositions(m_type);
		}

		public List<Vector3> CreatePositions(LightProbeVolumeType type)
		{
			return (type != 0) ? CreatePositionsFloat(base.transform, base.Origin, base.Size, Density) : CreatePositionsFixed(base.transform, base.Origin, base.Size, Density);
		}

		public static List<Vector3> CreatePositionsFixed(Transform volumeTransform, Vector3 origin, Vector3 size, Vector3 density)
		{
			List<Vector3> list = new List<Vector3>();
			Vector3 a = origin;
			float num = size.x / (float)Mathf.FloorToInt(density.x);
			float num2 = size.y / (float)Mathf.FloorToInt(density.y);
			float num3 = size.z / (float)Mathf.FloorToInt(density.z);
			a -= size * 0.5f;
			for (int i = 0; (float)i <= density.x; i++)
			{
				for (int j = 0; (float)j <= density.y; j++)
				{
					for (int k = 0; (float)k <= density.z; k++)
					{
						Vector3 position = a + new Vector3((float)i * num, (float)j * num2, (float)k * num3);
						position = volumeTransform.TransformPoint(position);
						list.Add(position);
					}
				}
			}
			return list;
		}

		public static List<Vector3> CreatePositionsFloat(Transform volumeTransform, Vector3 origin, Vector3 size, Vector3 density)
		{
			List<Vector3> list = new List<Vector3>();
			Vector3 a = origin;
			int num = Mathf.FloorToInt(size.x / density.x);
			int num2 = Mathf.FloorToInt(size.y / density.y);
			int num3 = Mathf.FloorToInt(size.z / density.z);
			a -= size * 0.5f;
			a.x += (size.x - (float)num * density.x) * 0.5f;
			a.y += (size.y - (float)num2 * density.y) * 0.5f;
			a.z += (size.z - (float)num3 * density.z) * 0.5f;
			for (int i = 0; i <= num; i++)
			{
				for (int j = 0; j <= num2; j++)
				{
					for (int k = 0; k <= num3; k++)
					{
						Vector3 position = a + new Vector3((float)i * density.x, (float)j * density.y, (float)k * density.z);
						position = volumeTransform.TransformPoint(position);
						list.Add(position);
					}
				}
			}
			return list;
		}
	}
}
