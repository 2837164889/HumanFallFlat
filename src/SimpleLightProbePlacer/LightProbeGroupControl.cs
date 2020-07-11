using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleLightProbePlacer
{
	[RequireComponent(typeof(LightProbeGroup))]
	[AddComponentMenu("Rendering/Light Probe Group Control")]
	public class LightProbeGroupControl : MonoBehaviour
	{
		[SerializeField]
		private float m_mergeDistance = 0.5f;

		[SerializeField]
		private bool m_usePointLights = true;

		[SerializeField]
		private float m_pointLightRange = 1f;

		private int m_mergedProbes;

		private LightProbeGroup m_lightProbeGroup;

		public float MergeDistance
		{
			get
			{
				return m_mergeDistance;
			}
			set
			{
				m_mergeDistance = value;
			}
		}

		public int MergedProbes => m_mergedProbes;

		public bool UsePointLights
		{
			get
			{
				return m_usePointLights;
			}
			set
			{
				m_usePointLights = value;
			}
		}

		public float PointLightRange
		{
			get
			{
				return m_pointLightRange;
			}
			set
			{
				m_pointLightRange = value;
			}
		}

		public LightProbeGroup LightProbeGroup
		{
			get
			{
				if (m_lightProbeGroup != null)
				{
					return m_lightProbeGroup;
				}
				return m_lightProbeGroup = GetComponent<LightProbeGroup>();
			}
		}

		public void DeleteAll()
		{
			LightProbeGroup.probePositions = null;
			m_mergedProbes = 0;
		}

		public void Create()
		{
			DeleteAll();
			List<Vector3> list = CreatePositions();
			list.AddRange(CreateAroundPointLights(m_pointLightRange));
			list = MergeClosestPositions(list, m_mergeDistance, out m_mergedProbes);
			ApplyPositions(list);
		}

		public void Merge()
		{
			if (LightProbeGroup.probePositions != null)
			{
				List<Vector3> source = MergeClosestPositions(LightProbeGroup.probePositions.ToList(), m_mergeDistance, out m_mergedProbes);
				source = source.Select((Vector3 x) => base.transform.TransformPoint(x)).ToList();
				ApplyPositions(source);
			}
		}

		private void ApplyPositions(List<Vector3> positions)
		{
			LightProbeGroup.probePositions = positions.Select((Vector3 x) => base.transform.InverseTransformPoint(x)).ToArray();
		}

		private static List<Vector3> CreatePositions()
		{
			LightProbeVolume[] array = Object.FindObjectsOfType<LightProbeVolume>();
			if (array.Length == 0)
			{
				return new List<Vector3>();
			}
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < array.Length; i++)
			{
				list.AddRange(array[i].CreatePositions());
			}
			return list;
		}

		private static List<Vector3> CreateAroundPointLights(float range)
		{
			List<Light> list = (from x in Object.FindObjectsOfType<Light>()
				where x.type == LightType.Point
				select x).ToList();
			if (list.Count == 0)
			{
				return new List<Vector3>();
			}
			List<Vector3> list2 = new List<Vector3>();
			for (int i = 0; i < list.Count; i++)
			{
				list2.AddRange(CreatePositionsAround(list[i].transform, range));
			}
			return list2;
		}

		private static List<Vector3> MergeClosestPositions(List<Vector3> positions, float distance, out int mergedCount)
		{
			if (positions == null)
			{
				mergedCount = 0;
				return new List<Vector3>();
			}
			int count = positions.Count;
			bool flag = false;
			while (!flag)
			{
				Dictionary<Vector3, List<Vector3>> dictionary = new Dictionary<Vector3, List<Vector3>>();
				for (int i = 0; i < positions.Count; i++)
				{
					List<Vector3> list = positions.Where((Vector3 x) => (x - positions[i]).magnitude < distance).ToList();
					if (list.Count > 0 && !dictionary.ContainsKey(positions[i]))
					{
						dictionary.Add(positions[i], list);
					}
				}
				positions.Clear();
				List<Vector3> list2 = dictionary.Keys.ToList();
				for (int j = 0; j < list2.Count; j++)
				{
					Vector3 center = dictionary[list2[j]].Aggregate(Vector3.zero, (Vector3 result, Vector3 target) => result + target) / dictionary[list2[j]].Count;
					if (!positions.Exists((Vector3 x) => x == center))
					{
						positions.Add(center);
					}
				}
				flag = positions.Select((Vector3 x) => positions.Where((Vector3 y) => y != x && (y - x).magnitude < distance)).All((IEnumerable<Vector3> x) => !x.Any());
			}
			mergedCount = count - positions.Count;
			return positions;
		}

		public static List<Vector3> CreatePositionsAround(Transform transform, float range)
		{
			Vector3[] source = new Vector3[8]
			{
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, 0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, 0.5f),
				new Vector3(0.5f, -0.5f, -0.5f)
			};
			return source.Select((Vector3 x) => transform.TransformPoint(x * range)).ToList();
		}
	}
}
