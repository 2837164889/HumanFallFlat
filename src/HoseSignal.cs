using HumanAPI;

public class HoseSignal : Node
{
	public NodeOutput startConnected;

	public NodeOutput endConnected;

	public NodeOutput bothConnected;

	public HosePipe hosePipe;

	private void Start()
	{
		if (hosePipe == null)
		{
			hosePipe = GetComponent<HosePipe>();
		}
	}

	private void Update()
	{
		if (hosePipe != null)
		{
			if (hosePipe.startPlug != null && hosePipe.startPlug.connectedSocket != null)
			{
				startConnected.SetValue(1f);
			}
			else
			{
				startConnected.SetValue(0f);
			}
			if (hosePipe.endPlug != null && hosePipe.endPlug.connectedSocket != null)
			{
				endConnected.SetValue(1f);
			}
			else
			{
				endConnected.SetValue(0f);
			}
			bothConnected.SetValue((!(startConnected.value >= 0.5f) || !(endConnected.value >= 0.5f)) ? 0f : 1f);
		}
	}
}
