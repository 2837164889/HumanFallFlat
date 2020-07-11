using HumanAPI;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomizationCameraController : MonoBehaviour
{
	public Transform cameraArm;

	public Transform cameraHolder;

	public Transform bothCharactersCamera;

	public Transform fullCamera;

	public Transform fullCameraFrontal;

	public Transform fullModelCamera;

	public Transform headCamera;

	public Transform upperCamera;

	public Transform lowerCamera;

	private float transitionDuration = 0.5f;

	private float transitionPhase = 1f;

	private Vector3 targetFrom;

	private Vector3 targetTo;

	private Vector3 offsetFrom;

	private Vector3 offsetTo;

	private Quaternion rotateFrom;

	private Quaternion rotateTo;

	private int currentMode;

	public bool uiLock;

	public bool alt;

	public bool ctrl;

	public bool lmb;

	public bool rmb;

	public bool mmb;

	public bool isInGesture;

	public bool navigationEnabled = true;

	public void FocusBoth()
	{
		FocusTransform(bothCharactersCamera);
	}

	public void FocusCharacterFrontal()
	{
		FocusTransform(fullCameraFrontal);
	}

	public void FocusCharacter()
	{
		FocusTransform(fullCamera);
	}

	public void FocusCharacterModel()
	{
		FocusTransform(fullModelCamera);
	}

	public void FocusHead()
	{
		FocusTransform(headCamera);
	}

	public void FocusUpperBody()
	{
		FocusTransform(upperCamera);
	}

	public void FocusLowerBody()
	{
		FocusTransform(lowerCamera);
	}

	public void FocusPart(WorkshopItemType part)
	{
		switch (part)
		{
		case WorkshopItemType.ModelFull:
			FocusCharacterModel();
			break;
		case WorkshopItemType.ModelHead:
			FocusHead();
			break;
		case WorkshopItemType.ModelUpperBody:
			FocusUpperBody();
			break;
		case WorkshopItemType.ModelLowerBody:
			FocusLowerBody();
			break;
		}
	}

	private void FocusTransform(Transform target)
	{
		Transform child = target.GetChild(0);
		offsetFrom = cameraHolder.localPosition;
		offsetTo = child.localPosition;
		targetFrom = cameraArm.position;
		targetTo = target.position;
		rotateFrom = cameraArm.rotation;
		rotateTo = target.rotation;
		transitionPhase = 0f;
	}

	private void Update()
	{
		if (transitionPhase < 1f)
		{
			transitionPhase += Time.deltaTime / transitionDuration;
			float t = Ease.easeInOutQuad(0f, 1f, transitionPhase);
			Quaternion quaternion = Quaternion.Lerp(rotateFrom, rotateTo, t);
			cameraArm.rotation = quaternion;
			Quaternion quaternion2 = quaternion;
			cameraArm.position = Vector3.Lerp(targetFrom, targetTo, t);
			cameraHolder.localPosition = Vector3.Lerp(offsetFrom, offsetTo, t);
			return;
		}
		if (EventSystem.current != null)
		{
			if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() && (!ColorWheel.isInside || !ColorWheel.isTransparent))
			{
				uiLock = true;
			}
			if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
			{
				uiLock = (EventSystem.current.IsPointerOverGameObject() && (!ColorWheel.isInside || !ColorWheel.isTransparent));
			}
		}
		if (uiLock && Input.GetMouseButtonUp(0))
		{
			uiLock = false;
		}
		Vector3 eulerAngles = cameraArm.rotation.eulerAngles;
		float num = eulerAngles.x;
		Vector3 eulerAngles2 = cameraArm.rotation.eulerAngles;
		float num2 = eulerAngles2.y;
		Vector3 localPosition = cameraHolder.transform.localPosition;
		float num3 = localPosition.x;
		Vector3 localPosition2 = cameraHolder.transform.localPosition;
		float num4 = localPosition2.y;
		Vector3 localPosition3 = cameraHolder.transform.localPosition;
		float num5 = 0f - localPosition3.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		int num6 = 1;
		lmb = (Input.GetMouseButton(0) && !uiLock);
		rmb = Input.GetMouseButton(1);
		mmb = Input.GetMouseButton(2);
		alt = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
		ctrl = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl));
		isInGesture = (lmb || rmb || mmb);
		if (!uiLock && (navigationEnabled || alt))
		{
			Vector2 mouseScrollDelta = Input.mouseScrollDelta;
			float y = mouseScrollDelta.y;
			num5 *= Mathf.Pow(1.1f, 0f - y);
			num5 = Mathf.Clamp(num5, 0.2f, 5f);
		}
		if (navigationEnabled || alt || mmb || rmb)
		{
			float axis = Input.GetAxis("mouse x");
			float axis2 = Input.GetAxis("mouse y");
			if (axis != 0f || axis2 != 0f)
			{
				if (rmb)
				{
					float num7 = 0.2f;
					num -= axis2 * num7;
					num = Mathf.Clamp(num, -89f, 89f);
					num2 += axis * num7;
					if (num2 > 180f)
					{
						num2 -= 360f;
					}
					if (num2 < -180f)
					{
						num2 += 360f;
					}
				}
				if (mmb || ((navigationEnabled || alt) && lmb))
				{
					float num8 = 0.003f;
					num3 -= axis * num8 * num5;
					num4 -= axis2 * num8 * num5;
				}
				num5 = Mathf.Clamp(num5, 0.2f, 5f);
				float num9 = 1f + num5;
				num3 = Mathf.Clamp(num3, 0f - num9, num9);
				num4 = Mathf.Clamp(num4, 0f - (1.2f + num5 * 0.6f), 1f + num5 * 0.6f);
			}
		}
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (activeDevice.RightStick.Vector != Vector2.zero)
		{
			float num10 = 2f;
			float num11 = num;
			Vector2 vector = activeDevice.RightStick.Vector;
			num = num11 - vector.y * num10;
			num = Mathf.Clamp(num, -89f, 89f);
			float num12 = num2;
			Vector2 vector2 = activeDevice.RightStick.Vector;
			num2 = num12 + vector2.x * num10;
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			if (num2 < -180f)
			{
				num2 += 360f;
			}
		}
		float num13 = activeDevice.LeftTrigger.Value - activeDevice.RightTrigger.Value;
		if (num13 != 0f)
		{
			float num14 = -0.2f * num13;
			num5 *= Mathf.Pow(1.1f, 0f - num14);
			num5 = Mathf.Clamp(num5, 0.2f, 5f);
		}
		cameraArm.rotation = Quaternion.Euler(num, num2, 0f);
		cameraHolder.transform.localPosition = new Vector3(num3, num4, 0f - num5);
	}
}
