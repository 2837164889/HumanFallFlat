public class HintLock : Hint
{
	public BreakableLock theLock;

	protected override bool StillValid()
	{
		return !theLock.shattered;
	}
}
