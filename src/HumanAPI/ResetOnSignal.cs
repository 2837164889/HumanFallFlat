using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class ResetOnSignal : Node
	{
		public float height;

		public NodeInput input;

		public override void Process()
		{
			base.Process();
			if (input.value > 0.5f)
			{
				Respawn(base.transform, Vector3.up * height);
				IReset[] componentsInChildren = GetComponentsInChildren<IReset>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].ResetState(Game.instance.currentCheckpointNumber, Game.instance.currentCheckpointSubObjectives);
				}
				IPostReset[] componentsInChildren2 = GetComponentsInChildren<IPostReset>(includeInactive: true);
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].PostResetState(Game.instance.currentCheckpointNumber);
				}
			}
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
