using UnityEngine;

namespace Multiplayer
{
	public class NetBodySleepOverride : MonoBehaviour
	{
		public float freezeDrag = 0.5f;

		public float freezeDragAngular = 0.5f;

		public float sleepTreshold = 0.005f;

		public float dampFrom = 0.005f;

		public float dampTo = 0.05f;
	}
}
