using UnityEngine;

namespace HumanAPI
{
	public static class Circuit
	{
		private static bool IsCircuitClosed(CircuitConnector from)
		{
			CircuitConnector circuitConnector = from;
			do
			{
				if (circuitConnector.parent.isOpen)
				{
					return false;
				}
				circuitConnector = ((!circuitConnector.isForward) ? circuitConnector.parent.forwardConnector.connected : circuitConnector.parent.reverseConnector.connected);
			}
			while (circuitConnector != from && circuitConnector != null);
			if (circuitConnector == null)
			{
				return false;
			}
			return true;
		}

		private static float CalculateU(CircuitConnector from, float I)
		{
			float num = 0f;
			CircuitConnector circuitConnector = from;
			do
			{
				if (circuitConnector.isForward)
				{
					num += circuitConnector.parent.CalculateVoltage(I);
					circuitConnector = circuitConnector.parent.reverseConnector.connected;
				}
				else
				{
					num -= circuitConnector.parent.CalculateVoltage(0f - I);
					circuitConnector = circuitConnector.parent.forwardConnector.connected;
				}
			}
			while (circuitConnector != from && circuitConnector != null);
			return num;
		}

		public static float CalculateI(CircuitConnector from)
		{
			float num = 128f;
			float num2 = 0f;
			float num3 = CalculateU(from, num2);
			for (int i = 0; i < 16; i++)
			{
				if (num3 == 0f)
				{
					return num2;
				}
				float num4 = CalculateU(from, num2 + num / 1024f) - num3;
				num /= 2f;
				num2 = ((!(num4 * num3 > 0f)) ? (num2 + num) : (num2 - num));
				num3 = CalculateU(from, num2);
			}
			return num2;
		}

		public static void RunCurrent(CircuitConnector from, float I)
		{
			CircuitConnector circuitConnector = from;
			do
			{
				if (circuitConnector.isForward)
				{
					circuitConnector.parent.RunCurrent(I);
					circuitConnector = circuitConnector.parent.reverseConnector.connected;
				}
				else
				{
					circuitConnector.parent.RunCurrent(0f - I);
					circuitConnector = circuitConnector.parent.forwardConnector.connected;
				}
			}
			while (circuitConnector != from && circuitConnector != null);
		}

		public static void StopCurrent(CircuitConnector from)
		{
			CircuitConnector circuitConnector = from;
			do
			{
				if (circuitConnector.isForward)
				{
					circuitConnector.parent.StopCurrent();
					circuitConnector = circuitConnector.parent.reverseConnector.connected;
				}
				else
				{
					circuitConnector.parent.StopCurrent();
					circuitConnector = circuitConnector.parent.forwardConnector.connected;
				}
			}
			while (circuitConnector != from && circuitConnector != null);
		}

		public static bool Refresh(PowerOutlet component)
		{
			CircuitConnector forwardConnector = component.forwardConnector;
			if (!IsCircuitClosed(forwardConnector))
			{
				StopCurrent(forwardConnector);
				return true;
			}
			float num = CalculateI(forwardConnector);
			if (Mathf.Abs(num) > 100f)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_POWER_SHORT_CIRCUIT);
				StopCurrent(forwardConnector);
				return false;
			}
			RunCurrent(forwardConnector, num);
			if (!IsCircuitClosed(forwardConnector))
			{
				StopCurrent(forwardConnector);
				return true;
			}
			num = CalculateI(forwardConnector);
			if (Mathf.Abs(num) > 100f)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_POWER_SHORT_CIRCUIT);
				StopCurrent(forwardConnector);
				return false;
			}
			return true;
		}

		public static bool Connect(CircuitConnector c1, CircuitConnector c2)
		{
			if (c1.connected != null)
			{
				Debug.LogError("Connector already in use", c1);
				return false;
			}
			if (c2.connected != null)
			{
				Debug.LogError("Connector already in use", c2);
				return false;
			}
			c1.connected = c2;
			c2.connected = c1;
			if (!IsCircuitClosed(c1))
			{
				return true;
			}
			float num = CalculateI(c1);
			if (Mathf.Abs(num) > 100f)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_POWER_SHORT_CIRCUIT);
				Disconnect(c1);
				return false;
			}
			RunCurrent(c1, num);
			if (!IsCircuitClosed(c1))
			{
				StopCurrent(c1);
				return true;
			}
			num = CalculateI(c1);
			if (Mathf.Abs(num) > 100f)
			{
				StatsAndAchievements.UnlockAchievement(Achievement.ACH_POWER_SHORT_CIRCUIT);
				StopCurrent(c1);
				Disconnect(c1);
				return false;
			}
			return true;
		}

		public static void Disconnect(CircuitConnector c1)
		{
			if (c1.connected == null)
			{
				Debug.LogError("Connector not connected", c1);
				return;
			}
			if (c1.connected.connected == null)
			{
				Debug.LogError("Connector not connected", c1.connected);
				return;
			}
			StopCurrent(c1);
			c1.connected = (c1.connected.connected = null);
		}
	}
}
