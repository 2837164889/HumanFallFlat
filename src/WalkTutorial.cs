public class WalkTutorial : TutorialBlock
{
	public override bool IsPlayerActivityMakingSense()
	{
		for (int i = 0; i < Human.all.Count; i++)
		{
			Human human = Human.all[i];
			if (human.state == HumanState.Walk)
			{
				return true;
			}
		}
		return false;
	}
}
