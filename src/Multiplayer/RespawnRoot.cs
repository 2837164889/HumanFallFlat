using UnityEngine;

namespace Multiplayer
{
	public class RespawnRoot : MonoBehaviour
	{
		public void Respawn(Vector3 offset)
		{
			Respawn(base.transform, offset);
		}

		private void Respawn(Transform transform, Vector3 offset)
		{
			IRespawnable[] components = transform.GetComponents<IRespawnable>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].Respawn(offset);
			}
			for (int j = 0; j < transform.childCount; j++)
			{
				Respawn(transform.GetChild(j), offset);
			}
		}
	}
}
