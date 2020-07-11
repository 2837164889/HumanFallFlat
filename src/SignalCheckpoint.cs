using HumanAPI;

public class SignalCheckpoint : Node, IReset
{
	private int lastCheckpointInternal;

	public NodeOutput lastCheckpoint;

	private void Update()
	{
		lastCheckpoint.SetValue(lastCheckpointInternal);
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		lastCheckpointInternal = checkpoint;
	}
}
