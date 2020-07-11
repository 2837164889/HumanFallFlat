using InControl;
using UnityEngine;

namespace TouchExample
{
	public class CubeController : MonoBehaviour
	{
		private Renderer cachedRenderer;

		private void Start()
		{
			cachedRenderer = GetComponent<Renderer>();
		}

		private void Update()
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			if (activeDevice != InputDevice.Null && activeDevice != TouchManager.Device)
			{
				TouchManager.ControlsEnabled = false;
			}
			cachedRenderer.material.color = GetColorFromActionButtons(activeDevice);
			base.transform.Rotate(Vector3.down, 500f * Time.deltaTime * activeDevice.Direction.X, Space.World);
			base.transform.Rotate(Vector3.right, 500f * Time.deltaTime * activeDevice.Direction.Y, Space.World);
		}

		private Color GetColorFromActionButtons(InputDevice inputDevice)
		{
			if ((bool)inputDevice.Action1)
			{
				return Color.green;
			}
			if ((bool)inputDevice.Action2)
			{
				return Color.red;
			}
			if ((bool)inputDevice.Action3)
			{
				return Color.blue;
			}
			if ((bool)inputDevice.Action4)
			{
				return Color.yellow;
			}
			return Color.white;
		}

		private void OnGUI()
		{
			float num = 10f;
			int touchCount = TouchManager.TouchCount;
			for (int i = 0; i < touchCount; i++)
			{
				InControl.Touch touch = TouchManager.GetTouch(i);
				GUI.Label(new Rect(10f, num, 500f, num + 15f), string.Empty + i + ": fingerId = " + touch.fingerId + ", phase = " + touch.phase.ToString() + ", position = " + touch.position);
				num += 20f;
			}
		}
	}
}
