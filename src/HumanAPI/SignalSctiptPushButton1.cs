using System;
using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Button", 10)]
	public class SignalSctiptPushButton1 : Node
	{
		[Tooltip("Reference to the joint the button will use when moving in and out")]
		public ConfigurableJoint slideJoint;

		private Rigidbody slideJointRigidBody;

		[Tooltip("The amount of force needed to press the button")]
		public float buttonForce = 100f;

		[Tooltip("The speed at which the button should move when pressed")]
		public float buttonSpeed = 0.25f;

		[Tooltip("a limit to the amount of motion the button will have when pressed ")]
		public float motionRange = 0.05f;

		[Tooltip("The mode the button is in , defines how the player interacts with it")]
		public ButtonActivationMode1 activationMode;

		[Tooltip("The action the button will perform when pressed")]
		public ButtonAction1 ButtonAction1;

		[Tooltip("Whether or not the button is positive when pressed")]
		public bool trueWhenPressed = true;

		[Tooltip("Input coming into the button from somewhere else")]
		public NodeInput input;

		[Tooltip("Input controlling the resetting of the buttons state")]
		public NodeInput reset;

		[Tooltip("The output from the button when pressed ")]
		public NodeOutput value;

		private Transform buttonTransform;

		private bool inputState;

		private bool pressedState;

		private bool currentlightOn;

		private bool previoudsLightOn;

		[Tooltip("Set to true to see the debug output from this button")]
		public bool showDebug;

		[Tooltip("Whether or not the button should use the action assigned")]
		public bool useButtonAction1;

		private bool grabbed;

		private float bypassTimer;

		private bool isDown;

		private float bounceBackTimer;

		protected void Awake()
		{
			if (slideJoint == null)
			{
				slideJoint = GetComponent<ConfigurableJoint>();
			}
			if (slideJoint != null && slideJointRigidBody == null)
			{
				slideJointRigidBody = slideJoint.GetComponent<Rigidbody>();
			}
			buttonTransform = slideJoint.transform;
			slideJoint.SetXMotionAnchorsAndLimits(motionRange / 2f, motionRange);
			SetForce(buttonForce);
			inputState = (Mathf.Abs(input.value) >= 0.5f);
			Snap(inputState);
		}

		private void SetForce(float buttonForce)
		{
			JointDrive xDrive = slideJoint.xDrive;
			xDrive.maximumForce = buttonForce;
			xDrive.positionSpring = buttonForce / motionRange;
			xDrive.positionDamper = buttonForce / buttonSpeed;
			slideJoint.xDrive = xDrive;
		}

		public override void Process()
		{
			base.Process();
			bool flag = Mathf.Abs(input.value) >= 0.5f;
			if (inputState != flag)
			{
				inputState = flag;
				Snap(inputState);
			}
			if (reset.value > 0.5f)
			{
				ResetButton();
			}
		}

		private void OnPress()
		{
			if (!useButtonAction1)
			{
				return;
			}
			switch (ButtonAction1)
			{
			case ButtonAction1.Hold:
				if (showDebug)
				{
					Debug.Log(base.name + " Hold Action setting value to 1 ");
				}
				value.SetValue(1f);
				if (showDebug)
				{
					Debug.Log(base.name + " doing the hold actions ");
				}
				PressedSettings();
				break;
			case ButtonAction1.Press:
				if (showDebug)
				{
					Debug.Log(base.name + " Doing the press actions ");
				}
				PressedSettings();
				break;
			case ButtonAction1.Toggle:
				if (showDebug)
				{
					Debug.Log(base.name + " Doing the toggle actions ");
				}
				currentlightOn = true;
				PressedSettings();
				break;
			default:
				throw new InvalidOperationException();
			}
		}

		private void OnRelease()
		{
			if (!useButtonAction1)
			{
				return;
			}
			switch (ButtonAction1)
			{
			case ButtonAction1.Hold:
				value.SetValue(0f);
				if (showDebug)
				{
					Debug.Log(base.name + " Releasing with a Hold action ");
				}
				ReleaseSettings();
				break;
			case ButtonAction1.Press:
				if (showDebug)
				{
					Debug.Log(base.name + " Releasing with a press action  ");
				}
				pressedState = false;
				Snap(pressedState);
				value.SetValue(1f);
				break;
			case ButtonAction1.Toggle:
				if (showDebug)
				{
					Debug.Log(base.name + " Releasing with a toggle action  ");
				}
				if ((double)input.value > 0.5)
				{
					if (showDebug)
					{
						Debug.Log(base.name + " Input Value is 1 ");
					}
					if (showDebug)
					{
						Debug.Log(base.name + " toggle release setting value to 1 ");
					}
					value.SetValue(1f);
					break;
				}
				if (showDebug)
				{
					Debug.Log(base.name + " Input value is 0  ");
				}
				if (showDebug)
				{
					Debug.Log(base.name + " Toggle OnRelease value = " + value.value);
				}
				if (currentlightOn)
				{
					value.SetValue(1f);
					if (!previoudsLightOn)
					{
						value.SetValue(1f);
						previoudsLightOn = true;
					}
					else
					{
						value.SetValue(0f);
						previoudsLightOn = false;
					}
				}
				pressedState = false;
				Snap(pressedState);
				break;
			default:
				throw new InvalidOperationException();
			}
		}

		private void ReleaseSettings()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Release Settings ");
			}
			if (showDebug)
			{
				Debug.Log(base.name + "setting the value to 0");
			}
			pressedState = false;
			Snap(pressedState);
			value.SetValue(0f);
		}

		private void PressedSettings()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Pressed Settings ");
			}
			if (showDebug)
			{
				Debug.Log(base.name + " Setting the value to 1 ");
			}
			pressedState = true;
			Snap(pressedState);
			value.SetValue(1f);
		}

		public void ResetButton()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Resetting this button ");
			}
			switch (ButtonAction1)
			{
			case ButtonAction1.Press:
				pressedState = false;
				if (showDebug)
				{
					Debug.Log(base.name + " Reset button setting the value to 0 ");
				}
				value.SetValue(0f);
				break;
			case ButtonAction1.Toggle:
				ReleaseSettings();
				break;
			}
			reset.value = 0f;
		}

		private void Snap(bool pressed)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Snap  Pressed State " + pressed);
			}
			if (activationMode == ButtonActivationMode1.Touch)
			{
				slideJoint.targetPosition = new Vector3((!pressed) ? motionRange : (0f - motionRange), 0f, 0f);
			}
			if (slideJointRigidBody != null)
			{
				slideJointRigidBody.WakeUp();
			}
			bypassTimer = 0.1f;
			if (SignalManager.skipTransitions)
			{
				slideJoint.ApplyXMotionTarget();
			}
		}

		public void Update()
		{
			float num = (activationMode == ButtonActivationMode1.Touch) ? (-1f) : CalculateNormalizedOffset();
			bool flag = num > 0.5f;
			bool flag2 = num < 0.25f;
			bool flag3 = activationMode != 0 && CalculateGrabbed();
			if (flag && ButtonAction1 == ButtonAction1.Hold && slideJointRigidBody != null && slideJointRigidBody.IsSleeping())
			{
				slideJointRigidBody.WakeUp();
			}
			if (!pressedState && (flag || flag3))
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Update - Pressed state should be true ");
				}
				if (useButtonAction1)
				{
					OnPress();
				}
				else
				{
					PressedSettings();
				}
			}
			else if (pressedState && flag2 && !flag3)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " Update -Pressed state should be false ");
				}
				if (useButtonAction1 && pressedState)
				{
					if (showDebug)
					{
						Debug.Log(base.name + " Update - using button action ");
					}
					OnRelease();
				}
				else if (!useButtonAction1 && pressedState)
				{
					if (showDebug)
					{
						Debug.Log(base.name + " Update - not using button action ");
					}
					ReleaseSettings();
				}
			}
			if (flag3 && activationMode != 0)
			{
				SetForce(buttonForce + 2000f);
			}
			else
			{
				SetForce(buttonForce);
			}
		}

		private bool CalculateGrabbed()
		{
			return GrabManager.IsGrabbedAny(buttonTransform.gameObject);
		}

		private float CalculateNormalizedOffset()
		{
			Vector3 rhs = buttonTransform.TransformDirection(slideJoint.axis);
			Vector3 a = buttonTransform.TransformPoint(slideJoint.anchor);
			Vector3 b = slideJoint.connectedBody.transform.TransformPoint(slideJoint.connectedAnchor);
			float num = Vector3.Dot(a - b, rhs);
			return num / motionRange + 0.5f;
		}
	}
}
