namespace HumanAPI
{
	public class SteamSource : SteamPort
	{
		public NodeInput pressure;

		public override bool isOpen => false;

		public override float ownPressure => pressure.value;

		public override SteamPort connectedPort => null;

		public override void ApplySystemState(bool isOpenSystem, float pressure)
		{
		}

		public override void Process()
		{
			base.Process();
			SteamSystem.Recalculate(node);
		}
	}
}
