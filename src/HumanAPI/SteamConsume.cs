namespace HumanAPI
{
	public class SteamConsume : SteamPort, IPostEndReset
	{
		public NodeOutput output;

		public int EnableAfterCheckpoint = -1;

		public override bool isOpen => false;

		public override float ownPressure => 0f;

		public override SteamPort connectedPort => null;

		public override void ApplySystemState(bool isOpenSystem, float pressure)
		{
			if (!isOpenSystem)
			{
				output.SetValue(pressure);
			}
			else
			{
				output.SetValue(0f);
			}
		}

		void IPostEndReset.PostEndResetState(int checkpoint)
		{
			if (EnableAfterCheckpoint != -1)
			{
				output.SetValue((checkpoint > EnableAfterCheckpoint) ? 1 : 0);
			}
		}
	}
}
