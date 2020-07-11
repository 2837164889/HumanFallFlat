public class HintTreeChop : Hint
{
	public TreeCutting theTree;

	protected override bool StillValid()
	{
		if (theTree != null)
		{
			return !theTree.IsCut;
		}
		return false;
	}
}
