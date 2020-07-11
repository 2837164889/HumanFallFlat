using Multiplayer;
using System;
using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("Human/Button", 10)]
	public class Button : Node
	{
		public ConfigurableJoint slideJoint;

		private Rigidbody slideJointRigidBody;

		public float buttonForce = 100f;

		public float buttonSpeed = 0.25f;

		public float motionRange = 0.05f;

		public ButtonActivationMode activationMode;

		public ButtonAction buttonAction;

		public bool trueWhenPressed = true;

		public NodeInput input;

		public NodeInput reset;

		public NodeOutput value;

		private Transform buttonTransform;

		private bool inputState;

		private bool pressedState;

		private bool lightOn;

		[Tooltip("Set to true to see the debug output from this button")]
		public bool showdebug;

		[Tooltip("Whether or not the button should use the action assigned")]
		public bool usebuttonaction;

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
			if (!usebuttonaction)
			{
				return;
			}
			switch (buttonAction)
			{
			case ButtonAction.Hold:
				value.SetValue(1f);
				if (showdebug)
				{
					Debug.Log(base.name + " doing the hold actions ");
				}
				PressedSettings();
				break;
			case ButtonAction.Press:
				if (showdebug)
				{
					Debug.Log(base.name + " Doing the press actions ");
				}
				PressedSettings();
				break;
			case ButtonAction.Toggle:
				value.SetValue((!(input.value > 0.5f)) ? 1 : 0);
				if (showdebug)
				{
					Debug.Log(base.name + " Doing the toggle actions ");
				}
				pressedState = true;
				Snap(pressedState);
				break;
			default:
				throw new InvalidOperationException();
			}
		}

		private void OnRelease()
		{
			if (!usebuttonaction)
			{
				return;
			}
			switch (buttonAction)
			{
			case ButtonAction.Hold:
				value.SetValue(0f);
				if (showdebug)
				{
					Debug.Log(base.name + " Releasing with a Hold action ");
				}
				ReleaseSettings();
				break;
			case ButtonAction.Press:
				if (showdebug)
				{
					Debug.Log(base.name + " Releasing with a press action  ");
				}
				pressedState = false;
				Snap(pressedState);
				value.SetValue(1f);
				break;
			case ButtonAction.Toggle:
				value.SetValue((input.value > 0.5f) ? 1 : 0);
				pressedState = false;
				if ((double)input.value > 0.5)
				{
					if (showdebug)
					{
						Debug.Log(base.name + " Input Value is 1 ");
					}
					value.SetValue(1f);
					Snap(pressed: false);
				}
				else
				{
					if (showdebug)
					{
						Debug.Log(base.name + " Input value is 0  ");
					}
					value.SetValue(0f);
					Snap(pressed: false);
				}
				break;
			default:
				throw new InvalidOperationException();
			}
		}

		private void ReleaseSettings()
		{
			pressedState = false;
			Snap(pressedState);
			value.SetValue(0f);
		}

		private void PressedSettings()
		{
			pressedState = true;
			Snap(pressedState);
			value.SetValue(1f);
		}

		public void ResetButton()
		{
			if (showdebug)
			{
				Debug.Log(base.name + " Resetting this button ");
			}
			switch (buttonAction)
			{
			case ButtonAction.Press:
				pressedState = false;
				value.SetValue(0f);
				break;
			case ButtonAction.Toggle:
				ReleaseSettings();
				break;
			}
			reset.value = 0f;
		}

		private void Snap(bool pressed)
		{
			if (showdebug)
			{
				Debug.Log(base.name + pressed);
			}
			if (activationMode == ButtonActivationMode.Touch)
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
			if (NetGame.isClient && !ReplayRecorder.isPlaying)
			{
				if (showdebug)
				{
					Debug.Log(base.name + " Client , dont do anything  ");
				}
				return;
			}
			float num = (activationMode == ButtonActivationMode.Touch) ? (-1f) : CalculateNormalizedOffset();
			bool flag = num > 0.5f;
			bool flag2 = num < 0.25f;
			bool flag3 = activationMode != 0 && CalculateGrabbed();
			if (flag && buttonAction == ButtonAction.Hold && slideJointRigidBody != null && slideJointRigidBody.IsSleeping())
			{
				slideJointRigidBody.WakeUp();
			}
			if (!pressedState && (flag || flag3))
			{
				if (showdebug)
				{
					Debug.Log(base.name + " Pressed state should be true ");
				}
				if (usebuttonaction)
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
				if (showdebug)
				{
					Debug.Log(base.name + " Pressed state should be false ");
				}
				if (usebuttonaction && pressedState)
				{
					if (showdebug)
					{
						Debug.Log(base.name + " using button action ");
					}
					OnRelease();
				}
				else if (!usebuttonaction && pressedState)
				{
					if (showdebug)
					{
						Debug.Log(base.name + " not using button action ");
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
