using System;

namespace HumanAPI
{
	[Serializable]
	public class NodeOutput : NodeSocket
	{
		[NonSerialized]
		public float value;

		public float initialValue;

		public event Action<float> onValueChanged;

		public void SetValue(float value)
		{
			if (this.value != value)
			{
				this.value = value;
				if (this.onValueChanged != null)
				{
					this.onValueChanged(value);
				}
			}
		}

		public override void OnEnable()
		{
		}

		public override void OnDisable()
		{
		}
	}
}
