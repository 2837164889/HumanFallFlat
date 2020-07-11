using UnityEngine;

namespace HumanAPI
{
	public class SignalLatch : Node, IReset
	{
		public NodeInput input;

		public NodeInput latch;

		public NodeOutput output;

		private bool latched;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		public override void Process()
		{
			base.Process();
			if (latched)
			{
				return;
			}
			output.SetValue(input.value);
			if (showDebug)
			{
				Debug.Log(base.name + " Latch Setting " + input.value);
			}
			if (latch.value > 0.5f)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Latch True ");
				}
				latched = true;
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset State ");
			}
			latched = false;
		}
	}
}
