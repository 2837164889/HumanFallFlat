using InControl;
using UnityEngine;

namespace VirtualDeviceExample
{
	public class VirtualDevice : InputDevice
	{
		private const float sensitivity = 0.1f;

		private const float mouseScale = 0.05f;

		private float kx;

		private float ky;

		private float mx;

		private float my;

		public VirtualDevice()
			: base("Virtual Controller")
		{
			AddControl(InputControlType.LeftStickLeft, "Left Stick Left");
			AddControl(InputControlType.LeftStickRight, "Left Stick Right");
			AddControl(InputControlType.LeftStickUp, "Left Stick Up");
			AddControl(InputControlType.LeftStickDown, "Left Stick Down");
			AddControl(InputControlType.RightStickLeft, "Right Stick Left");
			AddControl(InputControlType.RightStickRight, "Right Stick Right");
			AddControl(InputControlType.RightStickUp, "Right Stick Up");
			AddControl(InputControlType.RightStickDown, "Right Stick Down");
			AddControl(InputControlType.Action1, "A");
			AddControl(InputControlType.Action2, "B");
			AddControl(InputControlType.Action3, "X");
			AddControl(InputControlType.Action4, "Y");
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			Vector2 vectorFromKeyboard = GetVectorFromKeyboard(deltaTime, smoothed: true);
			UpdateLeftStickWithValue(vectorFromKeyboard, updateTick, deltaTime);
			Vector2 vectorFromMouse = GetVectorFromMouse(deltaTime, smoothed: true);
			UpdateRightStickWithRawValue(vectorFromMouse, updateTick, deltaTime);
			UpdateWithState(InputControlType.Action1, Input.GetKey(KeyCode.Space), updateTick, deltaTime);
			UpdateWithState(InputControlType.Action2, Input.GetKey(KeyCode.S), updateTick, deltaTime);
			UpdateWithState(InputControlType.Action3, Input.GetKey(KeyCode.D), updateTick, deltaTime);
			UpdateWithState(InputControlType.Action4, Input.GetKey(KeyCode.F), updateTick, deltaTime);
			Commit(updateTick, deltaTime);
		}

		private Vector2 GetVectorFromKeyboard(float deltaTime, bool smoothed)
		{
			if (smoothed)
			{
				kx = ApplySmoothing(kx, GetXFromKeyboard(), deltaTime, 0.1f);
				ky = ApplySmoothing(ky, GetYFromKeyboard(), deltaTime, 0.1f);
			}
			else
			{
				kx = GetXFromKeyboard();
				ky = GetYFromKeyboard();
			}
			return new Vector2(kx, ky);
		}

		private float GetXFromKeyboard()
		{
			float num = (!Input.GetKey(KeyCode.LeftArrow)) ? 0f : (-1f);
			float num2 = (!Input.GetKey(KeyCode.RightArrow)) ? 0f : 1f;
			return num + num2;
		}

		private float GetYFromKeyboard()
		{
			float num = (!Input.GetKey(KeyCode.UpArrow)) ? 0f : 1f;
			float num2 = (!Input.GetKey(KeyCode.DownArrow)) ? 0f : (-1f);
			return num + num2;
		}

		private Vector2 GetVectorFromMouse(float deltaTime, bool smoothed)
		{
			if (smoothed)
			{
				mx = ApplySmoothing(mx, Input.GetAxisRaw("mouse x") * 0.05f, deltaTime, 0.1f);
				my = ApplySmoothing(my, Input.GetAxisRaw("mouse y") * 0.05f, deltaTime, 0.1f);
			}
			else
			{
				mx = Input.GetAxisRaw("mouse x") * 0.05f;
				my = Input.GetAxisRaw("mouse y") * 0.05f;
			}
			return new Vector2(mx, my);
		}

		private float ApplySmoothing(float lastValue, float thisValue, float deltaTime, float sensitivity)
		{
			sensitivity = Mathf.Clamp(sensitivity, 0.001f, 1f);
			if (Mathf.Approximately(sensitivity, 1f))
			{
				return thisValue;
			}
			return Mathf.Lerp(lastValue, thisValue, deltaTime * sensitivity * 100f);
		}
	}
}
