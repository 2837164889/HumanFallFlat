using UnityEngine;

namespace HumanAPI
{
	public class SignalPlayAnimation : Node
	{
		public NodeInput input;

		private bool isOn;

		public string animationName;

		public override void Process()
		{
			base.Process();
			if (isOn != Mathf.Abs(input.value) >= 0.5f)
			{
				isOn = !isOn;
				if (isOn)
				{
					GetComponent<Animator>().Play(animationName);
				}
			}
		}
	}
}
