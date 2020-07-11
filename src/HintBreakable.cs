public class HintBreakable : Hint
{
	public VoronoiShatter shatter;

	protected override bool StillValid()
	{
		return !shatter.shattered;
	}
}
