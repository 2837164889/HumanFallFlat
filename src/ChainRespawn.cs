using Multiplayer;
using UnityEngine;

public sealed class ChainRespawn : MonoBehaviour
{
	private const float kMinimumTimeBetweenCallbacks = 2f;

	private NetBody[] chainBodies;

	private float lastCallback = -1f;

	private void Awake()
	{
		chainBodies = GetComponentsInChildren<NetBody>();
	}

	public void OnChainRespawn()
	{
		float unscaledTime = Time.unscaledTime;
		if (unscaledTime - lastCallback > 2f)
		{
			lastCallback = unscaledTime;
			NetBody[] array = chainBodies;
			foreach (NetBody netBody in array)
			{
				netBody.Respawn(Vector3.zero);
			}
		}
	}
}
