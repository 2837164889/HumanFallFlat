using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class LightBeamCollider : MonoBehaviour
	{
		public LightBeam beam;

		private void Awake()
		{
			if (!beam)
			{
				beam = GetComponentInParent<LightBeam>();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.isTrigger)
			{
				if (!beam.hitColliders.ContainsKey(other.transform))
				{
					beam.hitColliders.Add(other.transform, other.transform.position);
				}
				beam.UpdateLight();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.isTrigger)
			{
				if (beam.hitColliders.ContainsKey(other.transform))
				{
					beam.hitColliders.Remove(other.transform);
				}
				beam.UpdateLight();
			}
		}
	}
}
