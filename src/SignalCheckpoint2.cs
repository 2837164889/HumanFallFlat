using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class SignalCheckpoint2 : Node, IReset
{
	private int lastCheckpointInternal;

	private int lastCheckpointSubObjectivesInternal;

	public NodeOutput lastCheckpoint;

	public NodeOutput[] subObjectiveOutputs;

	public bool debugLog;

	protected override void CollectAllSockets(List<NodeSocket> sockets)
	{
		for (int i = 0; i < subObjectiveOutputs.Length; i++)
		{
			subObjectiveOutputs[i].node = this;
			subObjectiveOutputs[i].name = "Sub-objective " + (i + 1).ToString();
			sockets.Add(subObjectiveOutputs[i]);
		}
	}

	private void Update()
	{
		lastCheckpoint.SetValue(lastCheckpointInternal);
		for (int i = 0; i < subObjectiveOutputs.Length; i++)
		{
			subObjectiveOutputs[i].SetValue(lastCheckpointSubObjectivesInternal & (1 << i));
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		lastCheckpointInternal = checkpoint;
		lastCheckpointSubObjectivesInternal = subObjectives;
		if (!debugLog)
		{
			return;
		}
		string text = "SignalCheckpoint state reset. Checkpoint: " + checkpoint;
		if (subObjectives != 0)
		{
			text += ". SubObjectives: ";
			for (int i = 0; i < subObjectiveOutputs.Length; i++)
			{
				text += (lastCheckpointSubObjectivesInternal & (1 << i));
				if (i < subObjectiveOutputs.Length - 1)
				{
					text += ", ";
				}
			}
		}
		Debug.Log(text);
	}
}
