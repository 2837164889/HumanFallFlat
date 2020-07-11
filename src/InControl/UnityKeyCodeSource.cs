using UnityEngine;

namespace InControl
{
	public class UnityKeyCodeSource : InputControlSource
	{
		public KeyCode[] KeyCodeList;

		public UnityKeyCodeSource()
		{
		}

		public UnityKeyCodeSource(params KeyCode[] keyCodeList)
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
				if (Game.GetKey(KeyCodeList[i]))
				{
					return true;
				}
			}
			return false;
		}
	}
}
