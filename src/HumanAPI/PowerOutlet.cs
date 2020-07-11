using UnityEngine;

namespace HumanAPI
{
	public class PowerOutlet : Node, ICircuitComponent
	{
		[Tooltip("Reference to the positive socket")]
		public PowerSocket plus;

		[Tooltip("Reference to the negative socket")]
		public PowerSocket minus;

		[Tooltip("Volatage of this Power Outlet")]
		public float voltage = 1f;

		[Tooltip("Used to calculate the voltage of this outlet ")]
		public float reverseR = 1f;

		[Tooltip("The max allowed by this outlet")]
		public float maxI = 16f;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		public float current
		{
			get;
			set;
		}

		public CircuitConnector forwardConnector
		{
			get
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Forward Connector ");
				}
				return plus;
			}
		}

		public CircuitConnector reverseConnector
		{
			get
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Reverse Connector ");
				}
				return minus;
			}
		}

		public bool isOpen
		{
			get
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Is Open ");
				}
				return false;
			}
		}

		public virtual float CalculateVoltage(float I)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Calculate Voltage ");
			}
			if (I > maxI)
			{
				return (0f - voltage) * maxI / I;
			}
			if (I < 0f)
			{
				return I / reverseR - voltage;
			}
			return 0f - voltage;
		}

		public virtual void RunCurrent(float I)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Run Current ");
			}
			current = I;
		}

		public virtual void StopCurrent()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Stop Current ");
			}
			current = 0f;
		}

		protected override void OnEnable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " OnEnable ");
			}
			base.OnEnable();
			plus.parent = (minus.parent = this);
			plus.isForward = true;
			minus.isForward = false;
		}
	}
}
