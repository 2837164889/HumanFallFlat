using UnityEngine;

namespace HumanAPI
{
	public class OtherCollisionSensor : Node
	{
		public NodeOutput output;

		private void OnCollisionEnter(Collision collision)
		{
			output.SetValue(1f);
		}

		private void OnCollisionStay(Collision collision)
		{
			output.SetValue(1f);
		}

		private void OnCollisionExit(Collision collision)
		{
			output.SetValue(0f);
		}
	}
}
