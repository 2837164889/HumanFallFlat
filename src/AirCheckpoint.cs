using HumanAPI;
using Multiplayer;
using UnityEngine;

public class AirCheckpoint : Checkpoint, IReset
{
	public GameObject boilerCoalContainer;

	public Boiler boiler1;

	public Boiler boiler2;

	public Boiler boiler3;

	public override void OnTriggerEnter(Collider other)
	{
		if (boiler1.output.value >= 1f && boiler2.output.value >= 1f && boiler2.output.value >= 1f)
		{
			base.OnTriggerEnter(other);
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (boiler1.output.value >= 1f && boiler2.output.value >= 1f && boiler2.output.value >= 1f)
		{
			base.OnTriggerEnter(other);
		}
	}

	public override void LoadHere()
	{
		boiler1.Ignite();
		boiler2.Ignite();
		boiler3.Ignite();
		NetSetActive[] componentsInChildren = boilerCoalContainer.GetComponentsInChildren<NetSetActive>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetActive(show: true);
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		NetSetActive[] componentsInChildren = boilerCoalContainer.GetComponentsInChildren<NetSetActive>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetActive(show: false);
		}
	}
}
