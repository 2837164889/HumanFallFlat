using HumanAPI;
using UnityEngine;

public class NarrativeForceSignal : Node
{
	public NodeInput triggerSignal;

	public NarrativeBlock narrative;

	public override void Process()
	{
		base.Process();
		if (Mathf.Abs(triggerSignal.value) >= 0.5f)
		{
			narrative.Play();
		}
	}
}
