using UnityEngine;

namespace Multiplayer
{
	public class NetGamePre : MonoBehaviour
	{
		private void FixedUpdate()
		{
			NetGame.instance.PreFixedUpdate();
		}

		private void Update()
		{
			NetGame.instance.PreUpdate();
		}

		private void LateUpdate()
		{
			NetGame.instance.PreLateUpdate();
		}
	}
}
