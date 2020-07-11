using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class SignalCycler : Node, IReset
	{
		public NodeInput input;

		private const int defaultOutputs = 3;

		public NodeOutput[] outputs = new NodeOutput[3];

		[Tooltip("If true, the incoming signal must go back to zero before another increment can happen")]
		public bool requireSignalResetToIncrement = true;

		private bool currentlyActive;

		[Tooltip("Starting value, will immediately trigger selected value. Leave at 0 to have no starting value (Will go to output 1 once triggered)")]
		public int startingValue;

		private int currentOutput;

		protected override void CollectAllSockets(List<NodeSocket> sockets)
		{
			input.name = "Input";
			input.node = this;
			sockets.Add(input);
			for (int i = 0; i < outputs.Length; i++)
			{
				outputs[i].node = this;
				outputs[i].name = "Output " + (i + 1).ToString();
				sockets.Add(outputs[i]);
			}
		}

		public override void Process()
		{
			base.Process();
			bool flag = false;
			if (!requireSignalResetToIncrement)
			{
				flag = true;
			}
			else if (!currentlyActive)
			{
				flag = true;
			}
			else if (input.value <= 0.5f)
			{
				currentlyActive = false;
			}
			if (flag && input.value > 0.5f)
			{
				if (requireSignalResetToIncrement)
				{
					currentlyActive = true;
				}
				currentOutput++;
				if (currentOutput >= outputs.Length)
				{
					currentOutput = 0;
				}
				UpdateOutputs();
			}
		}

		private void UpdateOutputs()
		{
			for (int i = 0; i < outputs.Length; i++)
			{
				outputs[i].SetValue((currentOutput != i) ? 0f : 1f);
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			currentOutput = startingValue - 1;
			currentlyActive = false;
			UpdateOutputs();
		}
	}
}
