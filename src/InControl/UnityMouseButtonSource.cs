using UnityEngine;

namespace InControl
{
	public class UnityMouseButtonSource : InputControlSource
	{
		public int ButtonId;

		public UnityMouseButtonSource()
		{
		}

		public UnityMouseButtonSource(int buttonId)
		{
			ButtonId = buttonId;
		}

		public float GetValue(InputDevice inputDevice)
		{
			return (!GetState(inputDevice)) ? 0f : 1f;
		}

		public bool GetState(InputDevice inputDevice)
		{
			return Input.GetMouseButton(ButtonId);
		}
	}
}
