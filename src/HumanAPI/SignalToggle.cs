using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	public class SignalToggle : Node
	{
		public NodeInput[] inputs;

		public NodeOutput output;

		private bool[] oldValues;

		protected override void CollectAllSockets(List<NodeSocket> sockets)
		{
			oldValues = new bool[inputs.Length];
			for (int i = 0; i < inputs.Length; i++)
			{
				inputs[i].node = this;
				inputs[i].name = "input" + i.ToString();
				sockets.Add(inputs[i]);
				oldValues[i] = (Mathf.Abs(inputs[i].value) >= 0.5f);
			}
			output.name = "output";
			output.node = this;
			sockets.Add(output);
		}

		public override void Process()
		{
			base.Process();
			for (int i = 0; i < inputs.Length; i++)
			{
				bool flag = Mathf.Abs(inputs[i].value) >= 0.5f;
				if (flag != oldValues[i])
				{
					oldValues[i] = flag;
					if (flag)
					{
						output.SetValue(1f - output.value);
					}
				}
			}
		}
	}
}
