public class WallBreakTutorial : TutorialBlock
{
	public VoronoiShatter wall;

	public override bool IsPlayerActivityMakingSense()
	{
		return false;
	}

	public override bool CheckInstantSuccess(bool playerInside)
	{
		return wall.shattered;
	}
}
