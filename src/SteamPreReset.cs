using Multiplayer;
using UnityEngine;

public class SteamPreReset : MonoBehaviour, IPreReset
{
	[SerializeField]
	private GameObject[] pipes;

	[SerializeField]
	private GameObject[] oldPipes;

	[SerializeField]
	private int checkpointVisible;

	private void SetVisible(GameObject pipe, bool visible)
	{
		NetBody component = pipe.GetComponent<NetBody>();
		if ((bool)component)
		{
			component.SetVisible(visible);
		}
	}

	void IPreReset.PreResetState(int checkpoint)
	{
		bool flag = checkpoint < checkpointVisible;
		SetVisible(pipes[0], !flag);
		SetVisible(pipes[1], !flag);
		SetVisible(oldPipes[0], flag);
		SetVisible(oldPipes[1], flag);
	}
}
