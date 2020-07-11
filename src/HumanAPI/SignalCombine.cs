using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	[AddNodeMenuItem]
	[AddComponentMenu("Human/Signals/SignalCombine", 10)]
	public class SignalCombine : Node
	{
		public SignalBoolOperation operation = SignalBoolOperation.And;

		public NodeInput[] inputs;

		public NodeOutput output;

		public bool invert;

		public override string Title => "Combine: " + operation;

		protected override void CollectAllSockets(List<NodeSocket> sockets)
		{
			if (inputs == null || inputs.Length == 0)
			{
				Debug.LogError("Combine node has no inputs!");
			}
			else
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					inputs[i].node = this;
					inputs[i].name = "input" + i.ToString();
					sockets.Add(inputs[i]);
				}
			}
			output.name = "output";
			output.node = this;
			sockets.Add(output);
		}

		public override void Process()
		{
			float num = 0f;
			switch (operation)
			{
			case SignalBoolOperation.And:
			{
				num = 1f;
				for (int n = 0; n < inputs.Length; n++)
				{
					if (Mathf.Abs(inputs[n].value) < 0.5f)
					{
						num = 0f;
					}
				}
				break;
			}
			case SignalBoolOperation.Or:
			{
				num = 0f;
				for (int j = 0; j < inputs.Length; j++)
				{
					if (Mathf.Abs(inputs[j].value) >= 0.5f)
					{
						num = 1f;
					}
				}
				break;
			}
			case SignalBoolOperation.Xor:
			{
				num = 0f;
				for (int l = 0; l < inputs.Length; l++)
				{
					if (Mathf.Abs(inputs[l].value) >= 0.5f)
					{
						num = 1f - num;
					}
				}
				break;
			}
			case SignalBoolOperation.Sum:
			{
				num = 0f;
				for (int num2 = 0; num2 < inputs.Length; num2++)
				{
					num += inputs[num2].value;
				}
				break;
			}
			case SignalBoolOperation.Mul:
			{
				num = 1f;
				for (int m = 0; m < inputs.Length; m++)
				{
					num *= inputs[m].value;
				}
				break;
			}
			case SignalBoolOperation.Max:
			{
				num = float.NegativeInfinity;
				for (int k = 0; k < inputs.Length; k++)
				{
					num = Mathf.Max(num, inputs[k].value);
				}
				break;
			}
			case SignalBoolOperation.Min:
			{
				num = float.PositiveInfinity;
				for (int i = 0; i < inputs.Length; i++)
				{
					num = Mathf.Min(num, inputs[i].value);
				}
				break;
			}
			}
			if (invert)
			{
				output.SetValue(1f - num);
			}
			else
			{
				output.SetValue(num);
			}
		}
	}
}
