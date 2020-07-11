using InControl;
using UnityEngine;

namespace VirtualDeviceExample
{
	public class VirtualDeviceExample : MonoBehaviour
	{
		public GameObject leftObject;

		public GameObject rightObject;

		private VirtualDevice virtualDevice;

		private void OnEnable()
		{
			virtualDevice = new VirtualDevice();
			InputManager.OnSetup += delegate
			{
				InputManager.AttachDevice(virtualDevice);
			};
		}

		private void OnDisable()
		{
			InputManager.DetachDevice(virtualDevice);
		}

		private void Update()
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			leftObject.transform.Rotate(Vector3.down, 500f * Time.deltaTime * (float)activeDevice.LeftStickX, Space.World);
			leftObject.transform.Rotate(Vector3.right, 500f * Time.deltaTime * (float)activeDevice.LeftStickY, Space.World);
			rightObject.transform.Rotate(Vector3.down, 500f * Time.deltaTime * (float)activeDevice.RightStickX, Space.World);
			rightObject.transform.Rotate(Vector3.right, 500f * Time.deltaTime * (float)activeDevice.RightStickY, Space.World);
			Color color = Color.white;
			if (activeDevice.Action1.IsPressed)
			{
				color = Color.green;
			}
			if (activeDevice.Action2.IsPressed)
			{
				color = Color.red;
			}
			if (activeDevice.Action3.IsPressed)
			{
				color = Color.blue;
			}
			if (activeDevice.Action4.IsPressed)
			{
				color = Color.yellow;
			}
			leftObject.GetComponent<Renderer>().material.color = color;
		}
	}
}
