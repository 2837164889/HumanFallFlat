using HumanAPI;
using UnityEngine;

public class GrabTutorial : TutorialBlock
{
	public NodeInput successSignal;

	public override bool IsPlayerActivityMakingSense()
	{
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (human.controls.leftGrab || human.controls.rightGrab)
			{
				return true;
			}
		}
		return false;
	}

	public override bool CheckInstantSuccess(bool playerInside)
	{
		return Mathf.Abs(successSignal.value) >= 0.5f;
	}
}
