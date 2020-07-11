using Multiplayer;

public class RemoteTutorial : TutorialBlock
{
	public TutorialScreen screen;

	public override bool IsPlayerActivityMakingSense()
	{
		for (int i = 0; i < NetGame.instance.players.Count; i++)
		{
			Human human = NetGame.instance.players[i].human;
			if (human.controls.leftGrab || human.controls.rightGrab)
			{
				return true;
			}
		}
		return false;
	}

	public override bool CheckInstantSuccess(bool playerInside)
	{
		return screen.time > 2f;
	}
}
