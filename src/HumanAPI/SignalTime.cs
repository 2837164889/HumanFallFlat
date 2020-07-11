using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/SignalTime", 10)]
	public class SignalTime : Node, IReset
	{
		public NodeOutput output;

		public NodeOutput maxValueReached;

		public NodeInput input;

		public bool startTimerOnSignal;

		public bool resetTimerOnSignalOff;

		public float maxValue;

		public bool resetAtMaxValue;

		[Tooltip("If resetAtMaxValue is true and startTimerOnSignal is false, how long to hold maxValueReached at 1.0 after timer loops.")]
		public float holdTime = 0.25f;

		private float prevInput;

		private bool timerRunning;

		private float initialOutput;

		public override string Title => "Time: Max " + maxValue;

		protected override void OnEnable()
		{
			base.OnEnable();
			initialOutput = output.value;
			if (startTimerOnSignal)
			{
				timerRunning = false;
			}
			else
			{
				timerRunning = true;
			}
		}

		public override void Process()
		{
			base.Process();
			if (startTimerOnSignal && input.value >= 0.5f && prevInput < 0.5f)
			{
				timerRunning = true;
			}
			if (resetTimerOnSignalOff && input.value < 0.5f && prevInput >= 0.5f)
			{
				if (startTimerOnSignal)
				{
					timerRunning = false;
				}
				else
				{
					timerRunning = true;
				}
				output.SetValue(0f);
				maxValueReached.SetValue(0f);
			}
			prevInput = input.value;
		}

		private void FixedUpdate()
		{
			if (!timerRunning)
			{
				return;
			}
			float num = output.value + Time.fixedDeltaTime;
			if (maxValue > 0f && num >= maxValue)
			{
				if (resetAtMaxValue)
				{
					if (startTimerOnSignal)
					{
						timerRunning = false;
					}
					else
					{
						timerRunning = true;
					}
					num = 0f;
				}
				else
				{
					num = maxValue;
				}
				maxValueReached.SetValue(1f);
			}
			else if (num >= holdTime || startTimerOnSignal || !resetAtMaxValue)
			{
				maxValueReached.SetValue(0f);
			}
			output.SetValue(num);
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			output.SetValue(initialOutput);
			maxValueReached.SetValue(0f);
			if (startTimerOnSignal)
			{
				timerRunning = false;
			}
			else
			{
				timerRunning = true;
			}
		}
	}
}
