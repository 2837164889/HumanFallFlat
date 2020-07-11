using HumanAPI;

public class AchievementNoCrane : Node
{
	public NodeInput input;

	public NodeOutput output;

	public static bool usedCrane;

	private void Start()
	{
		usedCrane = false;
	}

	public override void Process()
	{
		if (input.value != 0f)
		{
			usedCrane = true;
		}
		output.SetValue(input.value);
	}
}
