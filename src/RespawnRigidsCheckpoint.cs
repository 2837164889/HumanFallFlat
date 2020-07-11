using HumanAPI;
using Multiplayer;
using UnityEngine;

public class RespawnRigidsCheckpoint : Checkpoint
{
	public RestartableRigid[] restartList;

	public NetBody[] bodies;

	public override void RespawnHere()
	{
		base.RespawnHere();
		for (int i = 0; i < restartList.Length; i++)
		{
			if (restartList[i] != null)
			{
				restartList[i].Reset(Vector3.zero);
			}
		}
		for (int j = 0; j < bodies.Length; j++)
		{
			RespawnRoot componentInParent = bodies[j].GetComponentInParent<RespawnRoot>();
			if (componentInParent != null)
			{
				componentInParent.Respawn(Vector3.zero);
			}
			else
			{
				bodies[j].Respawn(Vector3.zero);
			}
		}
	}
}
