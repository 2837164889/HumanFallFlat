using HumanAPI;

public class SignalTriggerCheckpoint : Node
{
	public NodeInput triggerCheckpoint;

	public int checkpointNum;

	public int checkpointSubObjective;

	public bool debugLog;

	public override void Process()
	{
		if (triggerCheckpoint.value > 0.5f)
		{
			Dependencies.Get<IGame>().EnterCheckpoint(checkpointNum, checkpointSubObjective);
		}
	}
}
