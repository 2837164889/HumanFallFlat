using HumanAPI;
using System.Collections;
using UnityEngine;

public class SignalScriptNode1 : Node
{
	[Tooltip("The Output to send to the linked graph node")]
	public NodeOutput sendSignal;

	[Tooltip("Use this nametag to pick a particular node")]
	public string nameTag;

	[Tooltip("Set this to the delay before the outgoing signal is reset")]
	public float delayTimer = 0.1f;

	public bool useDelayTime = true;

	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private void OnProcess()
	{
	}

	public void SendSignal(float signalValue)
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Sending signal now");
		}
		sendSignal.SetValue(signalValue);
		if (useDelayTime)
		{
			StartCoroutine(ResetPostDelay());
		}
	}

	private IEnumerator ResetPostDelay()
	{
		if (showDebug)
		{
			Debug.Log(base.name + " Resetting the value in a bit ");
		}
		yield return new WaitForSecondsRealtime(delayTimer);
		sendSignal.SetValue(0f);
	}
}
