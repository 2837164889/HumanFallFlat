using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HumanAPI
{
	public abstract class Node : MonoBehaviour
	{
		public Color nodeColour = Color.white;

		[HideInInspector]
		public Vector2 pos = new Vector2(10f, 10f);

		private List<NodeSocket> allSockets;

		public static List<Node> all = new List<Node>();

		[HideInInspector]
		public NodePriority priority;

		[NonSerialized]
		public bool isDirty;

		public virtual string Title => GetType().Name;

		public List<NodeSocket> ListAllSockets()
		{
			if (allSockets == null)
			{
				RebuildSockets();
			}
			return allSockets;
		}

		public virtual List<NodeSocket> ListNodeSockets()
		{
			return ListAllSockets();
		}

		public void RebuildSockets()
		{
			allSockets = new List<NodeSocket>();
			CollectAllSockets(allSockets);
		}

		protected virtual void CollectAllSockets(List<NodeSocket> sockets)
		{
			CollectReflectionSockets<NodeInput>(sockets);
			CollectReflectionSockets<NodeOutput>(sockets);
		}

		public NodeOutput GetOutput(string name)
		{
			List<NodeSocket> list = ListAllSockets();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is NodeOutput && name.Equals(list[i].name))
				{
					return list[i] as NodeOutput;
				}
			}
			return null;
		}

		private void CollectReflectionSockets<T>(List<NodeSocket> sockets) where T : NodeSocket
		{
			Type type = GetType();
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				Type fieldType = fields[i].FieldType;
				if (fieldType == typeof(T))
				{
					NodeSocket nodeSocket = fields[i].GetValue(this) as NodeSocket;
					if (nodeSocket == null)
					{
						nodeSocket = (Activator.CreateInstance(fieldType) as NodeSocket);
						fields[i].SetValue(this, nodeSocket);
					}
					nodeSocket.name = fields[i].Name;
					nodeSocket.node = this;
					sockets.Add(nodeSocket);
				}
			}
		}

		protected virtual void OnEnable()
		{
			List<NodeSocket> list = ListAllSockets();
			for (int i = 0; i < list.Count; i++)
			{
				list[i].OnEnable();
			}
			all.Add(this);
		}

		protected virtual void OnDisable()
		{
			List<NodeSocket> list = ListAllSockets();
			for (int i = 0; i < list.Count; i++)
			{
				list[i].OnDisable();
			}
			all.Remove(this);
		}

		public void ResetOutputs()
		{
			List<NodeSocket> list = ListAllSockets();
			for (int i = 0; i < list.Count; i++)
			{
				NodeOutput nodeOutput = list[i] as NodeOutput;
				if (nodeOutput != null)
				{
					nodeOutput.value = nodeOutput.initialValue;
				}
			}
		}

		public void ResetInputs()
		{
			List<NodeSocket> list = ListAllSockets();
			for (int i = 0; i < list.Count; i++)
			{
				NodeInput nodeInput = list[i] as NodeInput;
				if (nodeInput != null)
				{
					nodeInput.value = (nodeInput.GetConnectedOutput()?.value ?? nodeInput.initialValue);
				}
			}
		}

		public virtual void Process()
		{
		}

		public virtual void Execute(NodeEntry entry)
		{
		}

		public void SetDirty()
		{
			if (!isDirty)
			{
				isDirty = true;
				SignalManager.AddDirtyNode(this);
			}
		}
	}
}
