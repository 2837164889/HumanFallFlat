using InControl;
using UnityEngine;

public class PlayerActions : PlayerActionSet
{
	public PlayerAction LeftHand;

	public PlayerAction RightHand;

	public PlayerAction Jump;

	public PlayerAction Unconscious;

	public PlayerAction Left;

	public PlayerAction Right;

	public PlayerAction Forward;

	public PlayerAction Back;

	public PlayerAction LookLeft;

	public PlayerAction LookRight;

	public PlayerAction LookUp;

	public PlayerAction LookDown;

	public PlayerTwoAxisAction Move;

	public PlayerTwoAxisAction Look;

	public PlayerAction Calibrate;

	public PlayerAction ViewPlayerNames;

	public PlayerAction ShootFireworks;

	public float HScale = 1f;

	public float VScale = 1f;

	public float VPow = 1f;

	public float LookX => Look.X * HScale;

	public float LookY => Look.Y * VScale;

	public float LookYNormalized => Mathf.Sign(Look.Y) * Mathf.Pow(Mathf.Abs(Look.Y), VPow);

	public PlayerActions()
	{
		LeftHand = CreatePlayerAction("Left Hand");
		RightHand = CreatePlayerAction("Right Hand");
		Jump = CreatePlayerAction("Jump");
		Unconscious = CreatePlayerAction("Play Dead");
		Left = CreatePlayerAction("Move Left");
		Right = CreatePlayerAction("Move Right");
		Forward = CreatePlayerAction("Move Forward");
		Back = CreatePlayerAction("Move Back");
		Move = CreateTwoAxisPlayerAction(Left, Right, Back, Forward);
		LookLeft = CreatePlayerAction("Look Left");
		LookRight = CreatePlayerAction("Look Right");
		LookUp = CreatePlayerAction("Look Up");
		LookDown = CreatePlayerAction("Look Down");
		Calibrate = CreatePlayerAction("Calibrate");
		ViewPlayerNames = CreatePlayerAction("View Player Names");
		ShootFireworks = CreatePlayerAction("Shoot Fireworks");
		Look = CreateTwoAxisPlayerAction(LookLeft, LookRight, LookDown, LookUp);
		LookLeft.Raw = true;
		LookRight.Raw = true;
		LookUp.Raw = true;
		LookDown.Raw = true;
		Look.Raw = true;
	}

	public void ApplyMouseSensitivity(bool mouseInvert, float mouseSensitivity)
	{
		MouseBindingSource.ScaleX = (MouseBindingSource.ScaleY = Options.ThreePointMap(mouseSensitivity, 0f, 0.3f, 1f, 0.05f, 0.15f, 0.4f));
		if (mouseInvert)
		{
			MouseBindingSource.ScaleY *= -1f;
		}
	}

	public static PlayerActions CreateWithDefaultBindings()
	{
		PlayerActions playerActions = new PlayerActions();
		playerActions.HScale = 1f;
		playerActions.VScale = 2f;
		playerActions.LeftHand.AddDefaultBinding(Mouse.LeftButton);
		playerActions.RightHand.AddDefaultBinding(Mouse.RightButton);
		playerActions.Jump.AddDefaultBinding(Key.Space);
		playerActions.Unconscious.AddDefaultBinding(Key.Y);
		playerActions.Forward.AddDefaultBinding(Key.W);
		playerActions.Back.AddDefaultBinding(Key.S);
		playerActions.Left.AddDefaultBinding(Key.A);
		playerActions.Right.AddDefaultBinding(Key.D);
		playerActions.LookUp.AddDefaultBinding(Mouse.PositiveY);
		playerActions.LookDown.AddDefaultBinding(Mouse.NegativeY);
		playerActions.LookLeft.AddDefaultBinding(Mouse.NegativeX);
		playerActions.LookRight.AddDefaultBinding(Mouse.PositiveX);
		playerActions.ListenOptions.IncludeUnknownControllers = false;
		playerActions.ListenOptions.MaxAllowedBindings = 1u;
		playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
		playerActions.ListenOptions.IncludeMouseButtons = true;
		playerActions.ListenOptions.IncludeControllers = false;
		playerActions.ListenOptions.IncludeNonStandardControls = true;
		playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = false;
		playerActions.ViewPlayerNames.AddDefaultBinding(Key.V);
		playerActions.ShootFireworks.AddDefaultBinding(Key.E);
		return playerActions;
	}

