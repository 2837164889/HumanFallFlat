using HumanAPI;
using UnityEngine;

public class BendTutorial : TutorialBlock
{
	public GameObject bendableParent;

	private Bendable[] bendables;

	protected override void OnEnable()
	{
		base.OnEnable();
		bendables = bendableParent.GetComponentsInChildren<Bendable>();
	}

	public override bool IsPlayerActivityMakingSense()
	{
		return false;
	}

	public override bool CheckInstantSuccess(bool playerInside)
	{
		if (playerInside)
		{
			for (int i = 0; i < bendables.Length; i++)
			{
				if (bendables[i].isBent)
				{
					return true;
				}
			}
		}
		return false;
	}
}
