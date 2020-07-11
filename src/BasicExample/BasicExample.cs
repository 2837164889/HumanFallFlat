using InControl;
using UnityEngine;

namespace BasicExample
{
	public class BasicExample : MonoBehaviour
	{
		private void Update()
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			base.transform.Rotate(Vector3.down, 500f * Time.deltaTime * (float)activeDevice.LeftStickX, Space.World);
			base.transform.Rotate(Vector3.right, 500f * Time.deltaTime * (float)activeDevice.LeftStickY, Space.World);
			Color a = (!activeDevice.Action1.IsPressed) ? Color.white : Color.red;
			Color b = (!activeDevice.Action2.IsPressed) ? Color.white : Color.green;
			GetComponent<Renderer>().material.color = Color.Lerp(a, b, 0.5f);
		}
	}
}
