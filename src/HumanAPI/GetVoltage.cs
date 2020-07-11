using UnityEngine;

namespace HumanAPI
{
	public class GetVoltage : Node, ICircuitComponent
	{
		public NodeOutput voltage;

		public PowerSocket minus;

		public PowerSocket plus;

		public float resistance = 1f;

		public bool ignorePhase;

		public float current
		{
			get;
			set;
		}

		public CircuitConnector forwardConnector => minus;

		public CircuitConnector reverseConnector => plus;

		public bool isOpen => false;

		public float CalculateVoltage(float I)
		{
			return I / resistance;
		}

		public void RunCurrent(float I)
		{
			if (ignorePhase)
			{
				I = Mathf.Abs(I);
			}
			voltage.SetValue(I / resistance);
			if (Mathf.Abs(voltage.value) > 2.5f)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_POWER_3VOLTS);
			}
			current = I;
		}

		public void StopCurrent()
		{
			voltage.SetValue(0f);
			current = 0f;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			minus.parent = (plus.parent = this);
			minus.isForward = true;
			plus.isForward = false;
		}
	}
}
