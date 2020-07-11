using HumanAPI;
using Multiplayer;
using UnityEngine;

public class HeatToSignalPassThru : MonoBehaviour
{
	[SerializeField]
	private HeatToSignal heatToSignal;

	private void OnTriggerEnter(Collider other)
	{
		if (!NetGame.isClient)
		{
			heatToSignal.OnTriggerEnter(other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!NetGame.isClient)
		{
			heatToSignal.OnTriggerExit(other);
		}
	}
}
