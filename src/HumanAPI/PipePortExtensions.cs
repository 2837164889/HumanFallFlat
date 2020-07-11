namespace HumanAPI
{
	public static class PipePortExtensions
	{
		public static bool CanConnect(this PipePort lhs, PipePort rhs)
		{
			return rhs != null && rhs.connectedPort == null && lhs.connectedPort == null && lhs.connectable && rhs.connectable && lhs.isMale != rhs.isMale;
		}
	}
}
