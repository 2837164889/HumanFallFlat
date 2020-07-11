namespace HumanAPI
{
	public interface ICircuitComponent
	{
		CircuitConnector forwardConnector
		{
			get;
		}

		CircuitConnector reverseConnector
		{
			get;
		}

		bool isOpen
		{
			get;
		}

		float current
		{
			get;
		}

		float CalculateVoltage(float I);

		void RunCurrent(float I);

		void StopCurrent();
	}
}
