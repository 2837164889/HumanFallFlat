using CurveGame;
using InControl;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanControls : MonoBehaviour
{
	private PlayerActions controllerBindings;

	[NonSerialized]
	public InputDevice device;

	public bool allowMouse;

	private const InputAction leftStickX = InputAction.HorizontalAxis0;

	private const InputAction leftStickY = InputAction.VerticalAxis0;

	public static bool freezeMouse;

	private Vector2 keyLookCache;

	public float leftExtend;

	public float rightExtend;

	private float leftExtendCache;

	private float rightExtendCache;

	public bool leftGrab;

	public bool rightGrab;

	public bool unconscious;

	public bool holding;

	public bool jump;

	public bool shootingFirework;

	public VerticalLookMode verticalLookMode;

	public MouseLookMode mouseLookMode;

	public bool mouseControl = true;

	public float cameraPitchAngle;

	public float cameraYawAngle;

	public float targetPitchAngle;

	public float targetYawAngle;

	public Vector3 walkLocalDirection;

	public Vector3 walkDirection;

	public float unsmoothedWalkSpeed;

	public float walkSpeed;

	private List<float> mouseInputX = new List<float>();

	private List<float> mouseInputY = new List<float>();

	private Vector3 stickDirection;

	private Vector3 oldStickDirection;

	private float previousLeftExtend;

	private float previousRightExtend;

	private float shootCooldown;

	public Vector2 calc_joyLook
	{
		get
		{
			if (controllerBindings != null)
			{
				Vector2 result = new Vector2(controllerBindings.LookX, controllerBindings.LookY);
				if (Options.controllerInvert > 0)
				{
					result.y *= -1f;
				}
				float a = 0.1f + Mathf.Abs(result.y) * 0.15f;
				if (result.x < 0f)
				{
					result.x = 0f - Mathf.InverseLerp(a, 1f, 0f - result.x);
				}
				else
				{
					result.x = Mathf.InverseLerp(a, 1f, result.x);
				}
				result.x *= controllerBindings.HScale;
				result.y *= controllerBindings.VScale;
				return result;
			}
			return Vector2.zero;
		}
	}

	public Vector2 calc_keyLook
	{
		get
		{
			if (allowMouse)
			{
				if (!freezeMouse)
				{
					keyLookCache = new Vector2(Options.keyboardBindings.LookX, Options.keyboardBindings.LookY);
				}
				return keyLookCache;
			}
			return Vector2.zero;
		}
	}

	public Vector3 calc_joyWalk
	{
		get
		{
			if (controllerBindings != null)
			{
				Vector3 result = new Vector3(controllerBindings.Move.X, 0f, controllerBindings.Move.Y);
				float num = 0.1f + Mathf.Abs(result.y) * 0.1f;
				if (Mathf.Abs(result.x) < num)
				{
					result.x = 0f;
				}
				return result;
			}
			return Vector3.zero;
		}
	}

	public Vector3 calc_keyWalk
	{
		get
		{
			if (allowMouse)
			{
				return new Vector3(Options.keyboardBindings.Move.X, 0f, Options.keyboardBindings.Move.Y);
			}
			return Vector3.zero;
		}
	}

	public float lookYnormalized => (controllerBindings == null) ? 0f : controllerBindings.LookYNormalized;

	public float vScale => (controllerBindings == null) ? 0f : controllerBindings.VScale;

	private void OnEnable()
	{
		SetAny();
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
	}

	private float GetLeftExtend()
	{
		if (!freezeMouse)
		{
			leftExtendCache = ((!allowMouse) ? 0f : Options.keyboardBindings.LeftHand.Value);
		}
		return Mathf.Max((controllerBindings == null) ? 0f : controllerBindings.LeftHand.Value, leftExtendCache);
	}

	private float GetRightExtend()
	{
		if (!freezeMouse)
		{
			rightExtendCache = ((!allowMouse) ? 0f : Options.keyboardBindings.RightHand.Value);
		}
		return Mathf.Max((controllerBindings == null) ? 0f : controllerBindings.RightHand.Value, rightExtendCache);
	}

	private bool GetUnconscious()
	{
		return (controllerBindings != null && controllerBindings.Unconscious.IsPressed) || (allowMouse && Options.keyboardBindings.Unconscious.IsPressed);
	}

	public bool ControllerJumpPressed()
	{
		return controllerBindings != null && controllerBindings.Jump.IsPressed;
	}

	public bool ControllerFireworksPressed()
	{
		return controllerBindings != null && controllerBindings.ShootFireworks.IsPressed;
	}

	private bool GetJump()
	{
		return ControllerJumpPressed() || (allowMouse && Options.keyboardBindings.Jump.IsPressed);
	}

	private bool GetFireworks()
	{
		return ControllerFireworksPressed() || (allowMouse && Options.keyboardBindings.ShootFireworks.IsPressed);
	}

	public void ReadInput(out float walkForward, out float walkRight, out float cameraPitch, out float cameraYaw, out float leftExtend, out float rightExtend, out bool jump, out bool playDead, out bool shooting)
	{
		Vector2 calc_joyLook = this.calc_joyLook;
		Vector2 calc_keyLook = this.calc_keyLook;
		Vector3 calc_joyWalk = this.calc_joyWalk;
		Vector3 calc_keyWalk = this.calc_keyWalk;
		if (calc_joyLook.sqrMagnitude > calc_keyLook.sqrMagnitude)
		{
			mouseControl = false;
		}
		if (calc_keyLook.sqrMagnitude > calc_joyLook.sqrMagnitude)
		{
			mouseControl = true;
		}
		if (calc_joyWalk.sqrMagnitude > calc_keyWalk.sqrMagnitude)
		{
			mouseControl = false;
		}
		if (calc_keyWalk.sqrMagnitude > calc_joyWalk.sqrMagnitude)
		{
			mouseControl = true;
		}
		cameraPitch = cameraPitchAngle;
		cameraYaw = cameraYawAngle;
		if (mouseControl)
		{
			cameraYaw += Smoothing.SmoothValue(mouseInputX, calc_keyLook.x);
			cameraPitch -= Smoothing.SmoothValue(mouseInputY, calc_keyLook.y);
			cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
			if (mouseLookMode == MouseLookMode.SpringBackNoGrab && !leftGrab && !rightGrab)
			{
				int num = 0;
				float num2 = 0.25f;
				cameraPitch = Mathf.Lerp(cameraPitch, num, num2 * 5f * Time.fixedDeltaTime);
				cameraPitch = Mathf.MoveTowards(cameraPitch, num, num2 * 30f * Time.fixedDeltaTime);
			}
		}
		else
		{
			cameraYaw += calc_joyLook.x * Time.deltaTime * 120f;
			if (verticalLookMode == VerticalLookMode.Relative)
			{
				if (Options.controllerInvert > 0)
				{
					calc_joyLook.y = 0f - calc_joyLook.y;
				}
				cameraPitch -= calc_joyLook.y * Time.deltaTime * 120f * 2f;
				cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
			}
			else
			{
				float num3 = -80f * lookYnormalized;
				bool flag = num3 * cameraPitch < 0f || Mathf.Abs(num3) > Mathf.Abs(cameraPitch);
				float num4 = (!leftGrab && !rightGrab) ? 1f : Mathf.Abs(lookYnormalized);
				float num5 = (!leftGrab && !rightGrab) ? 0.25f : 0.0125f;
				float num6 = (!flag) ? num5 : num4;
				cameraPitch = Mathf.Lerp(cameraPitch, num3, num6 * 5f * Time.fixedDeltaTime * vScale);
				cameraPitch = Mathf.MoveTowards(cameraPitch, num3, num6 * 30f * Time.fixedDeltaTime * vScale);
			}
		}
		Vector3 vector = (!(calc_joyWalk.sqrMagnitude > calc_keyWalk.sqrMagnitude)) ? calc_keyWalk : calc_joyWalk;
		walkForward = vector.z;
		walkRight = vector.x;
		if (MenuSystem.keyboardState == KeyboardState.None)
		{
			leftExtend = GetLeftExtend();
			rightExtend = GetRightExtend();
			jump = GetJump();
			playDead = GetUnconscious();
			previousLeftExtend = leftExtend;
			previousRightExtend = rightExtend;
			shooting = GetFireworks();
		}
		else
		{
			leftExtend = previousLeftExtend;
			rightExtend = previousRightExtend;
			jump = false;
			playDead = false;
			shooting = false;
		}
	}

	public void HandleInput(float walkForward, float walkRight, float cameraPitch, float cameraYaw, float leftExtend, float rightExtend, bool jump, bool playDead, bool holding, bool shooting)
	{
		walkLocalDirection = new Vector3(walkRight, 0f, walkForward);
		cameraPitchAngle = cameraPitch;
		cameraYawAngle = cameraYaw;
		this.leftExtend = leftExtend;
		this.rightExtend = rightExtend;
		leftGrab = (leftExtend > 0f);
		rightGrab = (rightExtend > 0f);
		this.jump = jump;
		unconscious = playDead;
		this.holding = holding;
		shootingFirework = shooting;
		targetPitchAngle = Mathf.MoveTowards(targetPitchAngle, cameraPitchAngle, 180f * Time.fixedDeltaTime / 0.1f);
		targetYawAngle = cameraYawAngle;
		Quaternion rotation = Quaternion.Euler(0f, cameraYawAngle, 0f);
		Vector3 vector = rotation * walkLocalDirection;
		unsmoothedWalkSpeed = vector.magnitude;
		vector = new Vector3(FilterAxisAcceleration(oldStickDirection.x, vector.x), 0f, FilterAxisAcceleration(oldStickDirection.z, vector.z));
		walkSpeed = vector.magnitude;
		if (walkSpeed > 0f)
		{
			walkDirection = vector;
		}
		oldStickDirection = vector;
	}

	private float FilterAxisAcceleration(float currentValue, float desiredValue)
	{
		float num = Time.fixedDeltaTime / 1f;
		float num2 = 0.2f;
		if (currentValue * desiredValue <= 0f)
		{
			currentValue = 0f;
		}
		if (Mathf.Abs(currentValue) > Mathf.Abs(desiredValue))
		{
			currentValue = desiredValue;
		}
		if (Mathf.Abs(currentValue) < num2)
		{
			num = Mathf.Max(num, num2 - Mathf.Abs(currentValue));
		}
		if (Mathf.Abs(currentValue) > 0.8f)
		{
			num /= 3f;
		}
		return Mathf.MoveTowards(currentValue, desiredValue, num);
	}

	public void SetDevice(InputDevice inputDevice)
	{
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		device = inputDevice;
		controllerBindings = Options.CreateInput(inputDevice);
		allowMouse = false;
	}

	public void SetController()
	{
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		device = null;
		controllerBindings = Options.controllerBindings;
		allowMouse = false;
	}

	public void SetMouse()
	{
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		device = null;
		controllerBindings = null;
		allowMouse = true;
	}

	public void SetAny()
	{
		verticalLookMode = (VerticalLookMode)Options.controllerLookMode;
		mouseLookMode = (MouseLookMode)Options.mouseLookMode;
		device = null;
		controllerBindings = Options.controllerBindings;
		allowMouse = true;
	}
}
