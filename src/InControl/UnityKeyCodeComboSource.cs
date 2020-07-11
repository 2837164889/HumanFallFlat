using UnityEngine;

namespace InControl
{
	public class UnityKeyCodeComboSource : InputControlSource
	{
		public KeyCode[] KeyCodeList;

		public UnityKeyCodeComboSource()
		{
		}

		public UnityKeyCodeComboSource(params KeyCode[] keyCodeList)
		{
			KeyCodeList = keyCodeList;
		}

		public float GetValue(InputDevice inputDevice)
		{
			return (!GetState(inputDevice)) ? 0f : 1f;
		}

		public bool GetState(InputDevice inputDevice)
		{
			for (int i = 0; i < KeyCodeList.Length; i++)
			{
				if (!Game.GetKey(KeyCodeList[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
