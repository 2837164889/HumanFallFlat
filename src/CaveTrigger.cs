using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class CaveTrigger : MonoBehaviour, IReset
{
	private float depth;

	private Dictionary<NetPlayer, float> playerPhase = new Dictionary<NetPlayer, float>();

	private CaveLighting lighting;

	private void OnEnable()
	{
		lighting = GetComponentInParent<CaveLighting>();
		Vector3 size = GetComponent<BoxCollider>().size;
		depth = size.z;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!(other.tag != "Player"))
		{
			NetPlayer componentInParent = other.GetComponentInParent<NetPlayer>();
			if (!(componentInParent == null) && componentInParent.isLocalPlayer && !playerPhase.ContainsKey(componentInParent))
			{
				playerPhase[componentInParent] = GetPhase(componentInParent);
				lighting.SetPhase(componentInParent, playerPhase[componentInParent]);
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.tag != "Player")
		{
			return;
		}
		NetPlayer componentInParent = other.GetComponentInParent<NetPlayer>();
		if (!(componentInParent == null) && playerPhase.ContainsKey(componentInParent))
		{
			if (playerPhase[componentInParent] > 0.5f)
			{
				lighting.SetPhase(componentInParent, 1f);
			}
			else
			{
				lighting.SetPhase(componentInParent, 0f);
			}
			playerPhase.Remove(componentInParent);
		}
	}

	private void Update()
	{
		Dictionary<NetPlayer, float>.KeyCollection keys = playerPhase.Keys;
		for (int i = 0; i < NetGame.instance.local.players.Count; i++)
		{
			NetPlayer netPlayer = NetGame.instance.local.players[i];
			if (playerPhase.ContainsKey(netPlayer))
			{
				playerPhase[netPlayer] = GetPhase(netPlayer);
				lighting.SetPhase(netPlayer, playerPhase[netPlayer]);
			}
		}
	}

	private float GetPhase(NetPlayer player)
	{
		Vector3 vector = base.transform.InverseTransformPoint(player.human.transform.position);
		return vector.z / depth + 0.5f;
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		playerPhase.Clear();
	}
}
