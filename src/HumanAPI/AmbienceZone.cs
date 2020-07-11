using UnityEngine;

namespace HumanAPI
{
	public class AmbienceZone : MonoBehaviour
	{
		public int priority;

		public float transitionDuration = 3f;

		public float mainVerbLevel;

		public float musicLevel;

		public float ambienceLevel;

		public float effectsLevel;

		public float physicsLevel;

		public float characterLevel;

		public float ambienceFxLevel;

		public AmbienceSource[] sources;

		public float[] volumes;

		public float GetLevel(AmbienceSource source)
		{
			if (sources == null)
			{
				return 0f;
			}
			for (int i = 0; i < sources.Length; i++)
			{
				if (sources[i] == source)
				{
					return volumes[i];
				}
			}
			return 0f;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (Ambience.instance.useTriggers)
			{
				Ambience.instance.EnterZone(this);
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (Ambience.instance.useTriggers)
			{
				Ambience.instance.LeaveZone(this);
			}
		}

		public void OnDrawGizmosSelected()
		{
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
			BoxCollider component = GetComponent<BoxCollider>();
			if (component != null)
			{
				Gizmos.DrawCube(component.center, component.size);
			}
			MeshCollider component2 = GetComponent<MeshCollider>();
			if (component2 != null)
			{
				Gizmos.DrawMesh(component2.sharedMesh);
			}
			Gizmos.matrix = matrix;
		}
	}
}