	public static PlayerActions CreateWithControllerBindings(int layout, bool invert, bool invertH, float hSensitivity, float vSensitivity)
	{
		PlayerActions playerActions = new PlayerActions();
		playerActions.HScale = Options.ThreePointMap(hSensitivity, 0f, 0.3f, 1f, 0.5f, 1f, 3f);
		playerActions.VScale = Options.ThreePointMap(vSensitivity, 0f, 0.3f, 1f, 0.5f, 1f, 3f);
		playerActions.VPow = Options.ThreePointMap(vSensitivity, 0f, 0.3f, 1f, 2f, 1f, 0.2f);
		playerActions.LeftHand.AddDefaultBinding(InputControlType.LeftTrigger);
		playerActions.RightHand.AddDefaultBinding(InputControlType.RightTrigger);
		playerActions.Jump.AddDefaultBinding(InputControlType.Action1);
		playerActions.Unconscious.AddDefaultBinding(InputControlType.Action3);
		playerActions.Calibrate.AddDefaultBinding(InputControlType.DPadDown);
		playerActions.ShootFireworks.AddDefaultBinding(InputControlType.Action2);
		if (layout == 0)
		{
			playerActions.Forward.AddDefaultBinding(InputControlType.LeftStickUp);
			playerActions.Back.AddDefaultBinding(InputControlType.LeftStickDown);
			playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
			playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
			playerActions.LookUp.AddDefaultBinding((!invert) ? InputControlType.RightStickUp : InputControlType.RightStickDown);
			playerActions.LookDown.AddDefaultBinding((!invert) ? InputControlType.RightStickDown : InputControlType.RightStickUp);
			playerActions.LookLeft.AddDefaultBinding((!invertH) ? InputControlType.RightStickLeft : InputControlType.RightStickRight);
			playerActions.LookRight.AddDefaultBinding((!invertH) ? InputControlType.RightStickRight : InputControlType.RightStickLeft);
			playerActions.ViewPlayerNames.AddDefaultBinding(InputControlType.RightStickButton);
		}
		else
		{
			playerActions.Forward.AddDefaultBinding(InputControlType.RightStickUp);
			playerActions.Back.AddDefaultBinding(InputControlType.RightStickDown);
			playerActions.Left.AddDefaultBinding(InputControlType.RightStickLeft);
			playerActions.Right.AddDefaultBinding(InputControlType.RightStickRight);
			playerActions.LookUp.AddDefaultBinding((!invert) ? InputControlType.LeftStickUp : InputControlType.LeftStickDown);
			playerActions.LookDown.AddDefaultBinding(invert ? InputControlType.LeftStickUp : InputControlType.LeftStickDown);
			playerActions.LookLeft.AddDefaultBinding((!invertH) ? InputControlType.LeftStickLeft : InputControlType.LeftStickRight);
			playerActions.LookRight.AddDefaultBinding((!invertH) ? InputControlType.LeftStickRight : InputControlType.LeftStickLeft);
			playerActions.ViewPlayerNames.AddDefaultBinding(InputControlType.LeftStickButton);
		}
		return playerActions;
	}

	public bool HasInput()
	{
		if (Move.Value != Vector2.zero)
		{
			return true;
		}
		if (Look.Value != Vector2.zero)
		{
			return true;
		}
		for (int i = 0; i < base.Actions.Count; i++)
		{
			PlayerAction playerAction = base.Actions[i];
			if (playerAction.Value != 0f)
			{
				return true;
			}
		}
		return false;
	}
}
