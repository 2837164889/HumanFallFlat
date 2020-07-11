using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Respawning Rigidbody", 10)]
	public class RespawningBody : MonoBehaviour, IReset
	{
		private struct RigidState
		{
			public Rigidbody rigid;

			public Vector3 position;

			public Quaternion rotation;
		}

		public float despawnHeight = -50f;

		public float respawnHeight = 5f;

		private List<RespawningBodyOverride> overrides;

		private RigidState[] initialState;

		public void AddOverride(RespawningBodyOverride respawningBodyOverride)
		{
			if (overrides == null)
			{
				overrides = new List<RespawningBodyOverride>();
			}
			overrides.Add(respawningBodyOverride);
		}

		private void Awake()
		{
		}

		private void Update()
		{
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
		}

		public void Respawn(Vector3 offset)
		{
		}
	}
}
