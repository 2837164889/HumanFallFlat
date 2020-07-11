using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class NodeGraph : Node
	{
		[HideInInspector]
		public Vector2 inputsPos = new Vector2(10f, 10f);

		[HideInInspector]
		public Vector2 outputsPos = new Vector2(10f, 10f);

		public List<NodeGraphInput> inputs = new List<NodeGraphInput>();

		public List<NodeGraphOutput> outputs = new List<NodeGraphOutput>();

		protected override void CollectAllSockets(List<NodeSocket> sockets)
		{
			for (int i = 0; i < inputs.Count; i++)
			{
				if (!string.IsNullOrEmpty(inputs[i].name))
				{
					inputs[i].input.name = (inputs[i].inputSocket.name = inputs[i].name);
					inputs[i].input.node = (inputs[i].inputSocket.node = this);
					sockets.Add(inputs[i].input);
					sockets.Add(inputs[i].inputSocket);
				}
			}
			for (int j = 0; j < outputs.Count; j++)
			{
				if (!string.IsNullOrEmpty(outputs[j].name))
				{
					outputs[j].output.name = (outputs[j].outputSocket.name = outputs[j].name);
					outputs[j].output.node = (outputs[j].outputSocket.node = this);
					sockets.Add(outputs[j].output);
					sockets.Add(outputs[j].outputSocket);
				}
			}
		}

		public override List<NodeSocket> ListNodeSockets()
		{
			List<NodeSocket> list = new List<NodeSocket>();
			for (int i = 0; i < inputs.Count; i++)
			{
				list.Add(inputs[i].input);
			}
			for (int j = 0; j < outputs.Count; j++)
			{
				list.Add(outputs[j].output);
			}
			return list;
		}

		public override void Process()
		{
			base.Process();
			for (int i = 0; i < inputs.Count; i++)
			{
				inputs[i].inputSocket.SetValue(inputs[i].input.value);
			}
			for (int j = 0; j < outputs.Count; j++)
			{
				outputs[j].output.SetValue(outputs[j].outputSocket.value);
			}
		}
	}
}
