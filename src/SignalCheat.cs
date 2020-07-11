using HumanAPI;
using UnityEngine;

[AddNodeMenuItem]
public class SignalCheat : Node
{
	public string cheatName;

	public NodeInput input;

	public NodeOutput output;

	public float cheatValue;

	public bool enableCheat;

	public override string Title => "Cheat: " + cheatName + ((!enableCheat) ? " (Off)" : " (On)");

	public override void Process()
	{
		output.SetValue(input.value);
	}

	public static bool AnyCheatsEnabled()
	{
		SignalCheat[] array = Object.FindObjectsOfType<SignalCheat>();
		SignalCheat[] array2 = array;
		foreach (SignalCheat signalCheat in array2)
		{
			if (signalCheat.enableCheat)
			{
				Debug.Log("Cheat " + signalCheat.name + " enabled.", signalCheat);
				return true;
			}
		}
		return false;
	}
}
