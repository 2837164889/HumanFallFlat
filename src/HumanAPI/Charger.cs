namespace HumanAPI
{
	public class Charger : PowerOutlet
	{
		public NodeOutput ampers;

		public NodeOutput charging;

		public NodeOutput charged;

		public float cutoffCurrent = 0.1f;

		public override void StopCurrent()
		{
			base.StopCurrent();
			ampers.SetValue(0f);
			charging.SetValue(0f);
			charged.SetValue(0f);
		}

		public override void RunCurrent(float I)
		{
			base.RunCurrent(I);
			ampers.SetValue(I);
			charging.SetValue((I > cutoffCurrent) ? 1 : 0);
			charged.SetValue((I < cutoffCurrent) ? 1 : 0);
		}
	}
}
