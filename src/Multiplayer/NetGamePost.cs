using UnityEngine;

namespace Multiplayer
{
	public class NetGamePost : MonoBehaviour
	{
		private void FixedUpdate()
		{
			NetGame.instance.PostFixedUpdate();
		}

		private void Update()
		{
			NetGame.instance.PostUpdate();
		}

		private void LateUpdate()
		{
			NetGame.instance.PostLateUpdate();
		}
	}
}
