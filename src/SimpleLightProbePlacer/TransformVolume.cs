using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleLightProbePlacer
{
	[AddComponentMenu("")]
	public class TransformVolume : MonoBehaviour
	{
		[SerializeField]
		private Volume m_volume = new Volume(Vector3.zero, Vector3.one);

		public Volume Volume
		{
			get
			{
				return m_volume;
			}
			set
			{
				m_volume = value;
			}
		}

		public Vector3 Origin => m_volume.Origin;

		public Vector3 Size => m_volume.Size;

		public bool IsInBounds(Vector3[] points)
		{
			return GetBounds().Intersects(GetBounds(points));
		}

		public bool IsOnBorder(Vector3[] points)
		{
			if (points.All((Vector3 x) => !IsInVolume(x)))
			{
				return false;
			}
			return !points.All(IsInVolume);
		}

		public bool IsInVolume(Vector3[] points)
		{
			return points.All(IsInVolume);
		}

		public bool IsInVolume(Vector3 position)
		{
			for (int i = 0; i < 6; i++)
			{
				if (new Plane(GetSideDirection(i), GetSidePosition(i)).GetSide(position))
				{
					return false;
				}
			}
			return true;
		}

		public Vector3[] GetCorners()
		{
			Vector3[] array = new Vector3[8]
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
			for (int i = 0; i < array.Length; i++)
			{
				ref Vector3 reference = ref array[i];
				float x = reference.x;
				Vector3 size = m_volume.Size;
				reference.x = x * size.x;
				ref Vector3 reference2 = ref array[i];
				float y = reference2.y;
				Vector3 size2 = m_volume.Size;
				reference2.y = y * size2.y;
				ref Vector3 reference3 = ref array[i];
				float z = reference3.z;
				Vector3 size3 = m_volume.Size;
				reference3.z = z * size3.z;
				array[i] = base.transform.TransformPoint(m_volume.Origin + array[i]);
			}
			return array;
		}

		public Bounds GetBounds()
		{
			return GetBounds(GetCorners());
		}

		public Bounds GetBounds(Vector3[] points)
		{
			Vector3 center = points.Aggregate(Vector3.zero, (Vector3 result, Vector3 point) => result + point) / points.Length;
			Bounds result2 = new Bounds(center, Vector3.zero);
			for (int i = 0; i < points.Length; i++)
			{
				result2.Encapsulate(points[i]);
			}
			return result2;
		}

		public GameObject[] GetGameObjectsInBounds(LayerMask layerMask)
		{
			MeshRenderer[] array = Object.FindObjectsOfType<MeshRenderer>();
			List<GameObject> list = new List<GameObject>();
			Bounds bounds = GetBounds();
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].gameObject == base.transform.gameObject) && !(array[i].GetComponent<TransformVolume>() != null) && ((1 << array[i].gameObject.layer) & layerMask.value) != 0 && bounds.Intersects(array[i].bounds))
				{
					list.Add(array[i].gameObject);
				}
			}
			return list.ToArray();
		}

		public Vector3 GetSideDirection(int side)
		{
			Vector3[] array = new Vector3[6];
			Vector3 right = Vector3.right;
			Vector3 up = Vector3.up;
			Vector3 forward = Vector3.forward;
			array[0] = right;
			array[1] = -right;
			array[2] = up;
			array[3] = -up;
			array[4] = forward;
			array[5] = -forward;
			return base.transform.TransformDirection(array[side]);
		}

		public Vector3 GetSidePosition(int side)
		{
			Vector3[] array = new Vector3[6];
			Vector3 right = Vector3.right;
			Vector3 up = Vector3.up;
			Vector3 forward = Vector3.forward;
			array[0] = right;
			array[1] = -right;
			array[2] = up;
			array[3] = -up;
			array[4] = forward;
			array[5] = -forward;
			return base.transform.TransformPoint(array[side] * GetSizeAxis(side) + m_volume.Origin);
		}

		public float GetSizeAxis(int side)
		{
			switch (side)
			{
			case 0:
			case 1:
			{
				Vector3 size3 = m_volume.Size;
				return size3.x * 0.5f;
			}
			case 2:
			case 3:
			{
				Vector3 size2 = m_volume.Size;
				return size2.y * 0.5f;
			}
			default:
			{
				Vector3 size = m_volume.Size;
				return size.z * 0.5f;
			}
			}
		}
	}
}
