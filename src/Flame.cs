using HumanAPI;

public class Flame : Node
{
	public NodeOutput isHot = new NodeOutput
	{
		initialValue = 1f
	};

	public void Ignite()
	{
		isHot.SetValue(1f);
	}

	public void Extinguish()
	{
		isHot.SetValue(0f);
	}
}
