using System;

namespace HumanAPI
{
	[Serializable]
	public class NodeInput : NodeSocket
	{
		public Node connectedNode;

		public string connectedSocket;

		[NonSerialized]
		public float value;

		public float initialValue;

		public virtual bool CanConnect(NodeOutput output)
		{
			return !(output is NodeEntry);
		}

		public NodeOutput GetConnectedOutput()
		{
			if (connectedNode == null)
			{
				return null;
			}
			return connectedNode.GetOutput(connectedSocket);
		}

		public void Connect(NodeOutput output)
		{
			if (output == null)
			{
				connectedNode = null;
				connectedSocket = null;
			}
			else
			{
				connectedNode = output.node;
				connectedSocket = output.name;
			}
		}

		public override void OnEnable()
		{
			NodeOutput connectedOutput = GetConnectedOutput();
			if (connectedOutput != null)
			{
				connectedOutput.onValueChanged += ConnectedOutput_onValueChanged;
			}
		}

		public override void OnDisable()
		{
			NodeOutput connectedOutput = GetConnectedOutput();
			if (connectedOutput != null)
			{
				connectedOutput.onValueChanged -= ConnectedOutput_onValueChanged;
			}
		}

		private void ConnectedOutput_onValueChanged(float value)
		{
			if (this.value != value)
			{
				this.value = value;
				node.SetDirty();
			}
		}
	}
}
