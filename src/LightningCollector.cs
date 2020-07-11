using HumanAPI;
using System.Linq;

public class LightningCollector : Node
{
	public NodeInput input;

	public LightningTerminal[] terminals;

	public PowerOutlet outlet;

	public float voltage = 1f;

	private bool active;

	private void Start()
	{
	}

	private void Update()
	{
		if (input.value > 0.5f && terminals.All((LightningTerminal f) => f.isActive))
		{
			if (!active)
			{
				active = true;
				outlet.voltage = voltage;
				Circuit.Refresh(outlet);
			}
		}
		else if (active)
		{
			active = false;
			outlet.voltage = 0f;
			Circuit.Refresh(outlet);
		}
	}
}
