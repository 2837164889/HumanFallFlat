using Multiplayer;
using UnityEngine;

public class SetStateOnReset : MonoBehaviour, IReset
{
	public bool activeOnReset;

	public void ResetState(int checkpoint, int subObjectives)
	{
		NetBody component = GetComponent<NetBody>();
		if ((bool)component)
		{
			component.SetVisible(activeOnReset);
		}
		else
		{
			base.gameObject.SetActive(activeOnReset);
		}
	}
}
