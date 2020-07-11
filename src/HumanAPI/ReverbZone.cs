using UnityEngine;

namespace HumanAPI
{
	[RequireComponent(typeof(BoxCollider))]
	public class ReverbZone : MonoBehaviour
	{
		public float weight = 1f;

		public float level;

		public float delay = 0.5f;

		public float diffusion = 0.5f;

		public float innerZoneOffset = 2f;

		public float lowPass = 22000f;

		public float highPass = 10f;

		private BoxCollider collider;

		private void OnEnable()
		{
			collider = GetComponent<BoxCollider>();
		}

		public void OnDrawGizmosSelected()
		{
			collider = GetComponent<BoxCollider>();
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = new Color(1f, 0f, 0f, 0.8f);
			Gizmos.DrawCube(collider.center, collider.size - Vector3.one * innerZoneOffset * 2f);
			Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
			Gizmos.DrawCube(collider.center, collider.size);
			Gizmos.matrix = matrix;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player")
			{
				ReverbManager.instance.ZoneEntered(this);
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
			{
				ReverbManager.instance.ZoneLeft(this);
			}
		}

		public float GetWeight(Vector3 pos)
		{
			Vector3 vector = base.transform.InverseTransformPoint(pos) - collider.center;
			float b = innerZoneOffset;
			Vector3 size = collider.size;
			float a = Mathf.InverseLerp(0f, b, size.x / 2f - Mathf.Abs(vector.x));
			float b2 = innerZoneOffset;
			Vector3 size2 = collider.size;
			float b3 = Mathf.InverseLerp(0f, b2, size2.y / 2f - Mathf.Abs(vector.y));
			float b4 = innerZoneOffset;
			Vector3 size3 = collider.size;
			float b5 = Mathf.InverseLerp(0f, b4, size3.z / 2f - Mathf.Abs(vector.z));
			return Mathf.Min(Mathf.Min(a, b3), b5) * weight;
		}
	}
}
