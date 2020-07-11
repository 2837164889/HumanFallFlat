using UnityEngine;

namespace HumanAPI
{
	public class ChargeableBattery : PowerOutlet
	{
		public float maxVoltage = 1f;

		public float chargePerStep = 0.1f;

		public float currentThreshold = -0.001f;

		private float nextCharge;

		public override void RunCurrent(float I)
		{
			nextCharge = 0.5f;
			base.RunCurrent(I);
		}

		public override void StopCurrent()
		{
			base.StopCurrent();
		}

		private void Update()
		{
			if (voltage < maxVoltage && base.current < currentThreshold)
			{
				nextCharge -= Time.deltaTime;
				if (nextCharge < 0f)
				{
					voltage = Mathf.MoveTowards(voltage, maxVoltage, chargePerStep);
					Circuit.Refresh(this);
					nextCharge = 0.5f;
				}
			}
		}
	}
}
