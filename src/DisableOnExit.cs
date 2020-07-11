using HumanAPI;
using Multiplayer;
using UnityEngine;

public class DisableOnExit : MonoBehaviour
{
	public static void ExitingLevel(Level level)
	{
		DisableOnExit[] componentsInChildren = level.GetComponentsInChildren<DisableOnExit>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(value: false);
		}
		NetScope.ExitingLevel(level);
	}
}
