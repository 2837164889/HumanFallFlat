using HumanAPI;

public class AchievementClosingDoor : Node
{
	private NodeInput inTriggerVolume;

	public ServoMotorNew motor;

	public float speedThreshold = 2f;

	public override void Process()
	{
		if (inTriggerVolume.value >= 0.5f && motor.Speed > speedThreshold && motor.input.value != 0f)
		{
		}
	}
}
