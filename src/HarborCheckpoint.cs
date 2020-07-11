using HumanAPI;
using UnityEngine;

public class HarborCheckpoint : Checkpoint
{
	public Transform ship;

	public Transform harborShipAlign;

	public Transform harborShipAlignReverse;

	public bool reverse;

	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		float num = Vector3.Dot(ship.up, harborShipAlign.up);
		reverse = (num < 0f);
	}

	public override void LoadHere()
	{
		if (reverse)
		{
			ship.rotation = harborShipAlignReverse.rotation;
			ship.position = harborShipAlignReverse.position;
		}
		else
		{
			ship.rotation = harborShipAlign.rotation;
			ship.position = harborShipAlign.position;
		}
	}
}
